using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>Skills &gt; Companions HUD page. State resets on HUD re-bind (Build).</summary>
internal static class SkillsCompanionsPage
{
    private static HudFormPage page;

    public static void Build(HudPageId pageId, GameObject parent, GameObject hud, Font font)
    {
        page = HudFormPage.Build(parent, hud, font, pageId);
        if (page == null)
            return;
        var general = page.AddCard("General", "General");
        AddSlider(
            general,
            "MaximumCompanions",
            "Maximum Companions",
            SkillSliderLimits.CompanionsMaximum,
            ModSettings.SkillsCompanions.MaximumCompanions
        );

        var wolves = page.AddCard("Wolves", "Wolves");
        page.AddToggle(
            wolves,
            "SummonMaximum",
            "Summon To Maximum",
            () => ModSettings.SkillsCompanions.WolfSummonMaximum.Value,
            ModSettings.SkillsCompanions.WolfSummonMaximum.Set
        );
        AddSlider(
            wolves,
            "SummonLimit",
            "Summon Limit",
            SkillSliderLimits.WolfSummonLimit,
            ModSettings.SkillsCompanions.WolfSummonLimit
        );
        page.AddToggle(
            wolves,
            "StunImmunity",
            "Stun Immunity",
            () => ModSettings.SkillsCompanions.WolfStunImmunity.Value,
            ModSettings.SkillsCompanions.WolfStunImmunity.Set
        );

        var scorpions = page.AddCard("Scorpions", "Scorpions");
        AddSlider(
            scorpions,
            "BabyQuantity",
            "Baby Scorpion Quantity",
            SkillSliderLimits.ScorpionBabyQuantity,
            ModSettings.SkillsCompanions.ScorpionBabyQuantity
        );
    }

    public static void Show() => page?.Show();

    public static void Hide() => page?.Hide();

    public static void Refresh() => page?.RefreshValues();

    private static void AddSlider(
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
            setting.SetEnabled,
            () => setting.Value,
            setting.SetValue
        );
    }
}
