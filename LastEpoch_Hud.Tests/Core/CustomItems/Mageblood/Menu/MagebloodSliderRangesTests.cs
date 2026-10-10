using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu;

public sealed class MagebloodSliderRangesTests
{
    [Theory]
    [InlineData(5f, 20f, true)]
    [InlineData(20f, 20f, true)]
    [InlineData(35f, 35f, true)]
    [InlineData(35.5f, 36f, false)]
    [InlineData(12.5f, 20f, false)]
    [InlineData(float.NaN, 20f, true)]
    [InlineData(float.PositiveInfinity, 20f, true)]
    [InlineData(float.NegativeInfinity, 20f, true)]
    public void ForMaxResistances_Range(float value, float max, bool whole)
    {
        Assert.Equal(
            new MagebloodSliderRange(0f, max, whole),
            MagebloodSliderRanges.ForMaxResistances(value)
        );
    }
}
