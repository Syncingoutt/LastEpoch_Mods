using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;

namespace LastEpoch_Hud.Scripts.ModUI.ForceDrop;

// Recognition derives from saved affix IDs, so it survives mode changes/reload.
public static class IllegalItemAdapter
{
    static UniqueList.Entry SourcePiece(AffixList.Affix definition)
    {
        if (
            definition.IsNullOrDestroyed()
            || definition.specialAffixType != AffixList.SpecialAffixType.Set
            || UniqueList.instance.IsNullOrDestroyed()
        )
            return null;
        var piece = UniqueList.getUnique(definition.uniqueId);
        return !piece.IsNullOrDestroyed() && piece.isSetItem ? piece : null;
    }

    static bool ForeignSetPiece(ItemData item, out UniqueList.Entry source)
    {
        source = null;
        if (
            item.IsNullOrDestroyed()
            || item.affixes.IsNullOrDestroyed()
            || item.rarity < 7
            || UniqueList.instance.IsNullOrDestroyed()
        )
            return false;
        var own = UniqueList.getUnique(item.uniqueID);
        if (own.IsNullOrDestroyed() || own.isSetItem)
            return false;
        foreach (var affix in item.affixes)
        {
            if (affix.IsNullOrDestroyed())
                return false;
            if (affix.specialAffixType != AffixList.SpecialAffixType.Set)
                continue;
            var piece = SourcePiece(ForceDropCatalog.Find(affix.affixId));
            if (piece.IsNullOrDestroyed() || !source.IsNullOrDestroyed())
                return false;
            source = piece;
        }
        return !source.IsNullOrDestroyed();
    }

    // Added affixes need Legendary serialization. A Set entry still owns its
    // original piece identity; this can be reconstructed from the saved item.
    static bool AdditionalSetPiece(ItemData item, out UniqueList.Entry source)
    {
        source = null;
        if (
            !item.IsNullOrDestroyed()
            && item.rarity == 9
            && !item.affixes.IsNullOrDestroyed()
            && item.affixes.Count > 0
            && !UniqueList.instance.IsNullOrDestroyed()
        )
        {
            var own = UniqueList.getUnique(item.uniqueID);
            if (!own.IsNullOrDestroyed() && own.isSetItem)
            {
                source = own;
                return true;
            }
        }
        return ForeignSetPiece(item, out source);
    }

    public static void VerifyMembership(ItemDataUnpacked item)
    {
        if (item.rarity < 7 || UniqueList.instance.IsNullOrDestroyed())
            return;
        var own = UniqueList.getUnique(item.uniqueID);
        if (own.IsNullOrDestroyed())
            return;
        int count = 0;
        foreach (var affix in item.affixes)
            if (affix.specialAffixType == AffixList.SpecialAffixType.Set)
                count++;
        if (!own.isSetItem && count == 0)
            return;
        UniqueList.Entry source = own;
        if (
            (!own.isSetItem && (count != 1 || !AdditionalSetPiece(item, out source)))
            || !item.grantsSetBonus()
            || item.getSetItemUniqueId() != source.uniqueID
            || item.SetId != source.setID
        )
            throw new InvalidOperationException(
                "Set-piece identity did not survive packing; no item was dropped."
            );
    }

    [HarmonyPatch(typeof(ItemDataUnpacked), nameof(ItemDataUnpacked.MakeFullName))]
    static class PreserveUniqueName
    {
        [HarmonyPostfix]
        static void Postfix(ItemDataUnpacked __instance)
        {
            if (!ForeignSetPiece(__instance, out _))
                return;
            var own = UniqueList.getUnique(__instance.uniqueID);
            string name = null;
            try
            {
                name = Il2Cpp.Localization.Items.GetUniqueName(own.uniqueID, true, false);
            }
            catch { }
            if (string.IsNullOrWhiteSpace(name))
                name = string.IsNullOrWhiteSpace(own.displayName) ? own.name : own.displayName;
            if (!string.IsNullOrWhiteSpace(name))
                __instance.FullName = name;
        }
    }

    [HarmonyPatch(typeof(Il2CppLE.UI.Tooltips.ItemTooltipContentBuilder), "GetItemTooltipInfo")]
    static class SetTooltipContent
    {
        [HarmonyPostfix]
        static void Postfix(
            ItemDataUnpacked __0,
            TooltipItemManager.SlotType __1,
            Il2CppLE.UI.Tooltips.ItemTooltipInfo __result
        )
        {
            if (__result.IsNullOrDestroyed() || !AdditionalSetPiece(__0, out var source))
                return;
            Il2CppLE.UI.Tooltips.ItemTooltipContentBuilder.SetSetItemInfo(__result, __0, source);
            Il2CppLE.UI.Tooltips.ItemTooltipContentBuilder.SetSlotTypeOnAllModifiers(__result, __1);
        }
    }

    // The native dispatcher inlines default content for ordinary display mode.
    // Patch its final result too, so every legacy layout receives set bonus text.
    [HarmonyPatch(typeof(TooltipItemManager), nameof(TooltipItemManager.CreateTooltipContent))]
    static class DispatchedSetTooltipContent
    {
        [HarmonyPostfix]
        static void Postfix(
            TooltipItemManager.ItemTooltipParameters __0,
            ref UITooltipItem.ItemTooltipInfo __result
        )
        {
            if (!__0.IsNullOrDestroyed())
                AddLegacySetBonus(__0.Item, __0.TooltipMode, __0.SlotType, ref __result);
        }
    }

    // The game retains the older content path for some tooltip layouts.
    [HarmonyPatch(typeof(TooltipItemManager), "CreateTooltipDefaultContent")]
    static class LegacySetTooltipContent
    {
        [HarmonyPostfix]
        static void Postfix(
            ItemDataUnpacked __0,
            TooltipMode __1,
            TooltipItemManager.SlotType __2,
            ref UITooltipItem.ItemTooltipInfo __result
        ) => AddLegacySetBonus(__0, __1, __2, ref __result);
    }

    [HarmonyPatch(typeof(TooltipItemManager), "CreateTooltipFullContent")]
    static class FullSetTooltipContent
    {
        [HarmonyPostfix]
        static void Postfix(
            ItemDataUnpacked __0,
            TooltipMode __1,
            TooltipItemManager.SlotType __2,
            ref UITooltipItem.ItemTooltipInfo __result
        ) => AddLegacySetBonus(__0, __1, __2, ref __result);
    }

    // Keep the registered native row pool. Dense illegal set combinations can
    // exceed it, so retain overflow text in the final existing row.
    [HarmonyPatch(typeof(UITooltipItem), nameof(UITooltipItem.UpdateAffixesText))]
    static class SetTooltipRows
    {
        [HarmonyPrefix]
        static void Prefix(
            UITooltipItem __instance,
            ItemData __0,
            ref UITooltipItem.ItemTooltipInfo __1
        )
        {
            if (
                !AdditionalSetPiece(__0, out _)
                || __1.uniqueAffixesText.IsNullOrDestroyed()
                || __instance.uniqueAffixesText.IsNullOrDestroyed()
            )
                return;
            int capacity = __instance.uniqueAffixesText.Count;
            if (capacity <= 0 || __1.uniqueAffixesText.Count <= capacity)
                return;
            var lines = new Il2CppSystem.Collections.Generic.List<Il2CppSystem.ValueTuple<
                string,
                bool
            >>();
            for (int i = 0; i < capacity - 1; i++)
                lines.Add(__1.uniqueAffixesText[i]);
            var overflow = new System.Text.StringBuilder();
            var final = __1.uniqueAffixesText[capacity - 1];
            for (int i = capacity - 1; i < __1.uniqueAffixesText.Count; i++)
            {
                if (overflow.Length > 0)
                    overflow.Append("\n\n");
                overflow.Append(__1.uniqueAffixesText[i].Item1);
            }
            lines.Add(new Il2CppSystem.ValueTuple<string, bool>(overflow.ToString(), final.Item2));
            __1.uniqueAffixesText = lines;
        }
    }

    static void AddLegacySetBonus(
        ItemDataUnpacked item,
        TooltipMode mode,
        TooltipItemManager.SlotType slot,
        ref UITooltipItem.ItemTooltipInfo info
    )
    {
        if (!AdditionalSetPiece(item, out var source))
            return;
        var manager = ItemContainersManager.Instance;
        int count = manager.IsNullOrDestroyed() ? 0 : manager.getGearCountForSetID(source.setID);
        var bonuses = TooltipItemManager.GetSetBonus(item, source, mode, slot, count);
        if (bonuses.IsNullOrDestroyed())
            return;
        var lines = new Il2CppSystem.Collections.Generic.List<Il2CppSystem.ValueTuple<
            string,
            bool
        >>();
        if (!info.uniqueAffixesText.IsNullOrDestroyed())
            foreach (var line in info.uniqueAffixesText)
                lines.Add(line);
        foreach (string bonus in bonuses)
        {
            bool present = false;
            foreach (var line in lines)
                present |= line.Item1 == bonus;
            if (!present)
                lines.Add(new Il2CppSystem.ValueTuple<string, bool>(bonus, true));
        }
        info.uniqueAffixesText = lines;
    }

    static HashSet<ushort> EquippedSetPieces(
        ItemContainersManager manager,
        byte setId,
        out bool hasAdditional
    )
    {
        var pieces = new HashSet<ushort>();
        hasAdditional = false;
        if (
            manager.IsNullOrDestroyed()
            || manager.equipment.IsNullOrDestroyed()
            || manager.equipment.Containers.IsNullOrDestroyed()
            || UniqueList.instance.IsNullOrDestroyed()
        )
            return pieces;
        foreach (var container in manager.equipment.Containers)
        {
            if (
                container.IsNullOrDestroyed()
                || !container.TryGetContentItemData(out ItemData item)
                || item.IsNullOrDestroyed()
            )
                continue;
            if (AdditionalSetPiece(item, out var source))
            {
                if (source.setID == setId)
                {
                    pieces.Add(source.uniqueID);
                    hasAdditional = true;
                }
            }
            else if (item.grantsSetBonus())
            {
                var nativeSource = UniqueList.getUnique(item.getSetItemUniqueId());
                if (
                    !nativeSource.IsNullOrDestroyed()
                    && nativeSource.isSetItem
                    && nativeSource.setID == setId
                )
                    pieces.Add(nativeSource.uniqueID);
            }
        }
        return pieces;
    }

    [HarmonyPatch(
        typeof(ItemContainersManager),
        nameof(ItemContainersManager.getGearCountForSetID)
    )]
    static class EquippedSetCount
    {
        [HarmonyPostfix]
        static void Postfix(ItemContainersManager __instance, byte __0, ref int __result)
        {
            var pieces = EquippedSetPieces(__instance, __0, out bool hasAdditional);
            if (!hasAdditional)
                return;
            // Preserve the existing explicit "remove set requirements" feature.
            if (
                !Save_Manager.instance.IsNullOrDestroyed()
                && Save_Manager.instance.initialized
                && Save_Manager.instance.data.Items.Req.set
            )
                return;
            __result = pieces.Count;
        }
    }

    [HarmonyPatch(typeof(ItemContainersManager), nameof(ItemContainersManager.IsSetItemEquipped))]
    static class EquippedSetPiece
    {
        [HarmonyPostfix]
        static void Postfix(ItemContainersManager __instance, ushort __0, ref bool __result)
        {
            if (__result || UniqueList.instance.IsNullOrDestroyed())
                return;
            var source = UniqueList.getUnique(__0);
            if (source.IsNullOrDestroyed() || !source.isSetItem)
                return;
            var pieces = EquippedSetPieces(__instance, source.setID, out bool hasAdditional);
            if (hasAdditional && pieces.Contains(__0))
                __result = true;
        }
    }

    [HarmonyPatch(typeof(ItemDataUnpacked), "AfterIDChange")]
    static class PackedSetIdentity
    {
        [HarmonyPostfix]
        static void Postfix(ItemDataUnpacked __instance)
        {
            // Native equipment code can read the cached field directly.
            if (AdditionalSetPiece(__instance, out var source))
                __instance.SetId = source.setID;
        }
    }

    // These recognition patches are independent of the creation toggle so an
    // already-created item's bonuses continue working after reload or mode changes.
    [HarmonyPatch(typeof(ItemData), nameof(ItemData.grantsSetBonus))]
    static class GrantsSetBonus
    {
        [HarmonyPostfix]
        static void Postfix(ItemData __instance, ref bool __result)
        {
            if (AdditionalSetPiece(__instance, out _))
                __result = true;
        }
    }

    [HarmonyPatch(typeof(ItemData), nameof(ItemData.getSetItemUniqueId))]
    static class SetPieceIdentity
    {
        [HarmonyPostfix]
        static void Postfix(ItemData __instance, ref ushort __result)
        {
            if (AdditionalSetPiece(__instance, out var source))
                __result = source.uniqueID;
        }
    }

    [HarmonyPatch(typeof(ItemDataUnpacked), "get_SetId")]
    static class SetIdentity
    {
        [HarmonyPostfix]
        static void Postfix(ItemDataUnpacked __instance, ref int __result)
        {
            if (AdditionalSetPiece(__instance, out var source))
                __result = source.setID;
        }
    }
}
