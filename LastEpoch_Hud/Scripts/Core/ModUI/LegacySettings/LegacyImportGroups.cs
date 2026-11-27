using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>Tells which legacy groups still need an import.</summary>
public static class LegacyImportGroups
{
    public static IReadOnlyList<string> MissingFrom(
        JObject saveRoot,
        IReadOnlyList<LegacyFieldMapping> map
    )
    {
        return map.Select(mapping => mapping.Group)
            .Distinct()
            .Where(group => saveRoot == null || saveRoot[group] is not JObject)
            .ToList();
    }
}
