using LastEpoch_Hud.Scripts.Core.ModUI;

namespace LastEpoch_Hud.Tests.Core.ModUI;

public sealed class DropdownIndexTests
{
    [Theory]
    [InlineData(0, 3)]
    [InlineData(2, 3)]
    public void OrFirst_KeepsInRange(int index, int count)
    {
        Assert.Equal(index, DropdownIndex.OrFirst(index, count));
    }

    [Theory]
    [InlineData(3, 3)]
    [InlineData(-1, 3)]
    [InlineData(0, 0)]
    public void OrFirst_OutOfRangeGivesFirst(int index, int count)
    {
        Assert.Equal(0, DropdownIndex.OrFirst(index, count));
    }
}
