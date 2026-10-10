using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood;

public sealed class MagebloodFlaskNamesTests
{
    private static string FirstName => MagebloodConfigDefaults.Flasks[0].Name;

    [Fact]
    public void KeyFor_EveryDefault_IsPrefixPlusName()
    {
        foreach (MagebloodFlaskEntry flask in MagebloodConfigDefaults.Flasks)
        {
            Assert.Equal(
                MagebloodFlaskNames.KeyPrefix + flask.Name,
                MagebloodFlaskNames.KeyFor(flask.Name)
            );
        }
    }

    [Fact]
    public void KeyFor_CustomName_IsNull()
    {
        Assert.Null(MagebloodFlaskNames.KeyFor("My Flask"));
    }

    [Fact]
    public void KeyFor_OtherCase_IsNull()
    {
        Assert.Null(MagebloodFlaskNames.KeyFor(FirstName.ToLowerInvariant()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void KeyFor_NullOrEmpty_IsNull(string name)
    {
        Assert.Null(MagebloodFlaskNames.KeyFor(name));
    }

    [Fact]
    public void Display_DefaultWithText_IsTranslated()
    {
        var texts = new Dictionary<string, string>
        {
            [MagebloodFlaskNames.KeyFor(FirstName)] = "T1",
        };

        Assert.Equal("T1", MagebloodFlaskNames.Display(texts, FirstName));
    }

    [Fact]
    public void Display_KeyMissing_IsName()
    {
        Assert.Equal(
            FirstName,
            MagebloodFlaskNames.Display(new Dictionary<string, string>(), FirstName)
        );
    }

    [Fact]
    public void Display_EmptyValue_IsName()
    {
        var texts = new Dictionary<string, string> { [MagebloodFlaskNames.KeyFor(FirstName)] = "" };

        Assert.Equal(FirstName, MagebloodFlaskNames.Display(texts, FirstName));
    }

    [Fact]
    public void Display_NullTexts_IsName()
    {
        Assert.Equal(FirstName, MagebloodFlaskNames.Display(null, FirstName));
    }

    [Fact]
    public void Display_CustomName_AsWritten()
    {
        var texts = new Dictionary<string, string>
        {
            [MagebloodFlaskNames.KeyPrefix + "My Flask"] = "T1",
        };

        Assert.Equal("My Flask", MagebloodFlaskNames.Display(texts, "My Flask"));
    }

    [Fact]
    public void Keys_OnePerDefault_InOrder()
    {
        IEnumerable<string> expected = MagebloodConfigDefaults.Flasks.Select(f =>
            MagebloodFlaskNames.KeyPrefix + f.Name
        );

        Assert.Equal(expected, MagebloodFlaskNames.Keys);
    }
}
