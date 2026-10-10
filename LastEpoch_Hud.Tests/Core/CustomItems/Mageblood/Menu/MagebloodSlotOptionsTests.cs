using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu;

public sealed class MagebloodSlotOptionsTests
{
    [Fact]
    public void Order_DefaultsThenCustomInFileOrder()
    {
        string[] file = ["Custom2", Default(3), "Custom1", Default(0)];

        Assert.Equal(
            new[] { Default(0), Default(3), "Custom2", "Custom1" },
            MagebloodSlotOptions.Order(file)
        );
    }

    [Fact]
    public void Order_Empty()
    {
        Assert.Empty(MagebloodSlotOptions.Order(Array.Empty<string>()));
    }

    [Fact]
    public void TryPickSwap_OtherFlask_TrueWithCurrent()
    {
        string[] names = ["F0", "F1", "F2"];

        Assert.True(MagebloodSlotOptions.TryPickSwap(names, 0, "F2", out string current));
        Assert.Equal("F0", current);
    }

    [Theory]
    [InlineData(3, 0, "F0")]
    [InlineData(3, 0, "Unknown")]
    [InlineData(3, 0, null)]
    [InlineData(3, -1, "F1")]
    [InlineData(3, 3, "F1")]
    [InlineData(6, 4, "F1")]
    public void TryPickSwap_Refused(int count, int slot, string chosen)
    {
        string[] names = Enumerable.Range(0, count).Select(i => $"F{i}").ToArray();

        Assert.False(MagebloodSlotOptions.TryPickSwap(names, slot, chosen, out string current));
        Assert.Null(current);
    }

    private static string Default(int index) => MagebloodConfigDefaults.Flasks[index].Name;
}
