using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu;

public sealed class MagebloodMenuTextsTests
{
    private static readonly Dictionary<string, string> _texts = new()
    {
        [MagebloodMenuTexts.WornActiveFlasks] = "W{0}",
        [MagebloodMenuTexts.NotWorn] = "N",
        [MagebloodMenuTexts.FileError] = "E",
        [MagebloodMenuTexts.FileSkipped] = "S",
    };

    [Fact]
    public void Status_Worn_FillsCount()
    {
        var status = new MagebloodMenuStatus(true, 3, MagebloodFileWarning.None, true);

        Assert.Equal("W3", MagebloodMenuTexts.Status(_texts, status));
    }

    [Fact]
    public void Status_NotWorn_NotWornText()
    {
        var status = new MagebloodMenuStatus(false, 0, MagebloodFileWarning.None, true);

        Assert.Equal("N", MagebloodMenuTexts.Status(_texts, status));
    }

    [Fact]
    public void Status_MissingKey_Null()
    {
        var status = new MagebloodMenuStatus(true, 3, MagebloodFileWarning.None, true);

        Assert.Null(MagebloodMenuTexts.Status(new Dictionary<string, string>(), status));
    }

    [Theory]
    [InlineData(MagebloodFileWarning.None, null)]
    [InlineData(MagebloodFileWarning.FileError, "E")]
    [InlineData(MagebloodFileWarning.SomeSkipped, "S")]
    public void Warning_PerKind(MagebloodFileWarning warning, string expected)
    {
        Assert.Equal(expected, MagebloodMenuTexts.Warning(_texts, warning));
    }

    [Fact]
    public void Warning_MissingKey_Null()
    {
        Assert.Null(
            MagebloodMenuTexts.Warning(
                new Dictionary<string, string>(),
                MagebloodFileWarning.FileError
            )
        );
    }

    [Fact]
    public void RestoreLabel_PicksKey()
    {
        Assert.Equal(MagebloodMenuTexts.ConfirmRestore, MagebloodMenuTexts.RestoreLabel(true));
        Assert.Equal(MagebloodMenuTexts.RestoreDefaults, MagebloodMenuTexts.RestoreLabel(false));
    }

    [Fact]
    public void All_HasEveryKeyOnce()
    {
        string[] expected =
        [
            MagebloodMenuTexts.CardTitle,
            MagebloodMenuTexts.MaxResistances,
            MagebloodMenuTexts.RestoreDefaults,
            MagebloodMenuTexts.ConfirmRestore,
            MagebloodMenuTexts.WornActiveFlasks,
            MagebloodMenuTexts.NotWorn,
            MagebloodMenuTexts.FileError,
            MagebloodMenuTexts.FileSkipped,
            MagebloodMenuTexts.BackupSaved,
            MagebloodMenuTexts.TooltipRestart,
        ];

        Assert.Equal(expected.Order(), MagebloodMenuTexts.All.Order());
        Assert.Equal(expected.Length, MagebloodMenuTexts.All.Distinct().Count());
    }
}
