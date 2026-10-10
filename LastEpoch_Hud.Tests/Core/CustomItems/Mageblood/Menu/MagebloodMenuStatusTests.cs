using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu;

public sealed class MagebloodMenuStatusTests
{
    [Fact]
    public void From_Unreadable_FileErrorNotEditable()
    {
        Assert.Equal(
            new MagebloodMenuStatus(true, 2, MagebloodFileWarning.FileError, false),
            MagebloodMenuStatus.From(false, 3, true, 2)
        );
    }

    [Fact]
    public void From_Problems_SomeSkipped()
    {
        Assert.Equal(
            new MagebloodMenuStatus(true, 2, MagebloodFileWarning.SomeSkipped, true),
            MagebloodMenuStatus.From(true, 1, true, 2)
        );
    }

    [Fact]
    public void From_Clean_None()
    {
        Assert.Equal(
            new MagebloodMenuStatus(true, 2, MagebloodFileWarning.None, true),
            MagebloodMenuStatus.From(true, 0, true, 2)
        );
    }

    [Fact]
    public void From_NotWorn_ZeroActive()
    {
        Assert.Equal(0, MagebloodMenuStatus.From(true, 0, false, 5).ActiveFlasks);
    }

    [Fact]
    public void From_Worn_KeepsActive()
    {
        Assert.Equal(5, MagebloodMenuStatus.From(true, 0, true, 5).ActiveFlasks);
    }

    [Fact]
    public void IsSlotInactive_NotWorn_False()
    {
        var status = new MagebloodMenuStatus(false, 0, MagebloodFileWarning.None, true);

        Assert.False(status.IsSlotInactive(3));
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void IsSlotInactive_Worn_FromActiveCount(int slot, bool expected)
    {
        var status = new MagebloodMenuStatus(true, 2, MagebloodFileWarning.None, true);

        Assert.Equal(expected, status.IsSlotInactive(slot));
    }

    [Fact]
    public void From_NegativeActive_Zero()
    {
        Assert.Equal(0, MagebloodMenuStatus.From(true, 0, true, -3).ActiveFlasks);
    }
}
