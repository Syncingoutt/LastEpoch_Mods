using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu;

public sealed class MagebloodEditDebounceTests
{
    private const double Quiet = 0.4;

    private static readonly MagebloodValueTarget _maxRes = MagebloodValueTarget.ForMaxResistances();
    private static readonly MagebloodValueTarget _stat = MagebloodValueTarget.ForStat(
        "FakeA",
        "FakeA",
        MagebloodStatField.Increased
    );

    [Fact]
    public void TryTakeDue_BeforeQuiet_False()
    {
        var debounce = new MagebloodEditDebounce(Quiet);
        debounce.Set(new MagebloodValueEdit(_maxRes, 7), 0, out _);

        Assert.False(debounce.TryTakeDue(0.39, out _));
    }

    [Fact]
    public void TryTakeDue_AfterQuiet_ReturnsEditOnce()
    {
        var debounce = new MagebloodEditDebounce(Quiet);
        var edit = new MagebloodValueEdit(_maxRes, 7);
        debounce.Set(edit, 0, out _);

        Assert.True(debounce.TryTakeDue(0.4, out MagebloodValueEdit taken));
        Assert.Equal(edit, taken);
        Assert.False(debounce.TryTakeDue(1, out _));
    }

    [Fact]
    public void Set_SameTarget_ReplacesAndRestarts()
    {
        var debounce = new MagebloodEditDebounce(Quiet);
        var second = new MagebloodValueEdit(_maxRes, 9);
        debounce.Set(new MagebloodValueEdit(_maxRes, 7), 0, out _);

        bool flush = debounce.Set(second, 0.3, out _);

        Assert.False(flush);
        Assert.False(debounce.TryTakeDue(0.5, out _));
        Assert.True(debounce.TryTakeDue(0.75, out MagebloodValueEdit taken));
        Assert.Equal(second, taken);
    }

    [Fact]
    public void Set_OtherTarget_ReturnsOldToFlush()
    {
        var debounce = new MagebloodEditDebounce(Quiet);
        var old = new MagebloodValueEdit(_maxRes, 7);
        var other = new MagebloodValueEdit(_stat, 30);
        debounce.Set(old, 0, out _);

        bool flush = debounce.Set(other, 0.1, out MagebloodValueEdit flushFirst);

        Assert.True(flush);
        Assert.Equal(old, flushFirst);
        Assert.False(debounce.TryTakeDue(0.45, out _));
        Assert.True(debounce.TryTakeDue(0.55, out MagebloodValueEdit taken));
        Assert.Equal(other, taken);
    }

    [Fact]
    public void Set_OtherTargetAfterDiscard_NothingToFlush()
    {
        var debounce = new MagebloodEditDebounce(Quiet);
        debounce.Set(new MagebloodValueEdit(_maxRes, 7), 0, out _);
        debounce.Discard();

        Assert.False(debounce.Set(new MagebloodValueEdit(_stat, 30), 0.1, out _));
    }

    [Fact]
    public void Set_OtherTargetAfterTaken_NothingToFlush()
    {
        var debounce = new MagebloodEditDebounce(Quiet);
        debounce.Set(new MagebloodValueEdit(_maxRes, 7), 0, out _);
        debounce.TryTakeDue(1, out _);

        Assert.False(debounce.Set(new MagebloodValueEdit(_stat, 30), 1.1, out _));
    }

    [Fact]
    public void Set_FirstEdit_NothingToFlush()
    {
        var debounce = new MagebloodEditDebounce(Quiet);

        Assert.False(debounce.Set(new MagebloodValueEdit(_maxRes, 7), 0, out _));
    }

    [Fact]
    public void TryPeek_OnlyPendingTarget()
    {
        var debounce = new MagebloodEditDebounce(Quiet);
        debounce.Set(new MagebloodValueEdit(_maxRes, 7), 0, out _);

        Assert.True(debounce.TryPeek(_maxRes, out float value));
        Assert.Equal(7f, value);
        Assert.False(debounce.TryPeek(_stat, out _));
    }

    [Fact]
    public void TryPeek_AfterTaken_False()
    {
        var debounce = new MagebloodEditDebounce(Quiet);
        debounce.Set(new MagebloodValueEdit(_maxRes, 7), 0, out _);
        debounce.TryTakeDue(1, out _);

        Assert.False(debounce.TryPeek(_maxRes, out _));
    }

    [Fact]
    public void Discard_Empties()
    {
        var debounce = new MagebloodEditDebounce(Quiet);
        debounce.Set(new MagebloodValueEdit(_maxRes, 7), 0, out _);
        debounce.Discard();

        Assert.False(debounce.TryPeek(_maxRes, out _));
        Assert.False(debounce.TryTakeDue(1, out _));
    }

    [Fact]
    public void TryTakeDue_Empty_False()
    {
        var debounce = new MagebloodEditDebounce(Quiet);

        Assert.False(debounce.TryTakeDue(1, out _));
    }
}
