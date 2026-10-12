using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>
/// Utilities &gt; QOL: small cards for pickup, auto-sell, shatter and item requirements.
/// All cards bind to ModSettings.
/// State resets on HUD re-bind (Build).
/// </summary>
internal static class UtilitiesQolPage
{
    private static HudFormPage page;

    public static void Build(HudPageId pageId, GameObject parent, GameObject hud, Font font)
    {
        page = HudFormPage.Build(parent, hud, font, pageId);
        if (page == null)
            return;

        BuildAutoPickup();
        BuildAutoStore();
        BuildAutoShatter();
        BuildAutoSell();
        BuildMisc();
        BuildRequirements();
    }

    public static void Show() => page?.Show();

    public static void Hide() => page?.Hide();

    public static void Refresh() => page?.RefreshValues();

    private static void BuildAutoPickup()
    {
        var card = page.AddCard("AutoPickup", "Auto-Pickup");
        page.AddToggle(
            card,
            "Gold",
            "Gold",
            () => ModSettings.ItemsPickup.AutoPickupGold.Value,
            ModSettings.ItemsPickup.AutoPickupGold.Set
        );
        page.AddToggle(
            card,
            "Keys",
            "Keys",
            () => ModSettings.ItemsPickup.AutoPickupKeys.Value,
            ModSettings.ItemsPickup.AutoPickupKeys.Set
        );
        page.AddToggle(
            card,
            "Potions",
            "Potions",
            () => ModSettings.ItemsPickup.AutoPickupPotions.Value,
            ModSettings.ItemsPickup.AutoPickupPotions.Set
        );
        page.AddToggle(
            card,
            "ExperienceTomes",
            "Experience Tomes",
            () => ModSettings.ItemsPickup.AutoPickupXpTome.Value,
            ModSettings.ItemsPickup.AutoPickupXpTome.Set
        );
        page.AddToggle(
            card,
            "FavorTomes",
            "Favor Tomes",
            () => ModSettings.ItemsPickup.AutoPickupFavorTome.Value,
            ModSettings.ItemsPickup.AutoPickupFavorTome.Set
        );
        page.AddToggle(
            card,
            "MemoryAmber",
            "Memory Amber",
            () => ModSettings.ItemsPickup.AutoPickupMemoryAmber.Value,
            ModSettings.ItemsPickup.AutoPickupMemoryAmber.Set
        );
        page.AddToggle(
            card,
            "Materials",
            "Materials",
            () => ModSettings.ItemsPickup.AutoPickupMaterials.Value,
            ModSettings.ItemsPickup.AutoPickupMaterials.Set
        );
        page.AddToggle(
            card,
            "WovenEchoes",
            "Woven Echoes",
            () => ModSettings.ItemsPickup.AutoPickupWovenEchoes.Value,
            ModSettings.ItemsPickup.AutoPickupWovenEchoes.Set
        );
        page.AddToggle(
            card,
            "Filter",
            "Items In Filter",
            () => ModSettings.ItemsPickup.AutoPickupFromFilter.Value,
            ModSettings.ItemsPickup.AutoPickupFromFilter.Set
        );
    }

    private static void BuildAutoStore()
    {
        var card = page.AddCard("AutoStore", "Auto-Store");
        page.AddToggle(
            card,
            "OnDrop",
            "On Drop",
            () => ModSettings.ItemsPickup.AutoStoreOnDrop.Value,
            ModSettings.ItemsPickup.AutoStoreOnDrop.Set
        );
        page.AddToggle(
            card,
            "OnInventoryOpen",
            "On Inventory Open",
            () => ModSettings.ItemsPickup.AutoStoreOnInventoryOpen.Value,
            ModSettings.ItemsPickup.AutoStoreOnInventoryOpen.Set
        );
        page.AddToggle(
            card,
            "EveryTenSeconds",
            "Every 10s",
            () => ModSettings.ItemsPickup.AutoStoreTimer.Enabled,
            SetStoreEveryTenSeconds
        );
        page.AddToggle(
            card,
            "OnFullInventory",
            "On Full Inventory",
            () => ModSettings.AutoStore.OnFullInventory.Value,
            ModSettings.AutoStore.OnFullInventory.Set
        );
        page.AddText(
            card,
            "OnFullInventoryNote",
            "Stores materials in your stash when a pickup doesn't fit, then picks the item up.",
            58f
        );
    }

    private static void BuildAutoShatter()
    {
        var card = page.AddCard("AutoShatter", "Auto-Shatter");
        page.AddToggle(
            card,
            "NotInFilter",
            "Items Not In Filter",
            () => ModSettings.ItemsPickup.AutoShatterFromFilter.Value,
            ModSettings.ItemsPickup.AutoShatterFromFilter.Set
        );
        page.AddToggle(
            card,
            "UseRune",
            "Use Rune of Shattering",
            () => ModSettings.ItemsPickup.AutoShatterUseRune.Value,
            ModSettings.ItemsPickup.AutoShatterUseRune.Set
        );
        AddPercentSlider(
            card,
            "Chance",
            "Shatter Chance",
            ItemSliderLimits.AutoShatterChance,
            ModSettings.ItemsPickup.AutoShatterChance
        );
        AddPercentSlider(
            card,
            "AffixChance",
            "Affix Obtained From Shatter Chance",
            ItemSliderLimits.AutoShatterAffixChance,
            ModSettings.ItemsPickup.AutoShatterAffixChance
        );
        AddPercentSlider(
            card,
            "QuantityChance",
            "Affix Quantity Chance",
            ItemSliderLimits.AutoShatterQuantityChance,
            ModSettings.ItemsPickup.AutoShatterQuantityChance
        );
    }

    private static void BuildAutoSell()
    {
        var card = page.AddCard("AutoSell", "Auto-Sell");
        page.AddToggle(
            card,
            "NotInFilter",
            "Items Not In Filter",
            () => ModSettings.ItemsPickup.AutoSellFromFilter.Value,
            ModSettings.ItemsPickup.AutoSellFromFilter.Set
        );
    }

    private static void BuildMisc()
    {
        var card = page.AddCard("Misc", "Misc");
        page.AddToggle(
            card,
            "RangePickup",
            "Range Pickup",
            () => ModSettings.ItemsPickup.RangePickup.Value,
            ModSettings.ItemsPickup.RangePickup.Set
        );
        page.AddToggle(
            card,
            "HideNotifications",
            "Hide Material Notifications",
            () => ModSettings.ItemsPickup.HideMaterialsNotifications.Value,
            ModSettings.ItemsPickup.HideMaterialsNotifications.Set
        );
    }

    private static void BuildRequirements()
    {
        var card = page.AddCard("Requirements", "Requirements");
        page.AddToggle(
            card,
            "Class",
            "Class",
            () => ModSettings.ItemsRequirements.Class.Value,
            ModSettings.ItemsRequirements.Class.Set
        );
        page.AddToggle(
            card,
            "Level",
            "Level",
            () => ModSettings.ItemsRequirements.Level.Value,
            ModSettings.ItemsRequirements.Level.Set
        );
        page.AddToggle(
            card,
            "Set",
            "Set",
            () => ModSettings.ItemsRequirements.Set.Value,
            ModSettings.ItemsRequirements.Set.Set
        );
    }

    private static void SetStoreEveryTenSeconds(bool enabled)
    {
        ModSettings.ItemsPickup.AutoStoreTimer.SetEnabled(enabled);
        ModSettings.ItemsPickup.AutoStoreTimer.SetValue(
            ModSettings.ItemsPickup.AutoStoreTimerSeconds
        );
    }

    private static void AddPercentSlider(
        HudFormPage.Card card,
        string id,
        string label,
        SliderRange range,
        FloatSetting setting
    )
    {
        page.AddSlider(
            card,
            id,
            label,
            "%",
            range.Min,
            range.Max,
            range.WholeNumbers,
            () => setting.Value,
            setting.SetValue
        );
    }
}
