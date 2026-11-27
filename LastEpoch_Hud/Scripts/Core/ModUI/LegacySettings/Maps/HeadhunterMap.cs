using System.Collections.Generic;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Items.Headhunter fields mapped to the Headhunter group.</summary>
public static class HeadhunterMap
{
    public const string Group = "Headhunter";

    private const string Prefix = "Items.Headhunter.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } =
        new[] { Bool("WeaverWill"), Bool("UniqueDrop"), Bool("BaseDrop") };

    private static LegacyFieldMapping Bool(string name)
    {
        return new LegacyFieldMapping(Prefix + name, Group, name, LegacySettingPart.Bool);
    }
}
