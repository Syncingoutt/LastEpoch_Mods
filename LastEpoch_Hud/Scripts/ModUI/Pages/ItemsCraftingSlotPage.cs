using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>
/// Items &gt; Crafting Slot. Every override has an explicit checkbox; slider zero is
/// therefore a valid value and never doubles as an enabled/disabled sentinel.
/// State resets on HUD re-bind (Build).
/// </summary>
internal static class ItemsCraftingSlotPage
{
    private static HudFormPage page;

    public static void Build(HudPageId pageId, GameObject parent, GameObject hud, Font font)
    {
        page = HudFormPage.Build(parent, hud, font, pageId);
        if (page == null)
            return;

        var card = page.AddCard("CraftingSlot", "Crafting Slot");
        AddBool(card, "Enable", "Enable Crafting Slot", ModSettings.ItemsCraftingSlot.EnableMod);
        AddBool(
            card,
            "InfiniteForgingPotential",
            "Infinite Forging Potential",
            ModSettings.InfiniteForgingPotential.Enabled
        );
        page.AddButton(card, "DeselectAll", "Deselect All", DeselectAll);

        var advancedForgeCard = page.AddCard("AdvancedForge", "Advanced Forge");
        AddBool(
            advancedForgeCard,
            "AdvancedForgeT7",
            "Craft Affixes to T7",
            ModSettings.AdvancedForge.AllowT7Crafting
        );
        page.AddToggle(
            advancedForgeCard,
            "AdvancedForgeMaxRoll",
            "Max Crafted Roll",
            () => ModSettings.AdvancedForge.AffixRoll.Enabled,
            v =>
            {
                ModSettings.AdvancedForge.AffixRoll.SetEnabled(v);
                if (v)
                    ModSettings.AdvancedForge.AffixRoll.SetValue(255f);
            }
        );
        AddBool(
            advancedForgeCard,
            "AdvancedForgeHope",
            "Guarantee Hope",
            ModSettings.AdvancedForge.GuaranteedGlyphOfHope
        );
        AddBool(
            advancedForgeCard,
            "AdvancedForgeDespair",
            "Guarantee Despair",
            ModSettings.AdvancedForge.GuaranteedGlyphOfDespair
        );

        AddOverride(
            card,
            "ForgingPotential",
            "Forging Potential",
            ItemSliderLimits.CraftingSlotForgingPotential,
            ModSettings.ItemsCraftingSlot.ForgingPotential
        );
        for (int i = 0; i < ModSettings.ItemsCraftingSlot.Implicits.Count; i++)
        {
            AddImplicit(card, i);
        }
        AddOverride(
            card,
            "SealTier",
            "Seal Tier",
            ItemSliderLimits.CraftingSlotSealTier,
            ModSettings.ItemsCraftingSlot.SealTier
        );
        AddOverride(
            card,
            "SealValue",
            "Seal Value",
            ItemSliderLimits.CraftingSlotSealValue,
            ModSettings.ItemsCraftingSlot.SealValue
        );

        for (int i = 0; i < ModSettings.ItemsCraftingSlot.AffixTiers.Count; i++)
        {
            AddAffix(card, i);
        }
        for (int i = 0; i < ModSettings.ItemsCraftingSlot.UniqueMods.Count; i++)
        {
            AddUnique(card, i);
        }

        AddOverride(
            card,
            "LegendaryPotential",
            "Legendary Potential",
            ItemSliderLimits.CraftingSlotLegendaryPotential,
            ModSettings.ItemsCraftingSlot.LegendaryPotential
        );
        AddOverride(
            card,
            "WeaverWill",
            "Weaver Will",
            ItemSliderLimits.CraftingSlotWeaverWill,
            ModSettings.ItemsCraftingSlot.WeaverWill
        );
    }

    public static void Show() => page?.Show();

    public static void Hide() => page?.Hide();

    public static void Refresh() => page?.RefreshValues();

    private static void AddBool(HudFormPage.Card card, string id, string label, BoolSetting setting)
    {
        page.AddToggle(card, id, label, () => setting.Value, v => setting.Set(v));
    }

    private static void AddImplicit(HudFormPage.Card card, int index)
    {
        AddOverride(
            card,
            "Implicit" + index,
            "Implicit " + (index + 1),
            ItemSliderLimits.CraftingSlotImplicit,
            ModSettings.ItemsCraftingSlot.Implicits[index]
        );
    }

    private static void AddAffix(HudFormPage.Card card, int index)
    {
        AddOverride(
            card,
            "Affix" + index + "Tier",
            "Affix " + (index + 1) + " Tier",
            ItemSliderLimits.CraftingSlotAffixTier,
            ModSettings.ItemsCraftingSlot.AffixTiers[index]
        );
        AddOverride(
            card,
            "Affix" + index + "Value",
            "Affix " + (index + 1) + " Value",
            ItemSliderLimits.CraftingSlotAffixValue,
            ModSettings.ItemsCraftingSlot.AffixValues[index]
        );
    }

    private static void AddUnique(HudFormPage.Card card, int index)
    {
        AddOverride(
            card,
            "UniqueMod" + index,
            "Unique Mod " + (index + 1),
            ItemSliderLimits.CraftingSlotUniqueMod,
            ModSettings.ItemsCraftingSlot.UniqueMods[index]
        );
    }

    private static void AddOverride(
        HudFormPage.Card card,
        string id,
        string label,
        SliderRange range,
        FloatSetting setting
    )
    {
        page.AddToggleSlider(
            card,
            id,
            label,
            string.Empty,
            range.Min,
            range.Max,
            range.WholeNumbers,
            () => setting.Enabled,
            v => setting.SetEnabled(v),
            () => setting.Value,
            v => setting.SetValue(v)
        );
    }

    private static void DeselectAll()
    {
        foreach (FloatSetting setting in ModSettings.ItemsCraftingSlot.Overrides)
        {
            setting.SetEnabled(false);
        }
        page.RefreshValues();
    }
}
