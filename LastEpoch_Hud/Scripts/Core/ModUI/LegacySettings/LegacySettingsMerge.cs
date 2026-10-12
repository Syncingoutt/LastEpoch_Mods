using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>Imports the legacy groups a settings root lacks and merges them in. A failed legacy read only matters when a group is missing.</summary>
public static class LegacySettingsMerge
{
    public static LegacyImportOutcome ImportMissing(
        string legacyJson,
        bool legacyReadFailed,
        JObject root,
        IReadOnlyList<LegacyFieldMapping> map,
        ILegacyKeyNames keys,
        List<LegacyImportProblem> problems
    )
    {
        if (legacyJson == null && !legacyReadFailed)
        {
            return LegacyImportOutcome.NoLegacyFile;
        }
        IReadOnlyList<string> groups = LegacyImportGroups.MissingFrom(root, map);
        if (groups.Count == 0)
        {
            return LegacyImportOutcome.NothingMissing;
        }
        if (legacyReadFailed)
        {
            // Nothing was imported: saving defaults now would hide the legacy values for good.
            problems.Add(
                new LegacyImportProblem(
                    LegacyImportProblemCode.ReadFailed,
                    "",
                    "Legacy settings file could not be read."
                )
            );
            return LegacyImportOutcome.ReadFailed;
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
