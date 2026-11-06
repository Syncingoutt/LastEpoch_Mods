using System;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>Finds a dotted legacy path in a parsed Save.json.</summary>
internal static class LegacyJsonPath
{
    public static JToken Find(JObject root, string dottedPath)
    {
        JToken current = root;
        foreach (string segment in dottedPath.Split('.'))
        {
            if (current is not JObject node)
            {
                return null;
            }
            current = node.GetValue(segment, StringComparison.OrdinalIgnoreCase);
        }
        return current;
    }
}
