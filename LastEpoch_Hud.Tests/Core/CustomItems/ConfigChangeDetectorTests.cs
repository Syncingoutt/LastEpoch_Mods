using LastEpoch_Hud.Scripts.Core.CustomItems;

namespace LastEpoch_Hud.Tests.Core.CustomItems;

public sealed class ConfigChangeDetectorTests
{
    private static readonly DateTime _t1 = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime _t2 = _t1.AddSeconds(5);

    [Fact]
    public void IsCheckDue_BeforeInterval_False()
    {
        var detector = new ConfigChangeDetector(1.0);
        detector.IsCheckDue(0);

        Assert.False(detector.IsCheckDue(0.5));
    }

    [Fact]
    public void HasChanged_SameStamp_False()
    {
        var detector = new ConfigChangeDetector(1.0);
        detector.Remember(_t1);

        Assert.False(detector.HasChanged(_t1));
    }

    [Fact]
    public void HasChanged_NewStamp_TrueOnce()
    {
        var detector = new ConfigChangeDetector(1.0);
        detector.Remember(_t1);

        Assert.True(detector.HasChanged(_t2));
        Assert.False(detector.HasChanged(_t2));
    }

    [Fact]
    public void HasChanged_OlderStamp_True()
    {
        var detector = new ConfigChangeDetector(1.0);
        detector.Remember(_t2);

        Assert.True(detector.HasChanged(_t1));
    }

    [Fact]
    public void HasChanged_Null_False()
    {
        var detector = new ConfigChangeDetector(1.0);
        detector.Remember(_t1);

        Assert.False(detector.HasChanged(null));
        Assert.False(detector.HasChanged(_t1));
    }

    [Fact]
    public void HasChanged_NoBaseline_StampAppears_True()
    {
        var detector = new ConfigChangeDetector(1.0);
        detector.Remember(null);

        Assert.True(detector.HasChanged(_t1));
    }

    [Fact]
    public void ForceNext_IsCheckDueInsideInterval_True()
    {
        var detector = new ConfigChangeDetector(1.0);
        detector.IsCheckDue(0);
        detector.ForceNext();

        Assert.True(detector.IsCheckDue(0.5));
    }

    [Fact]
    public void ForceNext_SameStamp_ChangedOnce()
    {
        var detector = new ConfigChangeDetector(1.0);
        detector.Remember(_t1);
        detector.ForceNext();

        Assert.True(detector.HasChanged(_t1));
        Assert.False(detector.HasChanged(_t1));
    }

    [Fact]
    public void ForceNext_NullStamp_FalseAndCleared()
    {
        var detector = new ConfigChangeDetector(1.0);
        detector.Remember(_t1);
        detector.IsCheckDue(0);
        detector.ForceNext();

        Assert.False(detector.HasChanged(null));
        Assert.False(detector.IsCheckDue(0.5));
        Assert.False(detector.HasChanged(_t1));
    }

    [Fact]
    public void ForceNext_AfterConsumed_GateRules()
    {
        var detector = new ConfigChangeDetector(1.0);
        detector.Remember(_t1);
        detector.IsCheckDue(0);
        detector.ForceNext();
        detector.HasChanged(_t1);

        Assert.False(detector.IsCheckDue(0.5));
    }
}
