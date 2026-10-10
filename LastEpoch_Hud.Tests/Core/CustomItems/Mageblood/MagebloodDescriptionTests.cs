using System.Globalization;
using LastEpoch_Hud.Scripts.Core.CustomItems;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood;

public sealed class MagebloodDescriptionTests
{
    private static readonly Dictionary<string, string> _texts = new()
    {
        [CustomItemLocaleKeys.MagebloodDescription] = "a {0} b {1}",
    };

    [Fact]
    public void Text_FilledWithRangeAndBonus()
    {
        Assert.Equal("a (2-4) b 7.5", MagebloodDescription.Text(_texts, 7.5f));
    }

    [Fact]
    public void Text_WholeBonus_NoDecimals()
    {
        Assert.Equal("a (2-4) b 5", MagebloodDescription.Text(_texts, 5f));
    }

    [Fact]
    public void Text_CommaCulture_UsesDot()
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("de-DE");
        try
        {
            Assert.Equal("a (2-4) b 7.5", MagebloodDescription.Text(_texts, 7.5f));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void Text_MissingTemplate_ReturnsNull()
    {
        Assert.Null(MagebloodDescription.Text(new Dictionary<string, string>(), 5f));
    }

    [Fact]
    public void Text_NullTexts_ReturnsNull()
    {
        Assert.Null(MagebloodDescription.Text(null, 5f));
    }

    [Fact]
    public void WithSlots_RangeText_ReplacedBySlots()
    {
        Assert.Equal("a 3 b", MagebloodDescription.WithSlots("a (2-4) b", 3));
    }

    [Fact]
    public void WithSlots_NullText_ReturnsNull()
    {
        Assert.Null(MagebloodDescription.WithSlots(null, 3));
    }

    [Fact]
    public void WithSlots_TextWithoutRange_Unchanged()
    {
        Assert.Equal("plain", MagebloodDescription.WithSlots("plain", 3));
    }
}
