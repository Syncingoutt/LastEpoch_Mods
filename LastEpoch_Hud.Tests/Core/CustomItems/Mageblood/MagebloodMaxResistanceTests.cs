using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood;

public sealed class MagebloodMaxResistanceTests
{
    private const float Tolerance = 1e-4f;
    private const float Damage = 100f;

    [Theory]
    [InlineData(5f, 0.80f)]
    [InlineData(2.5f, 0.775f)]
    [InlineData(0f, 0.75f)]
    [InlineData(-5f, 0.75f)]
    [InlineData(25f, 1f)]
    [InlineData(40f, 1f)]
    public void RaisedCap_Bonus_AddsToGameCapWithinLimits(float bonus, float expected)
    {
        Assert.Equal(expected, MagebloodMaxResistance.RaisedCap(bonus), Tolerance);
    }

    [Theory]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    public void RaisedCap_Unusable_GameCap(float bonus)
    {
        Assert.Equal(0.75f, MagebloodMaxResistance.RaisedCap(bonus), Tolerance);
    }

    [Fact]
    public void Rescale_OverRaisedCap_UsesRaisedCap()
    {
        float result = MagebloodMaxResistance.Rescale(GameResisted(0.9f, 0f), Damage, 0.9f, 5f);

        Assert.Equal(20f, result, Tolerance);
    }

    [Fact]
    public void Rescale_BetweenCaps_UsesUncapped()
    {
        float result = MagebloodMaxResistance.Rescale(GameResisted(0.78f, 0f), Damage, 0.78f, 5f);

        Assert.Equal(22f, result, Tolerance);
    }

    [Theory]
    [InlineData(0.75f)]
    [InlineData(0.5f)]
    [InlineData(0f)]
    [InlineData(-0.3f)]
    public void Rescale_AtOrBelowGameCap_Unchanged(float uncapped)
    {
        float resisted = GameResisted(uncapped, 0.05f);

        Assert.Equal(resisted, MagebloodMaxResistance.Rescale(resisted, Damage, uncapped, 5f));
    }

    [Theory]
    [InlineData(0.1f, 30f)]
    [InlineData(0.3f, 50f)]
    public void Rescale_Penetration_StaysAfterCap(float penetration, float expected)
    {
        float resisted = GameResisted(0.9f, penetration);

        float result = MagebloodMaxResistance.Rescale(resisted, Damage, 0.9f, 5f);

        Assert.Equal(expected, result, Tolerance);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(-5f)]
    [InlineData(float.NaN)]
    public void Rescale_UnusableBonus_Unchanged(float bonus)
    {
        float resisted = GameResisted(0.9f, 0f);

        Assert.Equal(resisted, MagebloodMaxResistance.Rescale(resisted, Damage, 0.9f, bonus));
    }

    [Fact]
    public void Rescale_FullCap_Immune()
    {
        float result = MagebloodMaxResistance.Rescale(GameResisted(1.2f, 0f), Damage, 1.2f, 25f);

        Assert.Equal(0f, result, Tolerance);
    }

    [Fact]
    public void Rescale_NegativePenetration_NeverBelowZero()
    {
        float resisted = GameResisted(1f, -0.2f);

        float result = MagebloodMaxResistance.Rescale(resisted, Damage, 1f, 25f);

        Assert.Equal(0f, result, Tolerance);
    }

    [Fact]
    public void Rescale_NaNUncapped_Unchanged()
    {
        Assert.Equal(25f, MagebloodMaxResistance.Rescale(25f, Damage, float.NaN, 5f));
    }

    [Fact]
    public void Rescale_ZeroDamage_Zero()
    {
        Assert.Equal(0f, MagebloodMaxResistance.Rescale(0f, 0f, 0.9f, 5f), Tolerance);
    }

    [Fact]
    public void AppliesTo_PlayerInstance_True()
    {
        Assert.True(MagebloodMaxResistance.AppliesTo(new IntPtr(1), new IntPtr(1)));
    }

    [Fact]
    public void AppliesTo_OtherInstance_False()
    {
        Assert.False(MagebloodMaxResistance.AppliesTo(new IntPtr(2), new IntPtr(1)));
    }

    [Fact]
    public void AppliesTo_NoPlayer_False()
    {
        Assert.False(MagebloodMaxResistance.AppliesTo(IntPtr.Zero, IntPtr.Zero));
    }

    private static float GameResisted(float uncapped, float penetration) =>
        Damage * (1f - Math.Min(uncapped, 0.75f) + penetration);
}
