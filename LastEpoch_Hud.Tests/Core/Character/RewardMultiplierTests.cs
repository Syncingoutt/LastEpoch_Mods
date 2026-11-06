using LastEpoch_Hud.Scripts.Core.Character;

namespace LastEpoch_Hud.Tests.Core.Character;

public sealed class RewardMultiplierTests
{
    [Fact]
    public void Scale_TruncatesMultiplier()
    {
        Assert.Equal(200L, RewardMultiplier.Scale(100L, 2.9f));
    }

    [Fact]
    public void Scale_ZeroMultiplier_GivesZero()
    {
        Assert.Equal(0L, RewardMultiplier.Scale(100L, 0f));
    }

    [Fact]
    public void ScaleSaturated_TruncatesMultiplier()
    {
        Assert.Equal(6u, RewardMultiplier.ScaleSaturated(3u, 2.9f));
    }

    [Fact]
    public void ScaleSaturated_OneKeepsAmount()
    {
        Assert.Equal(7u, RewardMultiplier.ScaleSaturated(7u, 1f));
    }

    [Theory]
    [InlineData(uint.MaxValue)]
    [InlineData(20_000_000u)]
    public void ScaleSaturated_CapsAtUintMax(uint amount)
    {
        Assert.Equal(uint.MaxValue, RewardMultiplier.ScaleSaturated(amount, 255f));
    }
}
