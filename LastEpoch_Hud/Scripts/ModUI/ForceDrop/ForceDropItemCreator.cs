using System;
using System.Collections.Generic;
using Il2Cpp;
using LastEpoch_Hud.Scripts.Core.ForceDrop;
using LastEpoch_Hud.Scripts.Mods.Items;
using LastEpoch_Hud.Scripts.Mods.Items.AutoLoot;

namespace LastEpoch_Hud.Scripts.ModUI.ForceDrop;

// This boundary accepts data, never sliders or other hidden view state.
public static class ForceDropItemCreator
{
    public static void Drop(ResolvedForceDrop request)
    {
        if (
            Refs_Manager.player_actor.IsNullOrDestroyed()
            || Refs_Manager.ground_item_manager.IsNullOrDestroyed()
        )
            throw new InvalidOperationException("Enter the game before dropping items.");
        var item = Create(request);
        ModItemDrop.Drop(
            Refs_Manager.ground_item_manager,
            Refs_Manager.player_actor,
            item.TryCast<ItemData>(),
            Refs_Manager.player_actor.position()
        );
    }

    public static ItemDataUnpacked Create(ResolvedForceDrop request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        using var sealScope = UniqueRegularSealExtension.Begin();
        var item = CreateBeforeCorruption(request);
        // Eligibility/slot allocation runs on the actual uncorrupted item. The
        // adapter marks corruption immediately after addition, BEFORE its first
        // refresh. Finish the native corruption action before final verification.
        if (request.Corruption != null)
        {
            RequireTier(
                ForceDropCatalog.Find(request.Corruption.Id),
                request.Corruption.Tier,
                request.Mode
            );
            CorruptedAffixAdapter.Apply(
                item,
                request.Corruption.Id,
                request.Corruption.Tier,
                request.Corruption.Roll,
                request.IsIllegal,
                request.VariantIds
            );
        }
        if (request.Corrupted)
        {
            item.CorruptWithoutEffect();
            item.SetAsCorrupted();
        }
        int forging = request.Corrupted || request.Rarity >= 7 ? 0 : request.ForgingPotential;
        item.forgingPotential = (byte)Math.Min(63, forging);
        UniqueVariantAdapter.PrepareForPacking(item, request.VariantIds, request.IsIllegal);
        item.RefreshIDAndValues();
        try
        {
            // Stamp can rebuild the saved ID and maintain the mod's >63 FP
            // extension. It must complete BEFORE verification of the final ID.
            if (request.Rarity < 7 && request.ItemType < 100)
                Items_Drop_ForginPotencial.Stamp(item, (byte)forging);
            try
            {
                VerifyRequest(item, request, forging);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException(
                    ex.Message
                        + "; final native flags: regular="
                        + item.hasSealedRegularAffix
                        + ", primordial="
                        + item.hasSealedPrimordialAffix
                        + ", corruption="
                        + item.hasSealedAffixFromCorruption
                        + "; id="
                        + item.GetIDForLogging(),
                    ex
                );
            }
            IllegalItemAdapter.VerifyMembership(item);
            var expected = Snapshot(item);
            var packed = item.GetID();
            var restored = new ItemDataUnpacked(packed);
            string error = expected.Difference(Snapshot(restored), request.IsIllegal);
            if (error.Length != 0)
            {
                var bytes = new List<byte>();
                foreach (byte value in packed)
                    bytes.Add(value);
                throw new InvalidOperationException(
                    error
                        + "; packed="
                        + Convert.ToBase64String(bytes.ToArray())
                        + "; no item was dropped."
                );
            }
            IllegalItemAdapter.VerifyMembership(restored);
            sealScope.Commit(item);
            return item;
        }
        catch
        {
            // A rejected, unspawned item must not leave an FP extension entry.
            if (forging > 63)
                Items_Drop_ForginPotencial.Keep(item, 0);
            throw;
        }
    }

    // The picker uses this same seed as real drops, without RNG or spawning.
    public static ItemDataUnpacked CreateBeforeCorruption(ResolvedForceDrop request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        ValidateIdentity(request);
        bool uncheckedRegularSeal = request.IsIllegal && request.Rarity >= 7;
        foreach (var selected in request.Affixes)
            uncheckedRegularSeal |=
                request.IsIllegal && selected.Seal == ForceDropSeal.Regular && selected.Tier == 7;
        var affixes = new Il2CppSystem.Collections.Generic.List<ItemAffix>();
        ItemAffix regularSeal = null,
            primordialSeal = null;
        int ordinaryCount = 0;
        int prefixes = 0,
            suffixes = 0,
            enchantments = 0,
            weaverAffixes = 0;
        foreach (var selected in request.Affixes)
        {
            var definition = ForceDropCatalog.Find(selected.Id);
            bool enchantment =
                !definition.IsNullOrDestroyed()
                && definition.specialAffixType == AffixList.SpecialAffixType.IdolEnchantment;
            if (!request.IsIllegal)
            {
                var otherIds = new List<int>();
                foreach (var other in request.Affixes)
                    if (other.Id != selected.Id)
                        otherIds.Add(other.Id);
                var context = new ForceDropLegalAffixes(
                    request.ItemType,
                    request.SubType,
                    request.Rarity,
                    otherIds
                );
                string reason = context.OrdinaryReason(
                    definition,
                    selected.Seal != ForceDropSeal.None,
                    enchantment
                );
                if (reason.Length > 0)
                    throw new InvalidOperationException("Affix " + selected.Id + ": " + reason);
            }
            bool primordial = selected.Seal == ForceDropSeal.Primordial;
            if (
                primordial
                && !ForceDropModeRules.CanSealPrimordial(
                    request.ItemType,
                    request.Rarity,
                    ForceDropLegalAffixes.Family(definition),
                    ForceDropCatalog.MaximumTier(definition, 8),
                    request.Mode
                )
            )
                throw new InvalidOperationException(
                    "This affix cannot be Primordial sealed on this item."
                );
            RequireTier(
                definition,
                selected.Tier,
                primordial ? ForceDropMode.Illegal : request.Mode
            );
            // Evolution upgrades its T7 seed and assigns the separate Primordial seal.
            int seedTier =
                primordial || (uncheckedRegularSeal && selected.Seal == ForceDropSeal.Regular)
                    ? Math.Min(6, selected.Tier)
                    : selected.Tier;
            // Native metadata initialization is also used by variants and corruption.
            var affix = new ItemAffix(
                (ushort)selected.Id,
                (byte)seedTier,
                (byte)selected.Roll,
                (byte)request.ItemType,
                SealedAffixType.None
            );
            VerifyAffix(
                affix,
                new ResolvedForceDropAffix(
                    selected.Id,
                    seedTier,
                    selected.Roll,
                    ForceDropSeal.None
                ),
                definition,
                ForceDropSeal.None,
                request.IsIllegal && seedTier == 7
            );
            // Illegal unique regular seals use unchecked insertion below.
            // Do not insert their seed twice.
            if (!(uncheckedRegularSeal && selected.Seal == ForceDropSeal.Regular))
                affixes.Add(affix);
            if (selected.Seal == ForceDropSeal.Regular)
                regularSeal = affix;
            else if (primordial)
                primordialSeal = affix;
            else
            {
                ordinaryCount++;
                if (enchantment)
                    enchantments++;
                else if (definition.type == AffixList.AffixType.PREFIX)
                    prefixes++;
                else
                    suffixes++;
                if (definition.specialAffixType == AffixList.SpecialAffixType.IdolWeaver)
                    weaverAffixes++;
            }
        }
        bool idol = request.ItemType >= 25 && request.ItemType <= 33;
        if (
            !request.IsIllegal
            && (prefixes > (idol ? 1 : 2) || suffixes > (idol ? 1 : 2) || enchantments > 2)
        )
            throw new InvalidOperationException(
                "Too many ordinary prefixes or suffixes for this item."
            );
        var itemContext = new ForceDropLegalAffixes(
            request.ItemType,
            request.SubType,
            request.Rarity,
            Array.Empty<int>()
        );
        if (!request.IsIllegal && itemContext.IsWeaverIdol && weaverAffixes == 0)
            throw new InvalidOperationException(
                "A Weaver idol requires at least one Weaver affix."
            );
        byte rarity = (byte)request.Rarity;
        if (rarity < 7)
            rarity = (byte)ordinaryCount;
        else if (ForceDropPotentialRules.CreatesLegendary(request.Rarity, request.Affixes.Count))
            rarity = 9;
        var item = new ItemDataUnpacked
        {
            LvlReq = new ForceDropLegalAffixes(
                request.ItemType,
                request.SubType,
                request.Rarity,
                Array.Empty<int>()
            ).BaseLevel,
            classReq = ItemList.ClassRequirement.Any,
            itemType = (byte)request.ItemType,
            subType = (ushort)request.SubType,
            uniqueID = (ushort)request.UniqueId,
            rarity = rarity,
            forgingPotential = (byte)Math.Min(63, request.ForgingPotential),
            legendaryPotential = (byte)request.LegendaryPotential,
            weaversWill = (byte)request.WeaversWill,
            affixes = affixes,
            sockets = (byte)affixes.Count,
        };
        WriteRolls(item, request);
        if (!primordialSeal.IsNullOrDestroyed())
        {
            // Find the current native seed before evolving its tier and seal.
            int primordialId = primordialSeal.affixId;
            primordialSeal = null;
            foreach (var candidate in item.affixes)
                if (!candidate.IsNullOrDestroyed() && candidate.affixId == primordialId)
                    primordialSeal = candidate;
            if (primordialSeal.IsNullOrDestroyed())
                throw new InvalidOperationException(
                    "The Primordial seed is missing; no item was dropped."
                );
            item.MakeAffixSealedPrimordialAffix(primordialSeal, true);
            foreach (var selected in request.Affixes)
                if (selected.Seal == ForceDropSeal.Primordial)
                    VerifyAffix(
                        primordialSeal,
                        selected,
                        ForceDropCatalog.Find(selected.Id),
                        ForceDropSeal.Primordial
                    );
        }
        // Evolution can replace a regular seal. Install the independent regular
        // slot after Primordial conversion, with its selected illegal tier.
        // SealAffix is a crafting operation. Illegal unique/T8 creation uses the native
        // unchecked path, which owns the regular seal flag and insertion order.
        if (!regularSeal.IsNullOrDestroyed())
        {
            if (uncheckedRegularSeal)
            {
                var statChanges = new Il2CppSystem.Collections.Generic.List<Stats.Stat>();
                item.AddAffixNoCostOrChecks(
                    regularSeal.affixId,
                    true,
                    regularSeal.affixTier,
                    ref statChanges,
                    new Il2CppSystem.Nullable<byte>(regularSeal.affixRoll),
                    false
                );
            }
            else
                item.SealAffix(regularSeal);
            foreach (var selected in request.Affixes)
                if (
                    selected.Seal == ForceDropSeal.Regular
                    && item.TryGetSealedRegularAffix(out ItemAffix createdSeal)
                )
                    createdSeal.affixTier = (byte)selected.Tier;
            if (
                !item.hasSealedRegularAffix
                || !item.TryGetSealedRegularAffix(out ItemAffix savedSeal)
                || savedSeal.IsNullOrDestroyed()
                || savedSeal.affixId != regularSeal.affixId
            )
                throw new InvalidOperationException(
                    "The game did not create the selected regular seal; no item was dropped."
                );
        }
        UniqueRegularSealExtension.Track(item, request);
        if (request.VariantIds.Count > 0)
        {
            var ids = new int[request.VariantIds.Count];
            for (int i = 0; i < ids.Length; i++)
                ids[i] = request.VariantIds[i];
            UniqueVariantAdapter.Apply(item, request.IsIllegal, ids);
        }
        // Native level-dependent corruption checks must see the real item level.
        foreach (var selected in request.Affixes)
            item.LvlReq = Math.Max(
                item.LvlReq,
                item.CalculateLevelRequirementWithAffixAtTier(selected.Id, selected.Tier)
            );
        return item;
    }

    static void RequireTier(AffixList.Affix definition, int tier, ForceDropMode mode)
    {
        if (
            definition.IsNullOrDestroyed()
            || tier < 0
            || tier
                >= ForceDropModeRules.MaximumTier(ForceDropCatalog.MaximumTier(definition, 8), mode)
        )
            throw new InvalidOperationException("The selected tier does not exist for this affix.");
    }

    static void ValidateIdentity(ResolvedForceDrop request)
    {
        var list = ItemList.get();
        bool found = false;
        if (!list.IsNullOrDestroyed())
        {
            foreach (var type in list.EquippableItems)
                if (type.baseTypeID == request.ItemType)
                    foreach (var sub in type.subItems)
                        if (sub.subTypeID == request.SubType)
                            found = true;
            foreach (var type in list.nonEquippableItems)
                if (type.baseTypeID == request.ItemType)
                    foreach (var sub in type.subItems)
                        if (sub.subTypeID == request.SubType)
                            found = true;
        }
        if (!found)
            throw new InvalidOperationException("The selected base item is unavailable.");
        if (request.Rarity < 7)
        {
            if (request.UniqueId != 0)
                throw new InvalidOperationException("A base item cannot use a unique identity.");
            return;
        }
        var entry = UniqueList.getUnique((ushort)request.UniqueId);
        if (
            entry.IsNullOrDestroyed()
            || entry.baseType != request.ItemType
            || !entry.subTypes.Contains((byte)request.SubType)
            || entry.isSetItem != (request.Rarity == 8)
        )
            throw new InvalidOperationException(
                "The selected unique does not match the base item."
            );
    }

    static void WriteRolls(ItemDataUnpacked item, ResolvedForceDrop request)
    {
        if (request.ImplicitRolls.Count != item.implicitRolls.Count)
            throw new InvalidOperationException("Implicit roll storage is unavailable.");
        for (int i = 0; i < request.ImplicitRolls.Count; i++)
            item.implicitRolls[i] = (byte)request.ImplicitRolls[i];
        if (request.Rarity < 7)
            return;
        if (request.UniqueRolls.Count != item.uniqueRolls.Count)
            throw new InvalidOperationException("Unique roll storage is unavailable.");
        for (int i = 0; i < request.UniqueRolls.Count; i++)
            item.uniqueRolls[i] = (byte)request.UniqueRolls[i];
    }

    static ForceDropSeal Seal(ItemAffix affix)
    {
        if (affix.IsSealedRegular)
            return ForceDropSeal.Regular;
        if (affix.IsSealedPrimordial)
            return ForceDropSeal.Primordial;
        if (affix.IsSealedCorrupted)
            return ForceDropSeal.Corruption;
        if (affix.sealedAffixType != SealedAffixType.None)
            throw new InvalidOperationException("Unknown affix seal type.");
        return ForceDropSeal.None;
    }

    static void VerifyAffix(
        ItemAffix affix,
        ResolvedForceDropAffix selected,
        AffixList.Affix definition,
        ForceDropSeal seal,
        bool allowNativeT8Seal = false
    )
    {
        if (
            affix.IsNullOrDestroyed()
            || affix.affixId != selected.Id
            || affix.affixTier != selected.Tier
            || affix.affixRoll != selected.Roll
            || affix.affixType != definition.type
            || affix.specialAffixType != definition.specialAffixType
            || (
                Seal(affix) != seal
                && !(
                    allowNativeT8Seal
                    && seal == ForceDropSeal.None
                    && selected.Tier == 7
                    && Seal(affix) == ForceDropSeal.Primordial
                )
            )
        )
            throw new InvalidOperationException(
                "Affix "
                    + selected.Id
                    + " changed during construction or packing"
                    + " (expected="
                    + selected.Id
                    + ":"
                    + selected.Tier
                    + ":"
                    + selected.Roll
                    + ":"
                    + seal
                    + ":"
                    + definition.specialAffixType
                    + ":"
                    + definition.type
                    + ", actual="
                    + (
                        affix.IsNullOrDestroyed()
                            ? "missing"
                            : affix.affixId
                                + ":"
                                + affix.affixTier
                                + ":"
                                + affix.affixRoll
                                + ":"
                                + affix.sealedAffixType
                                + ":"
                                + affix.specialAffixType
                                + ":"
                                + affix.affixType
                    )
                    + ")."
            );
    }

    static void VerifyRequest(ItemDataUnpacked item, ResolvedForceDrop request, int forging)
    {
        if (
            item.itemType != request.ItemType
            || item.subType != request.SubType
            || item.uniqueID != request.UniqueId
            || item.legendaryPotential != request.LegendaryPotential
            || item.weaversWill != request.WeaversWill
            || item.corrupted != request.Corrupted
            || item.forgingPotential != forging
        )
            throw new InvalidOperationException(
                "Item identity or potential does not match the resolved request."
            );
        int expectedCount =
            request.Affixes.Count + request.VariantIds.Count + (request.Corruption == null ? 0 : 1);
        if (item.affixes.Count != expectedCount)
            throw new InvalidOperationException(
                "Item affix count does not match the resolved request."
            );
        foreach (var selected in request.Affixes)
        {
            ItemAffix saved = null;
            foreach (var affix in item.affixes)
                if (!affix.IsNullOrDestroyed() && affix.affixId == selected.Id)
                    saved = affix;
            VerifyAffix(
                saved,
                selected,
                ForceDropCatalog.Find(selected.Id),
                ForceDropModeRules.PersistedSeal(selected.Tier, selected.Seal, request.Mode)
            );
        }
        if (request.Corruption != null)
            CorruptedAffixAdapter.VerifySelection(
                item,
                request.Corruption.Id,
                request.Corruption.Tier,
                request.Corruption.Roll
            );
        if (request.VariantIds.Count > 0)
        {
            var ids = new int[request.VariantIds.Count];
            for (int i = 0; i < ids.Length; i++)
                ids[i] = request.VariantIds[i];
            UniqueVariantAdapter.VerifySelection(item, request.IsIllegal, ids);
        }
        for (int i = 0; i < request.ImplicitRolls.Count; i++)
            if (item.implicitRolls[i] != request.ImplicitRolls[i])
                throw new InvalidOperationException(
                    "Implicit roll does not match the resolved request."
                );
        if (request.Rarity >= 7)
            for (int i = 0; i < request.UniqueRolls.Count; i++)
                if (item.uniqueRolls[i] != request.UniqueRolls[i])
                    throw new InvalidOperationException(
                        "Unique roll does not match the resolved request."
                    );
    }

    static ForceDropPackingSnapshot Snapshot(ItemDataUnpacked item)
    {
        var implicits = new List<int>();
        var uniques = new List<int>();
        var affixes = new List<PackedForceDropAffix>();
        foreach (byte roll in item.implicitRolls)
            implicits.Add(roll);
        if (item.isUniqueSetOrLegendary())
            foreach (byte roll in item.uniqueRolls)
                uniques.Add(roll);
        foreach (var affix in item.affixes)
        {
            if (affix.IsNullOrDestroyed())
                throw new InvalidOperationException("Missing packed affix.");
            affixes.Add(
                new PackedForceDropAffix(
                    affix.affixId,
                    affix.affixTier,
                    affix.affixRoll,
                    Seal(affix),
                    (int)affix.specialAffixType,
                    (int)affix.affixType
                )
            );
        }
        return new ForceDropPackingSnapshot(
            item.itemType,
            item.subType,
            item.uniqueID,
            item.rarity,
            Math.Min(63, (int)item.forgingPotential),
            item.legendaryPotential,
            item.weaversWill,
            item.corrupted,
            item.sockets,
            item.hasSealedRegularAffix,
            item.hasSealedPrimordialAffix,
            item.hasSealedAffixFromCorruption,
            implicits,
            uniques,
            affixes
        );
    }
}
