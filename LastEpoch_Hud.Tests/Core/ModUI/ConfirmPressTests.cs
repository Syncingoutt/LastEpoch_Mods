using LastEpoch_Hud.Scripts.Core.ModUI;

namespace LastEpoch_Hud.Tests.Core.ModUI;

public sealed class ConfirmPressTests
{
    private const double Window = 3;

    [Fact]
    public void Press_First_ArmsWithoutConfirm()
    {
        var confirm = new ConfirmPress(Window);

        Assert.False(confirm.Press(0));
        Assert.True(confirm.IsArmed(0));
    }

    [Fact]
    public void Press_SecondInsideWindow_ConfirmsAndDisarms()
    {
        var confirm = new ConfirmPress(Window);
        confirm.Press(0);

        Assert.True(confirm.Press(2.9));
        Assert.False(confirm.IsArmed(2.9));
    }

    [Fact]
    public void Press_AtWindowEnd_Rearms()
    {
        var confirm = new ConfirmPress(Window);
        confirm.Press(0);

        Assert.False(confirm.Press(3.0));
        Assert.True(confirm.IsArmed(3.0));
    }

    [Fact]
    public void IsArmed_AfterWindow_False()
    {
        var confirm = new ConfirmPress(Window);
        confirm.Press(0);

        Assert.True(confirm.IsArmed(2.99));
        Assert.False(confirm.IsArmed(3.0));
    }

    [Fact]
    public void Reset_Disarms()
    {
        var confirm = new ConfirmPress(Window);
        confirm.Press(0);
        confirm.Reset();

        Assert.False(confirm.IsArmed(1));
    }

    [Fact]
    public void Press_AfterReset_ArmsAgain()
    {
        var confirm = new ConfirmPress(Window);
        confirm.Press(0);
        confirm.Reset();

        Assert.False(confirm.Press(1));
    }

    [Fact]
    public void IsArmed_New_False()
    {
        Assert.False(new ConfirmPress(Window).IsArmed(0));
    }
}
