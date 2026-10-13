using System.Collections.Generic;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Items.Req fields mapped to the ItemsRequirements group.</summary>
public static class ItemsRequirementsMap
{
    public const string Group = "ItemsRequirements";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } =
        new LegacyFieldMapping[]
        {
            new("Items.Req.level", Group, "Level", LegacySettingPart.Bool),
            new("Items.Req.classe", Group, "Class", LegacySettingPart.Bool),
            new("Items.Req.set", Group, "Set", LegacySettingPart.Bool),
        };
}
