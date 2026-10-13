using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Tests.Core.ModUI.Sliders;

public sealed class CharacterSliderLimitsTests
{
    private const int RangeCount = 21;

    [Fact]
    public void EveryRange_HasMinBelowMax()
    {
        SliderRange[] ranges = SliderLimitsReflection.AllRanges(typeof(CharacterSliderLimits));

        Assert.Equal(RangeCount, ranges.Length);
        Assert.All(ranges, range => Assert.True(range.Min < range.Max));
    }

    [Fact]
    public void WeaverRanges_KeepFormerPageLimits()
    {
        Assert.Equal(new SliderRange(0f, 200f, true), CharacterSliderLimits.WeaverTreePoints);
        Assert.Equal(new SliderRange(0f, 10f, true), CharacterSliderLimits.WeaverPointMultiplier);
    }
}
