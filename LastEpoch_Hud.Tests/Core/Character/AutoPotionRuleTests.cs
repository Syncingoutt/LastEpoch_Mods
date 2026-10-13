using LastEpoch_Hud.Scripts.Core.Character;

namespace LastEpoch_Hud.Tests.Core.Character;

public sealed class AutoPotionRuleTests
{
    [Theory]
    [InlineData(0f, 100f)]
    [InlineData(50f, 100f)]
    public void ShouldDrink_ReturnsFalse_WhenThresholdIsZero(float current, float max)
    {
        Assert.False(AutoPotionRule.ShouldDrink(current, max, 0f));
    }

    [Fact]
    public void ShouldDrink_ReturnsTrue_WhenBelowThresholdPercent()
    {
        Assert.True(AutoPotionRule.ShouldDrink(49f, 100f, 127.5f));
    }

    [Fact]
    public void ShouldDrink_ReturnsFalse_WhenAtThresholdPercent()
    {
        Assert.False(AutoPotionRule.ShouldDrink(50f, 100f, 127.5f));
    }

    [Fact]
    public void ShouldDrink_ReturnsFalse_WhenHealthPercentTruncatesToThreshold()
    {
        Assert.False(AutoPotionRule.ShouldDrink(50.9f, 100f, 128f));
    }

    [Fact]
    public void ShouldDrink_ReturnsTrue_WhenHealthPercentTruncatesBelowThreshold()
    {
        Assert.True(AutoPotionRule.ShouldDrink(49.9f, 100f, 128f));
    }

    [Fact]
    public void ShouldDrink_ReturnsTrue_WhenBelowFullAtMaxThreshold()
    {
        Assert.True(AutoPotionRule.ShouldDrink(99.9f, 100f, 255f));
    }

    [Fact]
    public void ShouldDrink_ReturnsFalse_WhenFullAtMaxThreshold()
    {
        Assert.False(AutoPotionRule.ShouldDrink(100f, 100f, 255f));
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(-5f)]
    public void ShouldDrink_ReturnsFalse_WhenNoMaxHealth(float max)
    {
        Assert.False(AutoPotionRule.ShouldDrink(0f, max, 255f));
    }
}
