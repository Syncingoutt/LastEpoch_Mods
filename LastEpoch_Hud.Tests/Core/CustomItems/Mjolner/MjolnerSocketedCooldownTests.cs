using LastEpoch_Hud.Scripts.Core.CustomItems.Mjolner;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mjolner;

public sealed class MjolnerSocketedCooldownTests
{
    [Theory]
    [InlineData(100f)]
    [InlineData(0f)]
    [InlineData(-5f)]
    [InlineData(float.NaN)]
    public void Milliseconds_BelowMinimum_UsesMinimum(float configured)
    {
        Assert.Equal(250d, MjolnerSocketedCooldown.Milliseconds(configured));
    }

    [Theory]
    [InlineData(250f)]
    [InlineData(400f)]
    public void Milliseconds_AtOrAbove_KeepsValue(float configured)
    {
        Assert.Equal((double)configured, MjolnerSocketedCooldown.Milliseconds(configured));
    }
}
