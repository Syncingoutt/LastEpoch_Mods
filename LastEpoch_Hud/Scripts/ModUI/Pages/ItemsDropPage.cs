using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>
/// Items &gt; Drop. This replaces the legacy mixed Items page with one focused card.
/// State resets on HUD re-bind (Build).
/// </summary>
internal static class ItemsDropPage
{
    private static HudFormPage page;

    public static void Build(HudPageId pageId, GameObject parent, GameObject hud, Font font)
    {
        page = HudFormPage.Build(parent, hud, font, pageId);
        if (page == null)
        {
            return;
        }

        var card = page.AddCard("Drop", "Drop");
        AddExclusiveRarity(card, "ForceUnique", "Force Unique", ModSettings.ItemsDrop.ForceUnique);
        AddExclusiveRarity(card, "ForceSet", "Force Set", ModSettings.ItemsDrop.ForceSet);
        // The legacy rarity-9 override produces unrelated unique identities.
        // Do not expose it as a working item-generation feature until a proper
        // native legendary creation path has been implemented.
        page.AddText(
            card,
            "ForceLegendaryUnavailable",
            "Force Legendary is temporarily unavailable: forcing rarity 9 creates invalid unique identities. Use Force Drop for explicit legendary items.",
            76f
        );
        BoolSetting seal = ModSettings.ItemsDrop.ForceSeal;
        page.AddToggle(card, "ForceSeal", "Force Seal", () => seal.Value, seal.Set);

        AddRange(
            card,
            "Implicits",
            "Implicit Value",
            ItemSliderLimits.DropImplicits,
            ModSettings.ItemsDrop.Implicits
        );
        AddRange(
            card,
            "ForgingPotential",
            "Forging Potential",
            ItemSliderLimits.DropForgingPotential,
            ModSettings.ItemsDrop.ForgingPotential
        );
        AddRange(
            card,
            "SealTier",
            "Seal Tier",
            ItemSliderLimits.DropSealTier,
            ModSettings.ItemsDrop.SealTier
        );
        AddRange(
            card,
            "SealValue",
            "Seal Value",
            ItemSliderLimits.DropSealValue,
            ModSettings.ItemsDrop.SealValue
        );
        AddRange(
            card,
            "AffixCount",
            "Affix Count",
            ItemSliderLimits.DropAffixCount,
            ModSettings.ItemsDrop.AffixCount
        );
        AddRange(
            card,
            "IdolAffixCount",
            "Idol Affix Count",
            ItemSliderLimits.DropIdolAffixCount,
            ModSettings.ItemsDrop.IdolAffixCount
        );
        AddRange(
            card,
            "AffixTiers",
            "Affix Tier",
            ItemSliderLimits.DropAffixTiers,
            ModSettings.ItemsDrop.AffixTiers
        );
        AddRange(
            card,
            "AffixValues",
            "Affix Value",
            ItemSliderLimits.DropAffixValues,
            ModSettings.ItemsDrop.AffixValues
        );
        AddRange(
            card,
            "UniqueMods",
            "Unique Mod Value",
            ItemSliderLimits.DropUniqueMods,
            ModSettings.ItemsDrop.UniqueMods
        );
        AddRange(
            card,
            "LegendaryPotential",
            "Legendary Potential",
            ItemSliderLimits.DropLegendaryPotential,
            ModSettings.ItemsDrop.LegendaryPotential
        );
        AddRange(
            card,
            "WeaverWill",
            "Weaver Will",
            ItemSliderLimits.DropWeaverWill,
            ModSettings.ItemsDrop.WeaverWill
        );
    }

    public static void Show() => page?.Show();

    public static void Hide() => page?.Hide();

    public static void Refresh() => page?.RefreshValues();

    private static void AddExclusiveRarity(
        HudFormPage.Card card,
        string id,
        string label,
        BoolSetting setting
    )
    {
        page.AddToggle(card, id, label, () => setting.Value, value => SetExclusive(setting, value));
    }

    private static void SetExclusive(BoolSetting chosen, bool value)
    {
        chosen.Set(value);
        if (!value)
        {
            return;
        }
        foreach (BoolSetting other in ModSettings.ItemsDrop.ForcedRarities)
        {
            if (other != chosen)
            {
                other.Set(false);
            }
        }
        page.RefreshValues();
    }

    private static void AddRange(
        HudFormPage.Card card,
        string id,
        string label,
        SliderRange range,
        RangeSetting setting
    )
    {
        page.AddToggle(card, id, label, () => setting.Enabled, setting.SetEnabled);
        var minimumSlider = page.AddSlider(
            card,
            id + "Minimum",
            "Minimum",
            string.Empty,
            range.Min,
            range.Max,
            range.WholeNumbers,
            () => setting.Min,
            value =>
            {
                setting.SetMin(value);
                page.RefreshValues();
            }
        );
        var maximumSlider = page.AddSlider(
            card,
            id + "Maximum",
            "Maximum",
            string.Empty,
            range.Min,
            range.Max,
            range.WholeNumbers,
            () => setting.Max,
            value =>
            {
                setting.SetMax(value);
                page.RefreshValues();
            }
        );
        page.SetVisibleWhen(minimumSlider, () => setting.Enabled);
        page.SetVisibleWhen(maximumSlider, () => setting.Enabled);
    }
}
