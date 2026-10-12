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
#pragma warning restore CA1034
}
