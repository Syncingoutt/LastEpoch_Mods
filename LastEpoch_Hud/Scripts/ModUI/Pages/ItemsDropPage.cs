using System;
using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;
using UnityEngine.UI;

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
            return;

        var card = page.AddCard("Drop", "Drop");
        AddExclusiveRarity(card, "ForceUnique", "Force Unique", 0);
        AddExclusiveRarity(card, "ForceSet", "Force Set", 1);
        AddExclusiveRarity(card, "ForceLegendary", "Force Legendary", 2);
        AddToggle(
            card,
            "ForceSeal",
            "Force Seal",
            () => Save_Manager.instance.data.Items.Drop.Enable_ForceSeal,
            value => Save_Manager.instance.data.Items.Drop.Enable_ForceSeal = value
        );

        AddRange(
            card,
            "Implicits",
            "Implicit Value",
            Hud_Manager.Content.Items.Drop.implicits_slider_min,
            Hud_Manager.Content.Items.Drop.implicits_slider_max,
            () => Save_Manager.instance.data.Items.Drop.Enable_Implicits,
            value => Save_Manager.instance.data.Items.Drop.Enable_Implicits = value,
            () => Save_Manager.instance.data.Items.Drop.Implicits_Min,
            value => Save_Manager.instance.data.Items.Drop.Implicits_Min = value,
            () => Save_Manager.instance.data.Items.Drop.Implicits_Max,
            value => Save_Manager.instance.data.Items.Drop.Implicits_Max = value
        );
        AddRange(
            card,
            "ForgingPotential",
            "Forging Potential",
            Hud_Manager.Content.Items.Drop.forgin_potencial_slider_min,
            Hud_Manager.Content.Items.Drop.forgin_potencial_slider_max,
            () => Save_Manager.instance.data.Items.Drop.Enable_ForginPotencial,
            value => Save_Manager.instance.data.Items.Drop.Enable_ForginPotencial = value,
            () => Save_Manager.instance.data.Items.Drop.ForginPotencial_Min,
            value => Save_Manager.instance.data.Items.Drop.ForginPotencial_Min = value,
            () => Save_Manager.instance.data.Items.Drop.ForginPotencial_Max,
            value => Save_Manager.instance.data.Items.Drop.ForginPotencial_Max = value
        );
        AddRange(
            card,
            "SealTier",
            "Seal Tier",
            Hud_Manager.Content.Items.Drop.seal_tier_slider_min,
            Hud_Manager.Content.Items.Drop.seal_tier_slider_max,
            () => Save_Manager.instance.data.Items.Drop.Enable_SealTier,
            v => Save_Manager.instance.data.Items.Drop.Enable_SealTier = v,
            () => Save_Manager.instance.data.Items.Drop.SealTier_Min,
            v => Save_Manager.instance.data.Items.Drop.SealTier_Min = v,
            () => Save_Manager.instance.data.Items.Drop.SealTier_Max,
            v => Save_Manager.instance.data.Items.Drop.SealTier_Max = v
        );
        AddRange(
            card,
            "SealValue",
            "Seal Value",
            Hud_Manager.Content.Items.Drop.seal_value_slider_min,
            Hud_Manager.Content.Items.Drop.seal_value_slider_max,
            () => Save_Manager.instance.data.Items.Drop.Enable_SealValue,
            v => Save_Manager.instance.data.Items.Drop.Enable_SealValue = v,
            () => Save_Manager.instance.data.Items.Drop.SealValue_Min,
            v => Save_Manager.instance.data.Items.Drop.SealValue_Min = v,
            () => Save_Manager.instance.data.Items.Drop.SealValue_Max,
            v => Save_Manager.instance.data.Items.Drop.SealValue_Max = v
        );
        AddRange(
            card,
            "AffixCount",
            "Affix Count",
            Hud_Manager.Content.Items.Drop.affix_count_slider_min,
            Hud_Manager.Content.Items.Drop.affix_count_slider_max,
            () => Save_Manager.instance.data.Items.Drop.Enable_AffixCount,
            v => Save_Manager.instance.data.Items.Drop.Enable_AffixCount = v,
            () => Save_Manager.instance.data.Items.Drop.AffixCount_Min,
            v => Save_Manager.instance.data.Items.Drop.AffixCount_Min = v,
            () => Save_Manager.instance.data.Items.Drop.AffixCount_Max,
            v => Save_Manager.instance.data.Items.Drop.AffixCount_Max = v
        );
        AddRange(
            card,
            "IdolAffixCount",
            "Idol Affix Count",
            null,
            null,
            () => Save_Manager.instance.data.Items.Drop.Enable_IdolAffixCount,
            v => Save_Manager.instance.data.Items.Drop.Enable_IdolAffixCount = v,
            () => Save_Manager.instance.data.Items.Drop.IdolAffixCount_Min,
            v => Save_Manager.instance.data.Items.Drop.IdolAffixCount_Min = v,
            () => Save_Manager.instance.data.Items.Drop.IdolAffixCount_Max,
            v => Save_Manager.instance.data.Items.Drop.IdolAffixCount_Max = v,
            0f,
            2f
        );
        AddRange(
            card,
            "AffixTiers",
            "Affix Tier",
            Hud_Manager.Content.Items.Drop.affix_tiers_slider_min,
            Hud_Manager.Content.Items.Drop.affix_tiers_slider_max,
            () => Save_Manager.instance.data.Items.Drop.Enable_AffixTiers,
            v => Save_Manager.instance.data.Items.Drop.Enable_AffixTiers = v,
            () => Save_Manager.instance.data.Items.Drop.AffixTiers_Min,
            v => Save_Manager.instance.data.Items.Drop.AffixTiers_Min = v,
            () => Save_Manager.instance.data.Items.Drop.AffixTiers_Max,
            v => Save_Manager.instance.data.Items.Drop.AffixTiers_Max = v
        );
        AddRange(
            card,
            "AffixValues",
            "Affix Value",
            Hud_Manager.Content.Items.Drop.affix_values_slider_min,
            Hud_Manager.Content.Items.Drop.affix_values_slider_max,
            () => Save_Manager.instance.data.Items.Drop.Enable_AffixValues,
            v => Save_Manager.instance.data.Items.Drop.Enable_AffixValues = v,
            () => Save_Manager.instance.data.Items.Drop.AffixValues_Min,
            v => Save_Manager.instance.data.Items.Drop.AffixValues_Min = v,
            () => Save_Manager.instance.data.Items.Drop.AffixValues_Max,
            v => Save_Manager.instance.data.Items.Drop.AffixValues_Max = v
        );
        AddRange(
            card,
            "UniqueMods",
            "Unique Mod Value",
            Hud_Manager.Content.Items.Drop.unique_mods_slider_min,
            Hud_Manager.Content.Items.Drop.unique_mods_slider_max,
            () => Save_Manager.instance.data.Items.Drop.Enable_UniqueMods,
            v => Save_Manager.instance.data.Items.Drop.Enable_UniqueMods = v,
            () => Save_Manager.instance.data.Items.Drop.UniqueMods_Min,
            v => Save_Manager.instance.data.Items.Drop.UniqueMods_Min = v,
            () => Save_Manager.instance.data.Items.Drop.UniqueMods_Max,
            v => Save_Manager.instance.data.Items.Drop.UniqueMods_Max = v
        );
        AddRange(
            card,
            "LegendaryPotential",
            "Legendary Potential",
            Hud_Manager.Content.Items.Drop.legendary_potencial_slider_min,
            Hud_Manager.Content.Items.Drop.legendary_potencial_slider_max,
            () => Save_Manager.instance.data.Items.Drop.Enable_LegendaryPotencial,
            v => Save_Manager.instance.data.Items.Drop.Enable_LegendaryPotencial = v,
            () => Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Min,
            v => Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Min = v,
            () => Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Max,
            v => Save_Manager.instance.data.Items.Drop.LegendaryPotencial_Max = v
        );
        AddRange(
            card,
            "WeaverWill",
            "Weaver Will",
            Hud_Manager.Content.Items.Drop.weaver_will_slider_min,
            Hud_Manager.Content.Items.Drop.weaver_will_slider_max,
            () => Save_Manager.instance.data.Items.Drop.Enable_WeaverWill,
            v => Save_Manager.instance.data.Items.Drop.Enable_WeaverWill = v,
            () => Save_Manager.instance.data.Items.Drop.WeaverWill_Min,
            v => Save_Manager.instance.data.Items.Drop.WeaverWill_Min = v,
            () => Save_Manager.instance.data.Items.Drop.WeaverWill_Max,
            v => Save_Manager.instance.data.Items.Drop.WeaverWill_Max = v
        );
    }

    public static void Show() => page?.Show();

    public static void Hide() => page?.Hide();

    public static void Refresh() => page?.RefreshValues();

    private static void AddExclusiveRarity(
        HudFormPage.Card card,
        string id,
        string label,
        int rarity
    )
    {
        AddToggle(
            card,
            id,
            label,
            () =>
                rarity == 0 ? Save_Manager.instance.data.Items.Drop.Enable_ForceUnique
                : rarity == 1 ? Save_Manager.instance.data.Items.Drop.Enable_ForceSet
                : Save_Manager.instance.data.Items.Drop.Enable_ForceLegendary,
            value =>
            {
                if (rarity == 0)
                    Save_Manager.instance.data.Items.Drop.Enable_ForceUnique = value;
                else if (rarity == 1)
                    Save_Manager.instance.data.Items.Drop.Enable_ForceSet = value;
                else
                    Save_Manager.instance.data.Items.Drop.Enable_ForceLegendary = value;
                if (value)
                {
                    if (rarity != 0)
                        Save_Manager.instance.data.Items.Drop.Enable_ForceUnique = false;
                    if (rarity != 1)
                        Save_Manager.instance.data.Items.Drop.Enable_ForceSet = false;
                    if (rarity != 2)
                        Save_Manager.instance.data.Items.Drop.Enable_ForceLegendary = false;
                    page.RefreshValues();
                }
            }
        );
    }

    private static void AddToggle(
        HudFormPage.Card card,
        string id,
        string label,
        Func<bool> read,
        Action<bool> write
    )
    {
        page.AddToggle(
            card,
            id,
            label,
            () => HasSave() && read(),
            value =>
            {
                if (HasSave())
                    write(value);
            }
        );
    }

    private static void AddRange(
        HudFormPage.Card card,
        string id,
        string label,
        Slider minimumSource,
        Slider maximumSource,
        Func<bool> readEnabled,
        Action<bool> writeEnabled,
        Func<float> readMinimum,
        Action<float> writeMinimum,
        Func<float> readMaximum,
        Action<float> writeMaximum,
        float fallbackMinimum = 0f,
        float fallbackMaximum = 255f
    )
    {
        AddToggle(card, id, label, readEnabled, writeEnabled);
        float minimum = minimumSource.IsNullOrDestroyed()
            ? fallbackMinimum
            : minimumSource.minValue;
        float maximum = maximumSource.IsNullOrDestroyed()
            ? fallbackMaximum
            : maximumSource.maxValue;
        bool whole = minimumSource.IsNullOrDestroyed() || minimumSource.wholeNumbers;
        var minimumSlider = page.AddSlider(
            card,
            id + "Minimum",
            "Minimum",
            string.Empty,
            minimum,
            maximum,
            whole,
            () => HasSave() ? readMinimum() : minimum,
            value =>
            {
                if (!HasSave())
                    return;
                writeMinimum(value);
                if (value > readMaximum())
                    writeMaximum(value);
                page.RefreshValues();
            }
        );
        var maximumSlider = page.AddSlider(
            card,
            id + "Maximum",
            "Maximum",
            string.Empty,
            minimum,
            maximum,
            whole,
            () => HasSave() ? readMaximum() : maximum,
            value =>
            {
                if (!HasSave())
                    return;
                writeMaximum(value);
                if (value < readMinimum())
                    writeMinimum(value);
                page.RefreshValues();
            }
        );
        page.SetVisibleWhen(minimumSlider, () => HasSave() && readEnabled());
        page.SetVisibleWhen(maximumSlider, () => HasSave() && readEnabled());
    }

    private static bool HasSave() =>
        !Save_Manager.instance.IsNullOrDestroyed() && Save_Manager.instance.initialized;
}
