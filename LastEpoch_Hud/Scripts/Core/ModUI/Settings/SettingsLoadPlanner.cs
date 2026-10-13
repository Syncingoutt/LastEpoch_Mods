using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

namespace LastEpoch_Hud.Scripts.Core.ModUI.Settings;

/// <summary>Maps the settings file state to a load plan.</summary>
public static class SettingsLoadPlanner
{
    public static SettingsLoadPlan Plan(
        SettingsFileState state,
        bool allGroupsLoaded,
        bool hasMissingEntries,
        LegacyImportOutcome outcome = LegacyImportOutcome.NoLegacyFile
    )
    {
        SettingsLoadPlan plan = PlanFor(state, allGroupsLoaded, hasMissingEntries);
        // Without the legacy values, saving would replace them with defaults for good.
        return plan with
        {
            WriteNow = plan.WriteNow || outcome == LegacyImportOutcome.Imported,
            BlockWrites = outcome == LegacyImportOutcome.ReadFailed,
        };
    }

    private static SettingsLoadPlan PlanFor(
        SettingsFileState state,
        bool allGroupsLoaded,
        bool hasMissingEntries
    )
    {
        return state switch
        {
            SettingsFileState.Missing => new SettingsLoadPlan(true, false, true, false),
            SettingsFileState.Corrupt => new SettingsLoadPlan(true, true, true, false),
            SettingsFileState.Unreadable => new SettingsLoadPlan(false, true, true, false),
            SettingsFileState.Loaded => LoadedPlan(allGroupsLoaded, hasMissingEntries),
            _ => default,
        };
    }

    // A group that failed to load is treated like a corrupt file.
    private static SettingsLoadPlan LoadedPlan(bool allGroupsLoaded, bool hasMissingEntries)
    {
        if (!allGroupsLoaded)
        {
            return new SettingsLoadPlan(true, true, true, false);
        }
        return new SettingsLoadPlan(false, false, hasMissingEntries, false);
    }
}
