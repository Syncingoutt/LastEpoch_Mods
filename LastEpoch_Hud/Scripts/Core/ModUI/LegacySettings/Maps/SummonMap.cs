using System.Collections.Generic;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Summon fields mapped to the Summon group.</summary>
public static class SummonMap
{
    public const string Group = "Summon";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } =
        new[]
        {
            Bool("Summon.Enable_GodMode", "GodMode"),
            Bool("Summon.Enable_Forever", "Forever"),
            Bool("Summon.Enable_DontCollide", "DontCollide"),
        };

    private static LegacyFieldMapping Bool(string legacyPath, string key)
    {
        return new LegacyFieldMapping(legacyPath, Group, key, LegacySettingPart.Bool);
    }
}
