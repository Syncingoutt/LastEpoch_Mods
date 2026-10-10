using LastEpoch_Hud.Scripts.Core.CustomItems.Headhunter;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Headhunter;

public sealed class HeadhunterActiveSceneGateTests
{
    private const string ZoneA = "FakeZoneA";
    private const string ZoneB = "FakeZoneB";

    [Fact]
    public void TryEnter_FirstValidScene_True()
    {
        Assert.True(new HeadhunterActiveSceneGate().TryEnter(11, ZoneA));
    }

    [Fact]
    public void TryEnter_SameHandleAgain_False()
    {
        var gate = new HeadhunterActiveSceneGate();
        gate.TryEnter(11, ZoneA);

        Assert.False(gate.TryEnter(11, ZoneA));
    }

    [Fact]
    public void TryEnter_NewHandle_True()
    {
        var gate = new HeadhunterActiveSceneGate();
        gate.TryEnter(11, ZoneA);

        Assert.True(gate.TryEnter(12, ZoneB));
    }

    [Fact]
    public void TryEnter_SameNameNewHandle_True()
    {
        var gate = new HeadhunterActiveSceneGate();
        gate.TryEnter(11, ZoneA);

        Assert.True(gate.TryEnter(12, ZoneA));
    }

    [Fact]
    public void TryEnter_BackToEarlierHandle_True()
    {
        var gate = new HeadhunterActiveSceneGate();
        gate.TryEnter(11, ZoneA);
        gate.TryEnter(12, ZoneB);

        Assert.True(gate.TryEnter(11, ZoneA));
    }

    [Fact]
    public void TryEnter_ZeroHandle_False()
    {
        Assert.False(new HeadhunterActiveSceneGate().TryEnter(0, ZoneA));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TryEnter_NullOrEmptyName_False(string name)
    {
        Assert.False(new HeadhunterActiveSceneGate().TryEnter(11, name));
    }

    [Fact]
    public void TryEnter_InvalidSceneKeepsLastHandle()
    {
        var gate = new HeadhunterActiveSceneGate();
        gate.TryEnter(11, ZoneA);

        Assert.False(gate.TryEnter(0, ZoneA));
        Assert.False(gate.TryEnter(11, ZoneA));
        Assert.True(gate.TryEnter(12, ZoneB));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TryEnter_EmptyNameDoesNotRememberHandle(string name)
    {
        var gate = new HeadhunterActiveSceneGate();
        gate.TryEnter(11, ZoneA);

        Assert.False(gate.TryEnter(12, name));
        Assert.True(gate.TryEnter(12, ZoneB));
    }

    [Fact]
    public void IsNew_FreshGateValidHandle_True()
    {
        Assert.True(new HeadhunterActiveSceneGate().IsNew(11));
    }

    [Fact]
    public void IsNew_OtherThanLastEntered_True()
    {
        var gate = new HeadhunterActiveSceneGate();
        gate.TryEnter(11, ZoneA);

        Assert.True(gate.IsNew(12));
    }

    [Fact]
    public void IsNew_LastEnteredHandle_False()
    {
        var gate = new HeadhunterActiveSceneGate();
        gate.TryEnter(11, ZoneA);

        Assert.False(gate.IsNew(11));
    }

    [Fact]
    public void IsNew_EarlierHandleAfterSwitch_True()
    {
        var gate = new HeadhunterActiveSceneGate();
        gate.TryEnter(11, ZoneA);
        gate.TryEnter(12, ZoneB);

        Assert.True(gate.IsNew(11));
    }

    [Fact]
    public void IsNew_ZeroHandleFreshGate_False()
    {
        Assert.False(new HeadhunterActiveSceneGate().IsNew(0));
    }

    [Fact]
    public void IsNew_ZeroHandleAfterEnter_False()
    {
        var gate = new HeadhunterActiveSceneGate();
        gate.TryEnter(11, ZoneA);

        Assert.False(gate.IsNew(0));
    }

    [Fact]
    public void IsNew_DoesNotRememberHandle()
    {
        var gate = new HeadhunterActiveSceneGate();

        Assert.True(gate.IsNew(11));
        Assert.True(gate.IsNew(11));
        Assert.True(gate.TryEnter(11, ZoneA));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void IsNew_HandleRejectedForEmptyName_StaysNew(string name)
    {
        var gate = new HeadhunterActiveSceneGate();
        gate.TryEnter(11, ZoneA);
        gate.TryEnter(12, name);

        Assert.True(gate.IsNew(12));
    }
}
