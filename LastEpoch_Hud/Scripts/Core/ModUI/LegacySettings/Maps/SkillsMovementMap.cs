using System.Collections.Generic;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Skills.MovementSkills toggles mapped to the SkillsMovement group.</summary>
public static class SkillsMovementMap
{
    public const string Group = "SkillsMovement";

    private const string Prefix = "Skills.MovementSkills.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } =
        new LegacyFieldMapping[]
        {
            new(Prefix + "Enable_NoTarget", Group, "NoTarget", LegacySettingPart.Bool),
            new(
                Prefix + "Enable_ImmuneDuringMovement",
                Group,
                "ImmuneDuringMovement",
                LegacySettingPart.Bool
            ),
            new(Prefix + "Disable_SimplePath", Group, "IgnoreSimplePath", LegacySettingPart.Bool),
        };
}
