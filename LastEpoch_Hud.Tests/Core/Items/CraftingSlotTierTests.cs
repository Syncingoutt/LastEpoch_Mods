using LastEpoch_Hud.Scripts.Core.Items;

namespace LastEpoch_Hud.Tests.Core.Items;

public sealed class CraftingSlotTierTests
{
    [Theory]
    [InlineData(255f, 6)]
    [InlineData(-1f, 0)]
    [InlineData(2.5f, 2)]
    [InlineData(3.5f, 4)]
    [InlineData(6f, 6)]
    [InlineData(4.4f, 4)]
    public void ToAffixTier_RoundsHalfEvenAndClamps(float value, int expected)
    {
        Assert.Equal((byte)expected, CraftingSlotTier.ToAffixTier(value));
    }

    [Theory]
    [InlineData(6, false)]
    [InlineData(7, true)]
    public void IsPrimordial_FromSeven(int tier, bool expected)
    {
        Assert.Equal(expected, CraftingSlotTier.IsPrimordial(tier));
    }
}
