using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>
/// Save bindings for Utilities &gt; Multipliers. The shared HudSliderCard owns all
/// visuals and interaction; these definitions preserve the original raw x values.
/// State resets on HUD re-bind (Build).
/// </summary>
internal static class UtilitiesMultipliersPage
{
    private static HudSliderCard page;

    public static void Build(HudPageId pageId, GameObject parent, GameObject hud, Font font)
    {
        page = HudSliderCard.Build(
            parent,
            hud,
            font,
            pageId,
            "Multipliers",
            new List<HudSliderCard.Definition>
            {
                Multiplier(
                    "Density",
                    "Density Multiplier",
                    ModSettings.CharacterMultipliers.Density
                ),
                Multiplier(
                    "Experience",
                    "Experience Multiplier",
                    ModSettings.CharacterMultipliers.Experience
                ),
                Multiplier(
                    "AbilityExperience",
                    "Ability Experience Multiplier",
                    ModSettings.CharacterMultipliers.Ability
                ),
                Multiplier("Favor", "Favor Multiplier", ModSettings.CharacterMultipliers.Favor),
                Multiplier(
                    "MemoryAmber",
                    "Memory Amber Multiplier",
                    ModSettings.CharacterMultipliers.MemoryAmber
                ),
                Multiplier(
                    "ItemDropQuantity",
                    "Item Drop Multiplier",
                    ModSettings.CharacterMultipliers.ItemDropMultiplier
                ),
                Multiplier(
                    "ItemDropChance",
                    "Item Drop Chance",
                    ModSettings.CharacterMultipliers.ItemDropChance
                ),
                Multiplier(
                    "GoldDropQuantity",
                    "Gold Drop Multiplier",
                    ModSettings.CharacterMultipliers.GoldDropMultiplier
                ),
                Multiplier(
                    "GoldDropChance",
                    "Gold Drop Chance",
                    ModSettings.CharacterMultipliers.GoldDropChance
                ),
            }
        );
    }

    public static void Show() => page?.Show();

    public static void Hide() => page?.Hide();

    private static HudSliderCard.Definition Multiplier(
        string id,
        string label,
        FloatSetting setting
    )
    {
        return new HudSliderCard.Definition
        {
            Id = id,
            Label = label,
            Unit = "x",
            Minimum = setting.MinLimit,
            Maximum = setting.MaxLimit,
            WholeNumbers = CharacterSliderLimits.Multiplier.WholeNumbers,
            Read = () => setting.Value,
            Write = setting.SetValue,
            ReadEnabled = () => setting.Enabled,
            WriteEnabled = setting.SetEnabled,
        };
    }
}
