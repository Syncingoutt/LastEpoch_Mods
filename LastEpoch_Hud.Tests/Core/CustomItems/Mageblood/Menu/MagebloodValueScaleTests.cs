using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu;

public sealed class MagebloodValueScaleTests
{
    [Theory]
    [InlineData(MagebloodStatField.Added, false, MagebloodValueUnit.Raw)]
    [InlineData(MagebloodStatField.Added, true, MagebloodValueUnit.Percent)]
    [InlineData(MagebloodStatField.Increased, false, MagebloodValueUnit.Percent)]
    [InlineData(MagebloodStatField.Increased, true, MagebloodValueUnit.Percent)]
    [InlineData(MagebloodStatField.More, false, MagebloodValueUnit.Percent)]
    [InlineData(MagebloodStatField.More, true, MagebloodValueUnit.Percent)]
    public void Unit_PerField(
        MagebloodStatField field,
        bool addedAsPercent,
        MagebloodValueUnit expected
    )
    {
        Assert.Equal(expected, MagebloodValueScale.Unit(field, addedAsPercent));
    }

    [Theory]
    [InlineData(0.35f, 35f)]
    [InlineData(0.2f, 20f)]
    public void ToDisplay_AddedPercent_Times100(float file, float expected)
    {
        Assert.Equal(expected, MagebloodValueScale.ToDisplay(file, MagebloodStatField.Added, true));
    }

    [Theory]
    [InlineData(MagebloodStatField.Added, false, 500f)]
    [InlineData(MagebloodStatField.Increased, true, 30f)]
    [InlineData(MagebloodStatField.More, false, -15f)]
    public void ToDisplay_Others_Unchanged(
        MagebloodStatField field,
        bool addedAsPercent,
        float value
    )
    {
        Assert.Equal(value, MagebloodValueScale.ToDisplay(value, field, addedAsPercent));
    }

    [Fact]
    public void ToFile_AddedPercent_Div100()
    {
        Assert.Equal(0.35f, MagebloodValueScale.ToFile(35f, MagebloodStatField.Added, true));
    }

    [Theory]
    [InlineData(MagebloodStatField.Added, false, 500f)]
    [InlineData(MagebloodStatField.More, false, -15f)]
    public void ToFile_Others_Unchanged(MagebloodStatField field, bool addedAsPercent, float value)
    {
        Assert.Equal(value, MagebloodValueScale.ToFile(value, field, addedAsPercent));
    }

    [Theory]
    [InlineData(0.35f)]
    [InlineData(0.2f)]
    public void RoundTrip_Exact(float file)
    {
        float shown = MagebloodValueScale.ToDisplay(file, MagebloodStatField.Added, true);

        Assert.Equal(file, MagebloodValueScale.ToFile(shown, MagebloodStatField.Added, true));
    }

    [Theory]
    [InlineData(MagebloodStatField.Added, false, false)]
    [InlineData(MagebloodStatField.Added, true, true)]
    [InlineData(MagebloodStatField.Increased, false, true)]
    [InlineData(MagebloodStatField.More, false, true)]
    public void IsUnitKnown_PerField(MagebloodStatField field, bool statInfoRead, bool expected)
    {
        Assert.Equal(expected, MagebloodValueScale.IsUnitKnown(field, statInfoRead));
    }

    [Fact]
    public void TryEdit_SameValue_False()
    {
        Assert.False(MagebloodValueScale.TryEdit(35f, 35f, MagebloodStatField.Added, true, out _));
    }

    [Fact]
    public void TryEdit_Changed_ReturnsFileUnit()
    {
        Assert.True(
            MagebloodValueScale.TryEdit(40f, 35f, MagebloodStatField.Added, true, out float file)
        );
        Assert.Equal(0.4f, file);
    }
}
