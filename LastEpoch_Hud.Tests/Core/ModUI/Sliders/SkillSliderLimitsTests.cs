using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Tests.Core.ModUI.Sliders;

public sealed class SkillSliderLimitsTests
{
    private const int RangeCount = 32;

    [Fact]
    public void EveryRange_HasMinBelowMax()
    {
        SliderRange[] ranges = SliderLimitsReflection.AllRanges(typeof(SkillSliderLimits));

        Assert.Equal(RangeCount, ranges.Length);
        Assert.All(ranges, range => Assert.True(range.Min < range.Max));
    }
}
