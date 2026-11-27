using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Items.Pickup fields mapped to the ItemsPickup group.</summary>
public static class ItemsPickupMap
{
    public const string Group = "ItemsPickup";

    private const string Prefix = "Items.Pickup.";
    private const string ConfiguredPath = Prefix + "AutoShatter_Configured";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } = Build();

    private static LegacyFieldMapping[] Build()
    {
        List<LegacyFieldMapping> all = new();
        AddBool(all, "Enable_AutoPickup_Gold", "AutoPickupGold");
        AddBool(all, "Enable_AutoPickup_Keys", "AutoPickupKeys");
        AddBool(all, "Enable_AutoPickup_Potions", "AutoPickupPotions");
        AddBool(all, "Enable_AutoPickup_XpTome", "AutoPickupXpTome");
        AddBool(all, "Enable_AutoPickup_FavorTome", "AutoPickupFavorTome");
        AddBool(all, "Enable_AutoPickup_MemoryAmber", "AutoPickupMemoryAmber");
        AddBool(all, "Enable_AutoPickup_Materials", "AutoPickupMaterials");
        AddBool(all, "Enable_AutoPickup_WovenEchoes", "AutoPickupWovenEchoes");
        AddBool(all, "Enable_AutoPickup_FromFilter", "AutoPickupFromFilter");
        AddBool(all, "Enable_AutoStore_OnDrop", "AutoStoreOnDrop");
        AddBool(all, "Enable_AutoStore_OnInventoryOpen", "AutoStoreOnInventoryOpen");
        AddBool(all, "Enable_AutoSell_FromFilter", "AutoSellFromFilter");
        AddBool(all, "Enable_AutoShatter_FromFilter", "AutoShatterFromFilter");
        AddBool(all, "Enable_AutoShatter_UseRune", "AutoShatterUseRune");
        AddBool(all, "Enable_RangePickup", "RangePickup");
        AddBool(all, "Enable_HideMaterialsNotifications", "HideMaterialsNotifications");
        all.Add(
            new(
                Prefix + "Enable_AutoStore_Timer",
                Group,
                "AutoStoreTimer",
                LegacySettingPart.FloatEnabled
            )
        );
        all.Add(
            new(Prefix + "AutoStore_Timer", Group, "AutoStoreTimer", LegacySettingPart.FloatValue)
        );
        AddChance(
            all,
            "AutoShatter_Chance",
            "AutoShatterChance",
            ItemSliderLimits.AutoShatterChance
        );
        AddChance(
            all,
            "AutoShatter_AffixChance",
            "AutoShatterAffixChance",
            ItemSliderLimits.AutoShatterAffixChance
        );
        AddChance(
            all,
            "AutoShatter_QuantityChance",
            "AutoShatterQuantityChance",
            ItemSliderLimits.AutoShatterQuantityChance
        );
        return all.ToArray();
    }

    private static void AddBool(List<LegacyFieldMapping> all, string legacyName, string key)
    {
        all.Add(new(Prefix + legacyName, Group, key, LegacySettingPart.Bool));
    }

    private static void AddChance(
        List<LegacyFieldMapping> all,
        string legacyName,
        string key,
        SliderRange limits
    )
    {
        all.Add(
            new(
                Prefix + legacyName,
                Group,
                key,
                LegacySettingPart.FloatValue,
                limits.Min,
                limits.Max,
                ConfiguredPath
            )
        );
    }
}
