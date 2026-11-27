using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Tests.Core.ModUI.Sliders;

public sealed class OffAtZeroSliderTests
{
    [Theory]
    [InlineData(0f, false)]
    [InlineData(0.00005f, false)]
    [InlineData(OffAtZeroSlider.OffThreshold, false)]
    [InlineData(0.0002f, true)]
    [InlineData(-1f, false)]
    [InlineData(0.5f, true)]
    [InlineData(1f, true)]
    public void IsOn_ComparesAgainstThreshold(float position, bool expected)
    {
        Assert.Equal(expected, OffAtZeroSlider.IsOn(position));
    }

    [Fact]
    public void Position_Off_IsZero()
    {
        Assert.Equal(0f, OffAtZeroSlider.Position(false, 7f));
    }

    [Fact]
    public void Position_On_IsValue()
    {
        Assert.Equal(7f, OffAtZeroSlider.Position(true, 7f));
    }
}
