using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Character.Cheats multipliers mapped to the CharacterMultipliers group.</summary>
public static class CharacterMultipliersMap
{
    public const string Group = "CharacterMultipliers";

    private const string Prefix = "Character.Cheats.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } = Build();

    private static LegacyFieldMapping[] Build()
    {
        List<LegacyFieldMapping> all = new();
        AddMultiplier(all, "DensityMultiplier", "Density");
        AddMultiplier(all, "ExperienceMultiplier", "Experience");
        AddMultiplier(all, "AbilityMultiplier", "Ability");
        AddMultiplier(all, "FavorMultiplier", "Favor");
        AddMultiplier(all, "ItemDropChance", "ItemDropChance");
        AddMultiplier(all, "ItemDropMultiplier", "ItemDropMultiplier");
        AddMultiplier(all, "GoldDropChance", "GoldDropChance");
        AddMultiplier(all, "GoldDropMultiplier", "GoldDropMultiplier");
        AddMultiplier(all, "MemoryAmberMultiplier", "MemoryAmber");
        return all.ToArray();
    }

    private static void AddMultiplier(List<LegacyFieldMapping> all, string legacyName, string key)
    {
        all.Add(
            new LegacyFieldMapping(
                Prefix + "Enable_" + legacyName,
                Group,
                key,
                LegacySettingPart.FloatEnabled
            )
        );
        all.Add(
            new LegacyFieldMapping(
                Prefix + legacyName,
                Group,
                key,
                LegacySettingPart.FloatValue,
                CharacterSliderLimits.Multiplier.Min,
                CharacterSliderLimits.Multiplier.Max
            )
        );
    }
}
