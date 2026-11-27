using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>
/// Bindings for Utilities &gt; Buffs. Values are shown in the same percentage
/// units as the legacy labels, derived from the stored limits.
/// State resets on HUD re-bind (Build).
/// </summary>
internal static class UtilitiesBuffsPage
{
    private static HudSliderCard page;

    public static void Build(HudPageId pageId, GameObject parent, GameObject hud, Font font)
    {
        page = HudSliderCard.Build(
            parent,
            hud,
            font,
            pageId,
            "Buffs",
            new List<HudSliderCard.Definition>
            {
                Buff(
                    "MoveSpeed",
                    "Move Speed",
                    ModSettings.PermanentBuffs.MoveSpeed,
                    BuffPercentScale.Fraction
                ),
                Buff(
                    "Damage",
                    "Damage",
                    ModSettings.PermanentBuffs.Damage,
                    BuffPercentScale.Fraction
                ),
                Buff(
                    "AreaOfEffect",
                    "Area of Effect",
                    ModSettings.PermanentBuffs.AreaOfEffect,
                    BuffPercentScale.Fraction
                ),
                Buff(
                    "AttackSpeed",
                    "Attack Speed",
                    ModSettings.PermanentBuffs.AttackSpeed,
                    BuffPercentScale.Fraction
                ),
                Buff(
                    "CastingSpeed",
                    "Casting Speed",
                    ModSettings.PermanentBuffs.CastSpeed,
                    BuffPercentScale.Fraction
                ),
                Buff(
                    "CriticalChance",
                    "Critical Chance",
                    ModSettings.PermanentBuffs.CriticalChance,
                    BuffPercentScale.CriticalChance
                ),
                Buff(
                    "CriticalMultiplier",
                    "Critical Multiplier",
                    ModSettings.PermanentBuffs.CriticalMultiplier,
                    BuffPercentScale.Fraction
                ),
                Buff(
                    "HealthRegen",
                    "Health Regeneration",
                    ModSettings.PermanentBuffs.HealthRegen,
                    BuffPercentScale.Fraction
                ),
                Buff(
                    "ManaRegen",
                    "Mana Regeneration",
                    ModSettings.PermanentBuffs.ManaRegen,
                    BuffPercentScale.Fraction
                ),
                Buff(
                    "Strength",
                    "Strength",
                    ModSettings.PermanentBuffs.Strength,
                    BuffPercentScale.Direct
                ),
                Buff(
                    "Intelligence",
                    "Intelligence",
                    ModSettings.PermanentBuffs.Intelligence,
                    BuffPercentScale.Direct
                ),
                Buff(
                    "Dexterity",
                    "Dexterity",
                    ModSettings.PermanentBuffs.Dexterity,
                    BuffPercentScale.Direct
                ),
                Buff(
                    "Vitality",
                    "Vitality",
                    ModSettings.PermanentBuffs.Vitality,
                    BuffPercentScale.Direct
                ),
                Buff(
                    "Attunement",
                    "Attunement",
                    ModSettings.PermanentBuffs.Attunement,
                    BuffPercentScale.Direct
                ),
            }
        );
    }

    public static void Show()
    {
        UpdateMasterEnable();
        page?.Show();
    }

    public static void Hide() => page?.Hide();

    private static HudSliderCard.Definition Buff(
        string id,
        string label,
        FloatSetting setting,
        BuffPercentScale scale
    )
    {
        return new HudSliderCard.Definition
        {
            Id = id,
            Label = label,
            Unit = "%",
            Minimum = BuffPercent.ToPercent(scale, setting.MinLimit),
            Maximum = BuffPercent.ToPercent(scale, setting.MaxLimit),
            WholeNumbers = true,
            Read = () => BuffPercent.ToPercent(scale, setting.Value),
            Write = value =>
            {
                setting.SetValue(
                    BuffPercent.ToStored(scale, value, setting.MinLimit, setting.MaxLimit)
                );
                UpdateMasterEnable();
            },
            ReadEnabled = () => ModSettings.PermanentBuffs.EnableMod.Value && setting.Enabled,
            WriteEnabled = isEnabled =>
            {
                setting.SetEnabled(isEnabled);
                UpdateMasterEnable();
            },
        };
    }

    private static void UpdateMasterEnable()
    {
        ModSettings.PermanentBuffs.EnableMod.Set(AnyBuffEnabled());
    }

    private static bool AnyBuffEnabled()
    {
        foreach (FloatSetting buff in ModSettings.PermanentBuffs.Buffs)
        {
            if (buff.Enabled)
            {
                return true;
            }
        }
        return false;
    }
}
