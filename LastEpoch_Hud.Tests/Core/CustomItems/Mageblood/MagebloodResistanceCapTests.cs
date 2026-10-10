using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood;

public sealed class MagebloodResistanceCapTests
{
    [Theory]
    [InlineData(75f, 5f, 80f)]
    [InlineData(75f, 0f, 75f)]
    [InlineData(75f, -5f, 75f)]
    [InlineData(75f, float.NaN, 75f)]
    [InlineData(75f, float.PositiveInfinity, 75f)]
    [InlineData(98f, 5f, 100f)]
    [InlineData(100f, 5f, 100f)]
    [InlineData(120f, 5f, 120f)]
    public void Raise_PercentUnits_AddsBonusUpToCeiling(float baseCap, float bonus, float expected)
    {
        Assert.Equal(expected, MagebloodResistanceCap.Raise(baseCap, bonus));
    }

    [Theory]
    [InlineData(0.75f, 5f, 0.8f)]
    [InlineData(0.98f, 5f, 1f)]
    [InlineData(1f, 5f, 1f)]
    public void Raise_FractionUnits_AddsBonusUpToCeiling(float baseCap, float bonus, float expected)
    {
        Assert.Equal(expected, MagebloodResistanceCap.Raise(baseCap, bonus), 1e-5f);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(-1f)]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    public void Raise_UnusableBase_Unchanged(float baseCap)
    {
        float result = MagebloodResistanceCap.Raise(baseCap, 5f);

        Assert.Equal(baseCap, result);
    }
}
