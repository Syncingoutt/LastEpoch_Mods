using LastEpoch_Hud.Scripts.Core.ModUI.Settings;

namespace LastEpoch_Hud.Tests.Core.ModUI.Settings;

public sealed class SettingsLoadPlannerTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Plan_Missing_ResetsAndWritesWithoutBackup(bool hasMissingEntries)
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(
            SettingsFileState.Missing,
            true,
            hasMissingEntries
        );

        Assert.Equal(new SettingsLoadPlan(true, false, true), plan);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Plan_Corrupt_ResetsBacksUpAndWrites(bool hasMissingEntries)
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(
            SettingsFileState.Corrupt,
            true,
            hasMissingEntries
        );

        Assert.Equal(new SettingsLoadPlan(true, true, true), plan);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Plan_Unreadable_BacksUpAndWritesWithoutReset(bool hasMissingEntries)
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(
            SettingsFileState.Unreadable,
            true,
            hasMissingEntries
        );

        Assert.Equal(new SettingsLoadPlan(false, true, true), plan);
    }

    [Fact]
    public void Plan_LoadedComplete_DoesNothing()
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(SettingsFileState.Loaded, true, false);

        Assert.Equal(new SettingsLoadPlan(false, false, false), plan);
    }

    [Fact]
    public void Plan_LoadedWithMissingEntries_OnlyWrites()
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(SettingsFileState.Loaded, true, true);

        Assert.Equal(new SettingsLoadPlan(false, false, true), plan);
    }

    [Fact]
    public void Plan_UndefinedState_DoesNothing()
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan((SettingsFileState)99, true, true);

        Assert.Equal(new SettingsLoadPlan(false, false, false), plan);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Plan_LoadedButGroupFailed_ResetsBacksUpAndWrites(bool hasMissingEntries)
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(
            SettingsFileState.Loaded,
            false,
            hasMissingEntries
        );

        Assert.Equal(new SettingsLoadPlan(true, true, true), plan);
    }

    [Theory]
    [InlineData(SettingsFileState.Missing, true, false, true)]
    [InlineData(SettingsFileState.Corrupt, true, true, true)]
    [InlineData(SettingsFileState.Unreadable, false, true, true)]
    public void Plan_NotLoaded_IgnoresGroupFlag(
        SettingsFileState state,
        bool reset,
        bool backup,
        bool write
    )
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(state, false, false);

        Assert.Equal(new SettingsLoadPlan(reset, backup, write), plan);
    }

    [Fact]
    public void Plan_LoadedComplete_Imported_WritesOnly()
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(
            SettingsFileState.Loaded,
            true,
            false,
            imported: true
        );

        Assert.Equal(new SettingsLoadPlan(false, false, true), plan);
    }

    [Theory]
    [InlineData(SettingsFileState.Missing)]
    [InlineData(SettingsFileState.Corrupt)]
    [InlineData(SettingsFileState.Unreadable)]
    [InlineData(SettingsFileState.Loaded)]
    public void Plan_Imported_DoesNotChangeResetOrBackup(SettingsFileState state)
    {
        SettingsLoadPlan baseline = SettingsLoadPlanner.Plan(state, true, false, imported: false);

        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(state, true, false, imported: true);

        Assert.Equal(baseline with { WriteNow = true }, plan);
    }
}
