using LastEpoch_Hud.Scripts.Core.Items;

namespace LastEpoch_Hud.Tests.Core.Items;

/// <summary>Saved affix-count floats round like the whole-number slider (midpoint to even).</summary>
public sealed class DropAffixCountTests
{
    [Theory]
    [InlineData(2.6f, 3)]
    [InlineData(2.4f, 2)]
    [InlineData(2.5f, 2)]
    [InlineData(3.5f, 4)]
    [InlineData(0f, 0)]
    [InlineData(6f, 6)]
    public void ToWhole_RoundsLikeWholeSlider(float value, int expected) =>
        Assert.Equal(expected, DropAffixCount.ToWhole(value));

    [Fact]
    public void Bounds_DoesNotOverride_WhenDefaultRange() =>
        Assert.Equal(new AffixCountBounds(0, 4, false), DropAffixCount.Bounds(0f, 4f));

    [Fact]
    public void Bounds_Overrides_WhenRangeDiffers() =>
        Assert.Equal(new AffixCountBounds(2, 5, true), DropAffixCount.Bounds(2f, 5f));

    [Fact]
    public void Bounds_OverridesAndClampsMinToZero_WhenMinNegative() =>
        Assert.Equal(new AffixCountBounds(0, 4, true), DropAffixCount.Bounds(-1f, 4f));

    [Fact]
    public void Bounds_OverridesAndClampsMaxToSix_WhenMaxTooHigh() =>
        Assert.Equal(new AffixCountBounds(3, 6, true), DropAffixCount.Bounds(3f, 9f));

    [Fact]
    public void Bounds_OverridesAndClampsMaxToZero_WhenMaxNegative() =>
        Assert.Equal(new AffixCountBounds(0, 0, true), DropAffixCount.Bounds(0f, -2f));

    [Fact]
    public void Bounds_RoundsBeforeDeciding() =>
        Assert.Equal(new AffixCountBounds(1, 4, true), DropAffixCount.Bounds(0.6f, 4.4f));

    [Fact]
    public void Bounds_DoesNotOverride_WhenRoundedToDefault() =>
        Assert.Equal(new AffixCountBounds(0, 4, false), DropAffixCount.Bounds(0.4f, 3.6f));
}
