using System.Collections.Generic;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy DamageMeter fields mapped to the DamageMeter group.</summary>
public static class DamageMeterMap
{
    public const string Group = "DamageMeter";

    private const string Prefix = "DamageMeter.";

    // Percent, Flat, Damage per Second (DamageMeter.DamageType_Dps)
    private const float LastDamageType = 2f;

    public static IReadOnlyList<LegacyFieldMapping> All { get; } =
        new[]
        {
            new LegacyFieldMapping(
                Prefix + "DamageType",
                Group,
                "DamageType",
                LegacySettingPart.Dropdown,
                0f,
                LastDamageType
            ),
            new LegacyFieldMapping(
                Prefix + "SeparateHitAndDot",
                Group,
                "SeparateHitAndDot",
                LegacySettingPart.Bool
            ),
        };
}
