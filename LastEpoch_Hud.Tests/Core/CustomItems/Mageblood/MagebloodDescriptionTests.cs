using LastEpoch_Hud.Scripts.Core.CustomItems;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood;

public sealed class MagebloodDescriptionTests
{
    private static readonly Dictionary<string, string> _texts = new()
    {
        [CustomItemLocaleKeys.MagebloodDescription] = "a {0} b",
    };

    [Fact]
    public void Text_Template_FilledWithRangeText()
    {
        Assert.Equal("a (2-4) b", MagebloodDescription.Text(_texts));
    }

    [Fact]
    public void Text_MissingTemplate_ReturnsNull()
    {
        Assert.Null(MagebloodDescription.Text(new Dictionary<string, string>()));
    }

    [Fact]
    public void Text_NullTexts_ReturnsNull()
    {
        Assert.Null(MagebloodDescription.Text(null));
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
