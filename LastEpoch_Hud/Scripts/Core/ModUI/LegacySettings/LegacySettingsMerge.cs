using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>Imports the legacy groups a settings root lacks and merges them in.</summary>
public static class LegacySettingsMerge
{
    public static LegacyImportOutcome ImportMissing(
        string legacyJson,
        JObject root,
        IReadOnlyList<LegacyFieldMapping> map,
        ILegacyKeyNames keys,
        List<LegacyImportProblem> problems
    )
    {
        if (legacyJson == null)
        {
            return LegacyImportOutcome.NoLegacyFile;
        }
        IReadOnlyList<string> groups = LegacyImportGroups.MissingFrom(root, map);
        if (groups.Count == 0)
        {
            return LegacyImportOutcome.NothingMissing;
        }
        if (
            !LegacySettingsImporter.TryImport(
                legacyJson,
                map,
                groups,
                keys,
                out JObject sections,
                problems
            )
        )
        {
            return LegacyImportOutcome.FileFailed;
        }
        if (!sections.HasValues)
        {
            return problems.Count > 0
                ? LegacyImportOutcome.FieldsFailed
                : LegacyImportOutcome.NothingMissing;
        }
        foreach (JProperty section in sections.Properties())
        {
            root[section.Name] = section.Value;
        }
        return LegacyImportOutcome.Imported;
    }
}
