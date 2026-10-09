using LastEpoch_Hud.Scripts.Core.CustomItems.Affixes;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood;

public sealed class MagebloodFlaskSlotsTests
{
    [Theory]
    [InlineData(0, 2)]
    [InlineData(85, 2)]
    [InlineData(86, 3)]
    [InlineData(170, 3)]
    [InlineData(171, 4)]
    [InlineData(255, 4)]
    public void FromRoll_Boundaries(int roll, int expected)
    {
        Assert.Equal(expected, MagebloodFlaskSlots.FromRoll((byte)roll));
    }

    [Fact]
    public void FromRoll_AllRolls_SpreadEvenly()
    {
        var counts = Enumerable
            .Range(0, 256)
            .GroupBy(roll => MagebloodFlaskSlots.FromRoll((byte)roll))
            .ToDictionary(group => group.Key, group => group.Count());

        Assert.Equal(
            new[] { MagebloodFlaskSlots.Min, 3, MagebloodFlaskSlots.Max },
            counts.Keys.OrderBy(slots => slots)
        );
        Assert.All(counts.Values, count => Assert.InRange(count, 85, 86));
    }

    [Fact]
    public void RangeText_ShowsMinAndMax()
    {
        Assert.Equal(
            MagebloodFlaskSlots.RangeText,
            $"({MagebloodFlaskSlots.Min}-{MagebloodFlaskSlots.Max})"
        );
    }

    [Fact]
    public void RollIndex_IsZero()
    {
        Assert.Equal(0, MagebloodFlaskSlots.RollIndex);
    }

    [Fact]
    public void RollIndex_NotUsedByUniqueMods()
    {
        Assert.True(MagebloodFlaskSlots.RollIndex >= CustomUniqueAffixes.MagebloodMods.Count);
    }

    [Fact]
    public void TryFromRolls_NullRolls_FalseAndZero()
    {
        bool ok = MagebloodFlaskSlots.TryFromRolls(null, out int slots);

        Assert.False(ok);
        Assert.Equal(0, slots);
    }

    [Fact]
    public void TryFromRolls_Empty_FalseAndZero()
    {
        bool ok = MagebloodFlaskSlots.TryFromRolls(Array.Empty<byte>(), out int slots);

        Assert.False(ok);
        Assert.Equal(0, slots);
    }

    [Theory]
    [InlineData(171, 0, 4)]
    [InlineData(0, 255, 2)]
    public void TryFromRolls_RollAtIndex_ReturnsFromRoll(int first, int second, int expected)
    {
        byte[] rolls = new byte[] { (byte)first, (byte)second, 0 };

        bool ok = MagebloodFlaskSlots.TryFromRolls(rolls, out int slots);

        Assert.True(ok);
        Assert.Equal(expected, slots);
    }
}
