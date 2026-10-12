using System;
using System.Collections.Generic;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>Finds map targets no registered setting has.</summary>
public static class LegacyMapCheck
{
    public static IReadOnlyList<string> UnknownTargets(
        IReadOnlyList<LegacyFieldMapping> map,
        Func<string, string, bool> isKnown
    )
    {
        List<string> unknown = new();
        foreach (LegacyFieldMapping mapping in map)
        {
            if (isKnown(mapping.Group, mapping.Key))
            {
                continue;
            }
            string target = mapping.Group + "." + mapping.Key;
            if (!unknown.Contains(target))
            {
                unknown.Add(target);
            }
        }
        return unknown;
    }
}
