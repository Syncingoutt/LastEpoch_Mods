using System.Collections.Generic;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy modsNotInHud fields mapped to the ModsNotInHud group.</summary>
public static class ModsNotInHudMap
{
    public const string Group = "ModsNotInHud";

    private const string Prefix = "modsNotInHud.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } =
        new[]
        {
            // The legacy field name has this typo.
            new LegacyFieldMapping(
                Prefix + "Enable_PotionResplenishment",
                Group,
                "PotionReplenishment",
                LegacySettingPart.Bool
            ),
            new LegacyFieldMapping(
                Prefix + "Shrines_Unlimited",
                Group,
                "ShrinesUnlimited",
                LegacySettingPart.Bool
            ),
            new LegacyFieldMapping(
                Prefix + "Shrines_Override",
                Group,
                "ShrinesOverride",
                LegacySettingPart.FloatEnabled
            ),
            new LegacyFieldMapping(
                Prefix + "Shrines_Override_id",
                Group,
                "ShrinesOverride",
                LegacySettingPart.FloatValue
            ),
        };
}
