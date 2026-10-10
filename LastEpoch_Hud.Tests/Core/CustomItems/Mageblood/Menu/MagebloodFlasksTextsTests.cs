using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu;

public sealed class MagebloodFlasksTextsTests
{
    private static readonly Dictionary<string, string> _texts = new()
    {
        [MagebloodFlasksTexts.Slot] = "S{0}",
        [MagebloodFlasksTexts.SlotInactive] = "I{0}",
    };

    [Fact]
    public void SlotLabel_FillsOneBased()
    {
        Assert.Equal("S1", MagebloodFlasksTexts.SlotLabel(_texts, 0, false));
    }

    [Fact]
    public void SlotLabel_Inactive_UsesInactiveKey()
    {
        Assert.Equal("I4", MagebloodFlasksTexts.SlotLabel(_texts, 3, true));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SlotLabel_MissingKey_Null(bool inactive)
    {
        Assert.Null(MagebloodFlasksTexts.SlotLabel(new Dictionary<string, string>(), 0, inactive));
    }

    [Fact]
    public void All_HasEveryKeyOnce()
    {
        string[] expected =
        [
            MagebloodFlasksTexts.CardTitle,
            MagebloodFlasksTexts.Slot,
            MagebloodFlasksTexts.SlotInactive,
            MagebloodFlasksTexts.LeftmostHelp,
            MagebloodFlasksTexts.NoFlasks,
        ];

        Assert.Equal(expected.Order(), MagebloodFlasksTexts.All.Order());
        Assert.Equal(expected.Length, MagebloodFlasksTexts.All.Distinct().Count());
    }
}
