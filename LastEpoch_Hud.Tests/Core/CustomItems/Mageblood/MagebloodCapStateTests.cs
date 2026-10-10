using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood;

public sealed class MagebloodCapStateTests
{
    private static readonly IntPtr _owner1 = new(1);
    private static readonly IntPtr _owner2 = new(2);

    [Fact]
    public void TryArm_WornFirst_ReportsRaise()
    {
        var state = new MagebloodCapState();

        bool result = state.TryArm(true, 5f, 75f, out float from, out float to);

        Assert.True(result);
        Assert.Equal(75f, from);
        Assert.Equal(80f, to);
        Assert.True(state.Armed);
    }

    [Fact]
    public void TryArm_SameAgain_NoChange()
    {
        MagebloodCapState state = Armed();

        bool result = state.TryArm(true, 5f, 75f, out _, out _);

        Assert.False(result);
    }

    [Fact]
    public void TryArm_Unworn_ReportsRestore()
    {
        MagebloodCapState state = Armed();

        bool result = state.TryArm(false, 5f, 75f, out float from, out float to);

        Assert.True(result);
        Assert.Equal(80f, from);
        Assert.Equal(75f, to);
        Assert.False(state.Armed);
    }

    [Fact]
    public void TryArm_BonusChange_Reports()
    {
        MagebloodCapState state = Armed();

        bool result = state.TryArm(true, 10f, 75f, out float from, out float to);

        Assert.True(result);
        Assert.Equal(80f, from);
        Assert.Equal(85f, to);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(-1f)]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    public void TryArm_UnusableBonus_Disarms(float bonus)
    {
        MagebloodCapState state = Armed();

        bool result = state.TryArm(true, bonus, 75f, out float from, out float to);

        Assert.True(result);
        Assert.Equal(80f, from);
        Assert.Equal(75f, to);
        Assert.False(state.Armed);
    }

    [Fact]
    public void TryArm_NonFiniteBase_StoresBonusNoReport()
    {
        var state = new MagebloodCapState();

        bool result = state.TryArm(true, 5f, float.NaN, out float from, out float to);

        Assert.False(result);
        Assert.True(float.IsNaN(from));
        Assert.True(float.IsNaN(to));
        Assert.True(state.Armed);
    }

    [Fact]
    public void TryArm_BaseChangedSameBonus_NoReport()
    {
        MagebloodCapState state = Armed();

        bool result = state.TryArm(true, 5f, 70f, out _, out _);

        Assert.False(result);
    }

    [Fact]
    public void TryArm_FractionBase_ReportsFractionRaise()
    {
        var state = new MagebloodCapState();

        bool result = state.TryArm(true, 5f, 0.75f, out _, out float to);

        Assert.True(result);
        Assert.Equal(0.8f, to, 1e-5f);
    }

    [Fact]
    public void TryArm_NeverWorn_NoReport()
    {
        var state = new MagebloodCapState();

        bool result = state.TryArm(false, 5f, 75f, out _, out _);

        Assert.False(result);
    }

    [Fact]
    public void Targets_Owner_True()
    {
        MagebloodCapState state = Armed();

        Assert.True(state.Targets(_owner1, _owner1));
    }

    [Fact]
    public void Targets_OtherInstance_False()
    {
        MagebloodCapState state = Armed();

        Assert.False(state.Targets(_owner2, _owner1));
    }

    [Fact]
    public void Targets_ZeroOwner_False()
    {
        MagebloodCapState state = Armed();

        Assert.False(state.Targets(IntPtr.Zero, IntPtr.Zero));
    }

    [Fact]
    public void Targets_Disarmed_False()
    {
        var state = new MagebloodCapState();

        Assert.False(state.Targets(_owner1, _owner1));
    }

    [Fact]
    public void Targets_Inside_False()
    {
        MagebloodCapState state = Armed();
        state.TryEnter(75f, out _);

        Assert.False(state.Targets(_owner1, _owner1));
    }

    [Fact]
    public void TryEnter_Armed_Raises()
    {
        MagebloodCapState state = Armed();

        bool result = state.TryEnter(75f, out float raised);

        Assert.True(result);
        Assert.Equal(80f, raised);
    }

    [Fact]
    public void TryEnter_Nested_False()
    {
        MagebloodCapState state = Armed();
        state.TryEnter(75f, out _);

        bool result = state.TryEnter(80f, out float raised);

        Assert.False(result);
        Assert.Equal(80f, raised);
    }

    [Fact]
    public void TryEnter_AtCeiling_False()
    {
        MagebloodCapState state = Armed();

        bool result = state.TryEnter(100f, out float raised);

        Assert.False(result);
        Assert.Equal(100f, raised);
        AssertStillIdle(state);
    }

    [Fact]
    public void TryEnter_NonFinite_False()
    {
        MagebloodCapState state = Armed();

        bool result = state.TryEnter(float.NaN, out float raised);

        Assert.False(result);
        Assert.True(float.IsNaN(raised));
        AssertStillIdle(state);
    }

    [Fact]
    public void TryEnter_Disarmed_False()
    {
        var state = new MagebloodCapState();

        bool result = state.TryEnter(75f, out float raised);

        Assert.False(result);
        Assert.Equal(75f, raised);
    }

    [Fact]
    public void TryExit_AfterEnter_RestoresOnce()
    {
        MagebloodCapState state = Armed();
        state.TryEnter(75f, out _);

        bool first = state.TryExit(out float restore);
        bool second = state.TryExit(out _);

        Assert.True(first);
        Assert.Equal(75f, restore);
        Assert.False(second);
    }

    [Fact]
    public void TryExit_WithoutEnter_False()
    {
        var state = new MagebloodCapState();

        bool result = state.TryExit(out float restore);

        Assert.False(result);
        Assert.Equal(0f, restore);
    }

    [Fact]
    public void EnterExitEnter_SecondHitRaisesAgain()
    {
        MagebloodCapState state = Armed();
        state.TryEnter(75f, out _);
        state.TryExit(out _);

        bool result = state.TryEnter(75f, out float raised);

        Assert.True(result);
        Assert.Equal(80f, raised);
    }

    private static void AssertStillIdle(MagebloodCapState state)
    {
        Assert.False(state.TryExit(out _));
        Assert.True(state.Targets(_owner1, _owner1));
    }

    private static MagebloodCapState Armed()
    {
        var state = new MagebloodCapState();
        state.TryArm(true, 5f, 75f, out _, out _);
        return state;
    }
}
