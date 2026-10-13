using LastEpoch_Hud.Scripts.Core.Items;

namespace LastEpoch_Hud.Tests.Core.Items;

public sealed class CraftingSlotAffixSlotsTests
{
    [Fact]
    public void NextPrefix_GivesZeroOneTwoThenNone()
    {
        CraftingSlotAffixSlots slots = new();

        int[] result =
        {
            slots.NextPrefix(),
            slots.NextPrefix(),
            slots.NextPrefix(),
            slots.NextPrefix(),
        };

        Assert.Equal(new[] { 0, 1, 2, -1 }, result);
    }

    [Fact]
    public void NextSuffix_GivesTwoThreeThenNone()
    {
        CraftingSlotAffixSlots slots = new();

        int[] result =
        {
            slots.NextSuffix(),
            slots.NextSuffix(),
            slots.NextSuffix(),
            slots.NextSuffix(),
        };

        Assert.Equal(new[] { 2, 3, -1, -1 }, result);
    }

    [Fact]
    public void Counters_AreIndependent()
    {
        CraftingSlotAffixSlots slots = new();

        int[] result =
        {
            slots.NextPrefix(),
            slots.NextSuffix(),
            slots.NextPrefix(),
            slots.NextSuffix(),
        };

        Assert.Equal(new[] { 0, 2, 1, 3 }, result);
    }

    [Fact]
    public void NewInstance_StartsClean()
    {
        CraftingSlotAffixSlots used = new();
        used.NextPrefix();
        used.NextSuffix();

        CraftingSlotAffixSlots fresh = new();

        Assert.Equal(0, fresh.NextPrefix());
        Assert.Equal(2, fresh.NextSuffix());
    }
}
