using LastEpoch_Hud.Scripts.Core.CustomItems.Headhunter.Buffs;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood;

public sealed class MagebloodAppliedStateTests
{
    [Fact]
    public void New_IsDirty()
    {
        var state = new MagebloodAppliedState();

        Assert.Equal(MagebloodSyncReason.Dirty, state.NeedsSync(false, true));
    }

    [Fact]
    public void New_HasEmptyDefaults()
    {
        var state = new MagebloodAppliedState();

        Assert.Empty(state.Names);
        Assert.False(state.Worn);
        Assert.Equal(0, state.ActiveFlasks);
    }

    [Fact]
    public void Record_KeepsAddNamesInOrder_Distinct()
    {
        var state = new MagebloodAppliedState();

        state.Record(
            new[]
            {
                Action(BuffActionKind.Add, "MB_B"),
                Action(BuffActionKind.Remove, "MB_X"),
                Action(BuffActionKind.Add, "MB_A"),
                Action(BuffActionKind.Add, "MB_B"),
            },
            true,
            1
        );

        Assert.Equal(new[] { "MB_B", "MB_A" }, state.Names);
    }

    [Fact]
    public void Record_Null_NoNames()
    {
        var state = new MagebloodAppliedState();
        state.Record(new[] { Action(BuffActionKind.Add, "MB_A") }, true, 1);

        state.Record(null, true, 1);

        Assert.Empty(state.Names);
    }

    [Fact]
    public void Record_StoresWornAndActive()
    {
        var state = new MagebloodAppliedState();

        state.Record(Array.Empty<BuffAction>(), true, 3);

        Assert.True(state.Worn);
        Assert.Equal(3, state.ActiveFlasks);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void AfterRecord_None(bool worn)
    {
        var state = new MagebloodAppliedState();
        state.Record(Array.Empty<BuffAction>(), worn, 2);

        Assert.Equal(MagebloodSyncReason.None, state.NeedsSync(worn, true));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WornChanged_IsWornChanged(bool recordedWorn)
    {
        var state = new MagebloodAppliedState();
        state.Record(Array.Empty<BuffAction>(), recordedWorn, 2);

        Assert.Equal(MagebloodSyncReason.WornChanged, state.NeedsSync(!recordedWorn, true));
    }

    [Fact]
    public void NotAllLive_IsBuffsLost()
    {
        var state = new MagebloodAppliedState();
        state.Record(Array.Empty<BuffAction>(), true, 2);

        Assert.Equal(MagebloodSyncReason.BuffsLost, state.NeedsSync(true, false));
    }

    [Fact]
    public void DirtyAndNotAllLive_IsBuffsLost()
    {
        var state = new MagebloodAppliedState();
        state.Record(Array.Empty<BuffAction>(), true, 2);
        state.MarkDirty();

        Assert.Equal(MagebloodSyncReason.BuffsLost, state.NeedsSync(true, false));
    }

    [Fact]
    public void WornChangedAndNotAllLive_IsWornChanged()
    {
        var state = new MagebloodAppliedState();
        state.Record(Array.Empty<BuffAction>(), true, 2);

        Assert.Equal(MagebloodSyncReason.WornChanged, state.NeedsSync(false, false));
    }

    [Fact]
    public void MarkDirty_IsDirty()
    {
        var state = new MagebloodAppliedState();
        state.Record(Array.Empty<BuffAction>(), true, 2);

        state.MarkDirty();

        Assert.Equal(MagebloodSyncReason.Dirty, state.NeedsSync(true, true));
    }

    [Fact]
    public void Clear_ResetsAll()
    {
        var state = new MagebloodAppliedState();
        state.Record(new[] { Action(BuffActionKind.Add, "MB_A") }, true, 2);

        state.Clear();

        Assert.Empty(state.Names);
        Assert.False(state.Worn);
        Assert.Equal(0, state.ActiveFlasks);
        Assert.Equal(MagebloodSyncReason.Dirty, state.NeedsSync(false, true));
    }

    [Fact]
    public void RecordIfIdle_New_TrueAndClean()
    {
        var state = new MagebloodAppliedState();

        bool idle = state.RecordIfIdle(false);

        Assert.True(idle);
        Assert.Equal(MagebloodSyncReason.None, state.NeedsSync(false, true));
    }

    [Fact]
    public void RecordIfIdle_Worn_FalseAndStateUntouched()
    {
        var state = new MagebloodAppliedState();

        bool idle = state.RecordIfIdle(true);

        Assert.False(idle);
        Assert.Equal(MagebloodSyncReason.Dirty, state.NeedsSync(false, true));
    }

    [Fact]
    public void RecordIfIdle_RecordedWorn_False()
    {
        var state = new MagebloodAppliedState();
        state.Record(Array.Empty<BuffAction>(), true, 2);

        MagebloodSyncReason before = state.NeedsSync(false, true);

        Assert.False(state.RecordIfIdle(false));
        Assert.Equal(MagebloodSyncReason.WornChanged, before);
        Assert.Equal(before, state.NeedsSync(false, true));
    }

    [Fact]
    public void RecordIfIdle_NamesApplied_False()
    {
        var state = new MagebloodAppliedState();
        state.Record(new[] { Action(BuffActionKind.Add, "MB_A") }, false, 1);

        Assert.False(state.RecordIfIdle(false));
        Assert.Equal(MagebloodSyncReason.BuffsLost, state.NeedsSync(false, false));
    }

    [Fact]
    public void RecordIfIdle_AfterClear_True()
    {
        var state = new MagebloodAppliedState();
        state.Record(new[] { Action(BuffActionKind.Add, "MB_A") }, true, 1);
        state.Clear();

        Assert.True(state.RecordIfIdle(false));
    }

    [Fact]
    public void RecordIfIdle_AfterMarkDirty_TrueAndClean()
    {
        var state = new MagebloodAppliedState();
        state.Record(Array.Empty<BuffAction>(), false, 0);
        state.MarkDirty();

        bool idle = state.RecordIfIdle(false);

        Assert.True(idle);
        Assert.Equal(MagebloodSyncReason.None, state.NeedsSync(false, true));
    }

    private static BuffAction Action(BuffActionKind kind, string name)
    {
        return new BuffAction(kind, name, 1, 1f, 0f, 1f, 1);
    }
}
