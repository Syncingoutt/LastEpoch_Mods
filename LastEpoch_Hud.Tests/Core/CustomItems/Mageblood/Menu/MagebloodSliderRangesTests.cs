using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu;

public sealed class MagebloodSliderRangesTests
{
    [Theory]
    [InlineData("Movespeed", MagebloodStatField.Increased, false, 30f, 1f, 100f)]
    [InlineData("AttackSpeed", MagebloodStatField.Increased, false, 20f, 1f, 100f)]
    [InlineData("CastSpeed", MagebloodStatField.Increased, false, 20f, 1f, 100f)]
    [InlineData("CriticalChance", MagebloodStatField.Increased, false, 100f, 1f, 300f)]
    [InlineData("Damage", MagebloodStatField.Increased, false, 40f, 1f, 200f)]
    [InlineData("FakeStat", MagebloodStatField.Increased, false, 10f, 1f, 200f)]
    [InlineData("Armour", MagebloodStatField.Added, false, 500f, 10f, 2000f)]
    [InlineData("DodgeRating", MagebloodStatField.Added, false, 500f, 10f, 2000f)]
    [InlineData("VoidResistance", MagebloodStatField.Added, true, 35f, 1f, 75f)]
    [InlineData("AllResistances", MagebloodStatField.Added, true, 35f, 1f, 75f)]
    [InlineData("IncreasedDropRate", MagebloodStatField.Added, true, 20f, 1f, 100f)]
    [InlineData("FakeStat", MagebloodStatField.Added, true, 10f, 1f, 100f)]
    [InlineData("FakeStat", MagebloodStatField.Added, false, 50f, 1f, 1000f)]
    [InlineData("FakeStat", MagebloodStatField.More, false, -15f, -50f, -1f)]
    [InlineData("FakeStat", MagebloodStatField.More, false, 20f, 1f, 200f)]
    public void ForStat_BaseTable(
        string stat,
        MagebloodStatField field,
        bool addedAsPercent,
        float value,
        float min,
        float max
    )
    {
        Assert.Equal(
            new MagebloodSliderRange(min, max, true),
            MagebloodSliderRanges.ForStat(stat, field, addedAsPercent, value)
        );
    }

    [Fact]
    public void ForStat_StretchesMax()
    {
        Assert.Equal(
            new MagebloodSliderRange(10f, 2501f, false),
            MagebloodSliderRanges.ForStat("Armour", MagebloodStatField.Added, false, 2500.5f)
        );
    }

    [Fact]
    public void ForStat_StretchesMax_More()
    {
        Assert.Equal(
            new MagebloodSliderRange(1f, 251f, false),
            MagebloodSliderRanges.ForStat("FakeStat", MagebloodStatField.More, false, 250.2f)
        );
    }

    [Fact]
    public void ForStat_StretchesMinPositive()
    {
        Assert.Equal(
            new MagebloodSliderRange(0.5f, 75f, false),
            MagebloodSliderRanges.ForStat("AllResistances", MagebloodStatField.Added, true, 0.5f)
        );
    }

    [Fact]
    public void ForStat_NegativeStretch_Min()
    {
        Assert.Equal(
            new MagebloodSliderRange(-80f, -1f, true),
            MagebloodSliderRanges.ForStat("FakeStat", MagebloodStatField.More, false, -80f)
        );
    }

    [Fact]
    public void ForStat_NegativeStretch_MinFloors()
    {
        Assert.Equal(
            new MagebloodSliderRange(-81f, -1f, false),
            MagebloodSliderRanges.ForStat("FakeStat", MagebloodStatField.More, false, -80.5f)
        );
    }

    [Theory]
    [InlineData(MagebloodStatField.Increased, false)]
    [InlineData(MagebloodStatField.Added, false)]
    [InlineData(MagebloodStatField.Added, true)]
    public void ForStat_NegativeValue_StaysBelowZero(MagebloodStatField field, bool addedAsPercent)
    {
        Assert.Equal(
            new MagebloodSliderRange(-50f, -1f, true),
            MagebloodSliderRanges.ForStat("FakeStat", field, addedAsPercent, -10f)
        );
    }

    [Fact]
    public void ForStat_NegativeStretch_MaxNeverZero()
    {
        Assert.Equal(
            new MagebloodSliderRange(-50f, -0.5f, false),
            MagebloodSliderRanges.ForStat("FakeStat", MagebloodStatField.More, false, -0.5f)
        );
    }

    [Theory]
    [InlineData(float.NaN)]
    [InlineData(float.PositiveInfinity)]
    [InlineData(float.NegativeInfinity)]
    [InlineData(0f)]
    public void ForStat_NonFiniteOrZero_Base(float value)
    {
        Assert.Equal(
            new MagebloodSliderRange(10f, 2000f, true),
            MagebloodSliderRanges.ForStat("Armour", MagebloodStatField.Added, false, value)
        );
    }

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
