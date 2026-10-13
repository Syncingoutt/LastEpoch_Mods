using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
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

        Assert.Equal(new SettingsLoadPlan(true, false, true, false), plan);
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

        Assert.Equal(new SettingsLoadPlan(true, true, true, false), plan);
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

        Assert.Equal(new SettingsLoadPlan(false, true, true, false), plan);
    }

    [Fact]
    public void Plan_LoadedComplete_DoesNothing()
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(SettingsFileState.Loaded, true, false);

        Assert.Equal(new SettingsLoadPlan(false, false, false, false), plan);
    }

    [Fact]
    public void Plan_LoadedWithMissingEntries_OnlyWrites()
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(SettingsFileState.Loaded, true, true);

        Assert.Equal(new SettingsLoadPlan(false, false, true, false), plan);
    }

    [Fact]
    public void Plan_UndefinedState_DoesNothing()
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan((SettingsFileState)99, true, true);

        Assert.Equal(new SettingsLoadPlan(false, false, false, false), plan);
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

        Assert.Equal(new SettingsLoadPlan(true, true, true, false), plan);
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

        Assert.Equal(new SettingsLoadPlan(reset, backup, write, false), plan);
    }

    [Fact]
    public void Plan_LoadedComplete_Imported_WritesOnly()
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(
            SettingsFileState.Loaded,
            true,
            false,
            LegacyImportOutcome.Imported
        );

        Assert.Equal(new SettingsLoadPlan(false, false, true, false), plan);
    }

    [Theory]
    [InlineData(SettingsFileState.Missing)]
    [InlineData(SettingsFileState.Corrupt)]
    [InlineData(SettingsFileState.Unreadable)]
    [InlineData(SettingsFileState.Loaded)]
    public void Plan_Imported_DoesNotChangeResetOrBackup(SettingsFileState state)
    {
        SettingsLoadPlan baseline = SettingsLoadPlanner.Plan(
            state,
            true,
            false,
            LegacyImportOutcome.NothingMissing
        );

        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(
            state,
            true,
            false,
            LegacyImportOutcome.Imported
        );

        Assert.Equal(baseline with { WriteNow = true }, plan);
    }

    [Theory]
    [InlineData(SettingsFileState.Missing)]
    [InlineData(SettingsFileState.Corrupt)]
    [InlineData(SettingsFileState.Unreadable)]
    [InlineData(SettingsFileState.Loaded)]
    public void Plan_LegacyReadFailed_BlocksWrites(SettingsFileState state)
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(
            state,
            true,
            false,
            LegacyImportOutcome.ReadFailed
        );

        Assert.True(plan.BlockWrites);
    }

    [Theory]
    [InlineData(LegacyImportOutcome.NoLegacyFile)]
    [InlineData(LegacyImportOutcome.NothingMissing)]
    [InlineData(LegacyImportOutcome.Imported)]
    [InlineData(LegacyImportOutcome.FileFailed)]
    [InlineData(LegacyImportOutcome.FieldsFailed)]
    public void Plan_OtherOutcomes_DoNotBlockWrites(LegacyImportOutcome outcome)
    {
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(
            SettingsFileState.Missing,
            true,
            false,
            outcome
        );

        Assert.False(plan.BlockWrites);
    }
}
