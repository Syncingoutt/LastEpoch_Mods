namespace LastEpoch_Hud.Scripts.Core.ModUI.Settings;

/// <summary>Maps the settings file state to a load plan.</summary>
public static class SettingsLoadPlanner
{
    public static SettingsLoadPlan Plan(
        SettingsFileState state,
        bool allGroupsLoaded,
        bool hasMissingEntries,
        bool imported = false
    )
    {
        SettingsLoadPlan plan = PlanFor(state, allGroupsLoaded, hasMissingEntries);
        return imported ? plan with { WriteNow = true } : plan;
    }

    private static SettingsLoadPlan PlanFor(
        SettingsFileState state,
        bool allGroupsLoaded,
        bool hasMissingEntries
    )
    {
        return state switch
        {
            SettingsFileState.Missing => new SettingsLoadPlan(true, false, true),
            SettingsFileState.Corrupt => new SettingsLoadPlan(true, true, true),
            SettingsFileState.Unreadable => new SettingsLoadPlan(false, true, true),
            SettingsFileState.Loaded => LoadedPlan(allGroupsLoaded, hasMissingEntries),
            _ => default,
        };
    }

    // A group that failed to load is treated like a corrupt file.
    private static SettingsLoadPlan LoadedPlan(bool allGroupsLoaded, bool hasMissingEntries)
    {
        if (!allGroupsLoaded)
        {
            return new SettingsLoadPlan(true, true, true);
        }
        return new SettingsLoadPlan(false, false, hasMissingEntries);
    }
}
