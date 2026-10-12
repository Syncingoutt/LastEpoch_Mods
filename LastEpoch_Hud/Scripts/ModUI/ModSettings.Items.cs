using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Scripts.Mods.Items.Requirements;
using LastEpoch_Hud.Scripts.ModUI.Settings;

namespace LastEpoch_Hud.Scripts.ModUI;

public static partial class ModSettings
{
#pragma warning disable CA1034 // nested like every settings section
    /// <summary>Remove item requirements (class, level, set). Any change re-applies them.</summary>
    public static class ItemsRequirements
    {
        public static readonly SettingsGroup Group = new(ItemsRequirementsMap.Group);

        public static readonly BoolSetting Level = Group.Bool("Level");
        public static readonly BoolSetting Class = Group.Bool("Class");
        public static readonly BoolSetting Set = Group.Bool("Set");

        static ItemsRequirements()
        {
            Level.Changed += OnChanged;
            Class.Changed += OnChanged;
            Set.Changed += OnChanged;
        }

        private static void OnChanged(bool _)
        {
            Items_Update.Reqs();
        }
    }

    /// <summary>Items &gt; Drop overrides for newly rolled drops.</summary>
    public static class ItemsDrop
    {
        public static readonly SettingsGroup Group = new(ItemsDropMap.Group);

        public static readonly BoolSetting ForceUnique = Group.Bool("ForceUnique");
        public static readonly BoolSetting ForceSet = Group.Bool("ForceSet");
        public static readonly BoolSetting ForceLegendary = Group.Bool("ForceLegendary");
        public static readonly BoolSetting ForceSeal = Group.Bool("ForceSeal");

        public static readonly RangeSetting Implicits = Group.Range(
            "Implicits",
            ItemSliderLimits.DropImplicits.Min,
            ItemSliderLimits.DropImplicits.Max
        );
        public static readonly RangeSetting ForgingPotential = Group.Range(
            "ForgingPotential",
            ItemSliderLimits.DropForgingPotential.Min,
            ItemSliderLimits.DropForgingPotential.Max
        );
        public static readonly RangeSetting SealTier = Group.Range(
            "SealTier",
            ItemSliderLimits.DropSealTier.Min,
            ItemSliderLimits.DropSealTier.Max
        );
        public static readonly RangeSetting SealValue = Group.Range(
            "SealValue",
            ItemSliderLimits.DropSealValue.Min,
            ItemSliderLimits.DropSealValue.Max
        );
        public static readonly RangeSetting AffixCount = Group.Range(
            "AffixCount",
            ItemSliderLimits.DropAffixCount.Min,
            ItemSliderLimits.DropAffixCount.Max,
            defaultMin: ItemSliderLimits.DropAffixCount.Min,
            defaultMax: 4f
        );
        public static readonly RangeSetting IdolAffixCount = Group.Range(
            "IdolAffixCount",
            ItemSliderLimits.DropIdolAffixCount.Min,
            ItemSliderLimits.DropIdolAffixCount.Max
        );
        public static readonly RangeSetting AffixTiers = Group.Range(
            "AffixTiers",
            ItemSliderLimits.DropAffixTiers.Min,
            ItemSliderLimits.DropAffixTiers.Max
        );
        public static readonly RangeSetting AffixValues = Group.Range(
            "AffixValues",
            ItemSliderLimits.DropAffixValues.Min,
            ItemSliderLimits.DropAffixValues.Max
        );
        public static readonly RangeSetting UniqueMods = Group.Range(
            "UniqueMods",
            ItemSliderLimits.DropUniqueMods.Min,
            ItemSliderLimits.DropUniqueMods.Max
        );
        public static readonly RangeSetting LegendaryPotential = Group.Range(
            "LegendaryPotential",
            ItemSliderLimits.DropLegendaryPotential.Min,
            ItemSliderLimits.DropLegendaryPotential.Max
        );
        public static readonly RangeSetting WeaverWill = Group.Range(
            "WeaverWill",
            ItemSliderLimits.DropWeaverWill.Min,
            ItemSliderLimits.DropWeaverWill.Max
        );

        /// <summary>Settings the Unique/Set exclusivity clears.</summary>
        public static IReadOnlyList<BoolSetting> ForcedRarities { get; } =
            new[] { ForceUnique, ForceSet, ForceLegendary };
    }

    /// <summary>Items pickup, store, sell, shatter and notification options.</summary>
    public static class ItemsPickup
    {
        public const float AutoStoreTimerSeconds = 10f;

        public static readonly SettingsGroup Group = new(ItemsPickupMap.Group);

        public static readonly BoolSetting AutoPickupGold = Group.Bool("AutoPickupGold");
        public static readonly BoolSetting AutoPickupKeys = Group.Bool("AutoPickupKeys");
        public static readonly BoolSetting AutoPickupPotions = Group.Bool("AutoPickupPotions");
        public static readonly BoolSetting AutoPickupXpTome = Group.Bool("AutoPickupXpTome");
        public static readonly BoolSetting AutoPickupFavorTome = Group.Bool("AutoPickupFavorTome");
        public static readonly BoolSetting AutoPickupMemoryAmber = Group.Bool(
            "AutoPickupMemoryAmber"
        );
        public static readonly BoolSetting AutoPickupMaterials = Group.Bool("AutoPickupMaterials");
        public static readonly BoolSetting AutoPickupWovenEchoes = Group.Bool(
            "AutoPickupWovenEchoes"
        );
        public static readonly BoolSetting AutoPickupFromFilter = Group.Bool(
            "AutoPickupFromFilter"
        );
        public static readonly BoolSetting AutoStoreOnDrop = Group.Bool("AutoStoreOnDrop");
        public static readonly BoolSetting AutoStoreOnInventoryOpen = Group.Bool(
            "AutoStoreOnInventoryOpen"
        );
        public static readonly BoolSetting AutoSellFromFilter = Group.Bool("AutoSellFromFilter");
        public static readonly BoolSetting AutoShatterFromFilter = Group.Bool(
            "AutoShatterFromFilter"
        );
        public static readonly BoolSetting AutoShatterUseRune = Group.Bool("AutoShatterUseRune");
        public static readonly BoolSetting RangePickup = Group.Bool("RangePickup");
        public static readonly BoolSetting HideMaterialsNotifications = Group.Bool(
            "HideMaterialsNotifications"
        );

        public static readonly FloatSetting AutoStoreTimer = Group.Float(
            "AutoStoreTimer",
            AutoStoreTimerSeconds
        );
        public static readonly FloatSetting AutoShatterChance = Group.Float(
            "AutoShatterChance",
            100f,
            min: ItemSliderLimits.AutoShatterChance.Min,
            max: ItemSliderLimits.AutoShatterChance.Max
        );
        public static readonly FloatSetting AutoShatterAffixChance = Group.Float(
            "AutoShatterAffixChance",
            100f,
            min: ItemSliderLimits.AutoShatterAffixChance.Min,
            max: ItemSliderLimits.AutoShatterAffixChance.Max
        );
        public static readonly FloatSetting AutoShatterQuantityChance = Group.Float(
            "AutoShatterQuantityChance",
            0f,
            min: ItemSliderLimits.AutoShatterQuantityChance.Min,
            max: ItemSliderLimits.AutoShatterQuantityChance.Max
        );
    }

    /// <summary>Items &gt; Crafting Slot overrides applied when an item enters the crafting slot.</summary>
    public static class ItemsCraftingSlot
    {
        public static readonly SettingsGroup Group = new(ItemsCraftingSlotMap.Group);

        public static readonly BoolSetting EnableMod = Group.Bool("EnableMod");

        public static readonly FloatSetting ForgingPotential = Group.Float(
            "ForgingPotential",
            255f,
            min: ItemSliderLimits.CraftingSlotForgingPotential.Min,
            max: ItemSliderLimits.CraftingSlotForgingPotential.Max
        );
        public static readonly FloatSetting Implicit0 = Group.Float(
            "Implicit0",
            255f,
            min: ItemSliderLimits.CraftingSlotImplicit.Min,
            max: ItemSliderLimits.CraftingSlotImplicit.Max
        );
        public static readonly FloatSetting Implicit1 = Group.Float(
            "Implicit1",
            255f,
            min: ItemSliderLimits.CraftingSlotImplicit.Min,
            max: ItemSliderLimits.CraftingSlotImplicit.Max
        );
        public static readonly FloatSetting Implicit2 = Group.Float(
            "Implicit2",
            255f,
            min: ItemSliderLimits.CraftingSlotImplicit.Min,
            max: ItemSliderLimits.CraftingSlotImplicit.Max
        );
        public static readonly FloatSetting SealTier = Group.Float(
            "SealTier",
            6f,
            min: ItemSliderLimits.CraftingSlotSealTier.Min,
            max: ItemSliderLimits.CraftingSlotSealTier.Max
        );
        public static readonly FloatSetting SealValue = Group.Float(
            "SealValue",
            255f,
            min: ItemSliderLimits.CraftingSlotSealValue.Min,
            max: ItemSliderLimits.CraftingSlotSealValue.Max
        );
        public static readonly FloatSetting Affix0Tier = Group.Float(
            "Affix0Tier",
            6f,
            min: ItemSliderLimits.CraftingSlotAffixTier.Min,
            max: ItemSliderLimits.CraftingSlotAffixTier.Max
        );
        public static readonly FloatSetting Affix1Tier = Group.Float(
            "Affix1Tier",
            6f,
            min: ItemSliderLimits.CraftingSlotAffixTier.Min,
            max: ItemSliderLimits.CraftingSlotAffixTier.Max
        );
        public static readonly FloatSetting Affix2Tier = Group.Float(
            "Affix2Tier",
            6f,
            min: ItemSliderLimits.CraftingSlotAffixTier.Min,
            max: ItemSliderLimits.CraftingSlotAffixTier.Max
        );
        public static readonly FloatSetting Affix3Tier = Group.Float(
            "Affix3Tier",
            6f,
            min: ItemSliderLimits.CraftingSlotAffixTier.Min,
            max: ItemSliderLimits.CraftingSlotAffixTier.Max
        );
        public static readonly FloatSetting Affix0Value = Group.Float(
            "Affix0Value",
            255f,
            min: ItemSliderLimits.CraftingSlotAffixValue.Min,
            max: ItemSliderLimits.CraftingSlotAffixValue.Max
        );
        public static readonly FloatSetting Affix1Value = Group.Float(
            "Affix1Value",
            255f,
            min: ItemSliderLimits.CraftingSlotAffixValue.Min,
            max: ItemSliderLimits.CraftingSlotAffixValue.Max
        );
        public static readonly FloatSetting Affix2Value = Group.Float(
            "Affix2Value",
            255f,
            min: ItemSliderLimits.CraftingSlotAffixValue.Min,
            max: ItemSliderLimits.CraftingSlotAffixValue.Max
        );
        public static readonly FloatSetting Affix3Value = Group.Float(
            "Affix3Value",
            255f,
            min: ItemSliderLimits.CraftingSlotAffixValue.Min,
            max: ItemSliderLimits.CraftingSlotAffixValue.Max
        );
        public static readonly FloatSetting UniqueMod0 = Group.Float(
            "UniqueMod0",
            255f,
            min: ItemSliderLimits.CraftingSlotUniqueMod.Min,
            max: ItemSliderLimits.CraftingSlotUniqueMod.Max
        );
        public static readonly FloatSetting UniqueMod1 = Group.Float(
            "UniqueMod1",
            255f,
            min: ItemSliderLimits.CraftingSlotUniqueMod.Min,
            max: ItemSliderLimits.CraftingSlotUniqueMod.Max
        );
        public static readonly FloatSetting UniqueMod2 = Group.Float(
            "UniqueMod2",
            255f,
            min: ItemSliderLimits.CraftingSlotUniqueMod.Min,
            max: ItemSliderLimits.CraftingSlotUniqueMod.Max
        );
        public static readonly FloatSetting UniqueMod3 = Group.Float(
            "UniqueMod3",
            255f,
            min: ItemSliderLimits.CraftingSlotUniqueMod.Min,
            max: ItemSliderLimits.CraftingSlotUniqueMod.Max
        );
        public static readonly FloatSetting UniqueMod4 = Group.Float(
            "UniqueMod4",
            255f,
            min: ItemSliderLimits.CraftingSlotUniqueMod.Min,
            max: ItemSliderLimits.CraftingSlotUniqueMod.Max
        );
        public static readonly FloatSetting UniqueMod5 = Group.Float(
            "UniqueMod5",
            255f,
            min: ItemSliderLimits.CraftingSlotUniqueMod.Min,
            max: ItemSliderLimits.CraftingSlotUniqueMod.Max
        );
        public static readonly FloatSetting UniqueMod6 = Group.Float(
            "UniqueMod6",
            255f,
            min: ItemSliderLimits.CraftingSlotUniqueMod.Min,
            max: ItemSliderLimits.CraftingSlotUniqueMod.Max
        );
        public static readonly FloatSetting UniqueMod7 = Group.Float(
            "UniqueMod7",
            255f,
            min: ItemSliderLimits.CraftingSlotUniqueMod.Min,
            max: ItemSliderLimits.CraftingSlotUniqueMod.Max
        );
        public static readonly FloatSetting LegendaryPotential = Group.Float(
            "LegendaryPotential",
            4f,
            min: ItemSliderLimits.CraftingSlotLegendaryPotential.Min,
            max: ItemSliderLimits.CraftingSlotLegendaryPotential.Max
        );
        public static readonly FloatSetting WeaverWill = Group.Float(
            "WeaverWill",
            28f,
            min: ItemSliderLimits.CraftingSlotWeaverWill.Min,
            max: ItemSliderLimits.CraftingSlotWeaverWill.Max
        );

        /// <summary>Implicit roll overrides, slot order.</summary>
        public static IReadOnlyList<FloatSetting> Implicits { get; } =
            new[] { Implicit0, Implicit1, Implicit2 };

        /// <summary>Affix tier overrides, slot order.</summary>
        public static IReadOnlyList<FloatSetting> AffixTiers { get; } =
            new[] { Affix0Tier, Affix1Tier, Affix2Tier, Affix3Tier };

        /// <summary>Affix roll overrides, slot order.</summary>
        public static IReadOnlyList<FloatSetting> AffixValues { get; } =
            new[] { Affix0Value, Affix1Value, Affix2Value, Affix3Value };

        /// <summary>Unique mod roll overrides, slot order.</summary>
        public static IReadOnlyList<FloatSetting> UniqueMods { get; } =
            new[]
            {
                UniqueMod0,
                UniqueMod1,
                UniqueMod2,
                UniqueMod3,
                UniqueMod4,
                UniqueMod5,
                UniqueMod6,
                UniqueMod7,
            };

        /// <summary>Every override setting, in page order.</summary>
        public static IReadOnlyList<FloatSetting> Overrides { get; } =
            new[]
            {
                ForgingPotential,
                Implicit0,
                Implicit1,
                Implicit2,
                SealTier,
                SealValue,
                Affix0Tier,
                Affix0Value,
                Affix1Tier,
                Affix1Value,
                Affix2Tier,
                Affix2Value,
                Affix3Tier,
                Affix3Value,
                UniqueMod0,
                UniqueMod1,
                UniqueMod2,
                UniqueMod3,
                UniqueMod4,
                UniqueMod5,
                UniqueMod6,
                UniqueMod7,
                LegendaryPotential,
                WeaverWill,
            };
    }
#pragma warning restore CA1034
}
