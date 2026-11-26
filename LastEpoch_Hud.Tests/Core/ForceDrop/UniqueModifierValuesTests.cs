using LastEpoch_Hud.Scripts.Core.ForceDrop;

namespace LastEpoch_Hud.Tests.Core.ForceDrop;

public sealed class UniqueModifierValuesTests
{
    static ResolvedForceDrop Request(
        int[] tiers = null,
        int[] rolls = null,
        ResolvedForceDropAffix[] affixes = null,
        ForceDropMode mode = ForceDropMode.Legal
    ) =>
        new(
            14,
            0,
            420,
            7,
            0,
            4,
            10,
            true,
            Array.Empty<int>(),
            Array.Empty<int>(),
            affixes ?? Array.Empty<ResolvedForceDropAffix>(),
            new[] { 100, 101 },
            null,
            mode,
            tiers,
            rolls
        );

    [Fact]
    public void ExistingFixedModifiersRetainTheirDefaultValues()
    {
        var request = Request();
        Assert.Equal(new[] { 0, 0 }, request.VariantTiers);
        Assert.Equal(new[] { 255, 255 }, request.VariantRolls);
    }

    [Fact]
    public void TwoTieredModifiersKeepIndependentImmutableValues()
    {
        int[] tiers =  { 6, 2 },
            rolls =  { 255, 38 };
        var request = Request(tiers, rolls);
        tiers[0] = 0;
        rolls[1] = 255;
        Assert.Equal(new[] { 6, 2 }, request.VariantTiers);
        Assert.Equal(new[] { 255, 38 }, request.VariantRolls);
        Assert.Equal(0, request.LegendaryPotential);
        Assert.Equal(0, request.WeaversWill);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public void IllegalTieredPoolAllowsRegularSealWithinSavedCapacity(int ordinaryCount)
    {
        var affixes = Enumerable
            .Range(1, ordinaryCount)
            .Select(id => new ResolvedForceDropAffix(id, 6, 255, ForceDropSeal.None))
            .Append(new ResolvedForceDropAffix(20, 7, 255, ForceDropSeal.Regular))
            .ToArray();
        var request = Request(new[] { 6, 2 }, new[] { 255, 38 }, affixes, ForceDropMode.Illegal);
        Assert.Equal(ordinaryCount + 3, request.Affixes.Count + request.VariantIds.Count);
        Assert.Contains(request.Affixes, a => a.Seal == ForceDropSeal.Regular && a.Tier == 7);
        Assert.Null(request.Corruption);
        Assert.True(request.Corrupted);
    }

    [Fact]
    public void TwoNativeModifiersPlusFourOrdinaryAndBothSealsExceedSavedCapacity()
    {
        var affixes = Enumerable
            .Range(1, 4)
            .Select(id => new ResolvedForceDropAffix(id, 6, 255, ForceDropSeal.None))
            .Append(new ResolvedForceDropAffix(20, 7, 255, ForceDropSeal.Regular))
            .Append(new ResolvedForceDropAffix(21, 7, 255, ForceDropSeal.Primordial))
            .ToArray();
        var error = Assert.Throws<ArgumentException>(() =>
            Request(new[] { 6, 2 }, new[] { 255, 38 }, affixes, ForceDropMode.Illegal)
        );
        Assert.Contains("at most seven affixes", error.Message);
    }

    [Fact]
    public void MissingModifierValueIsRejected() =>
        Assert.Throws<ArgumentException>(() => Request(new[] { 6 }, new[] { 255, 255 }));

    [Theory]
    [InlineData(-1)]
    [InlineData(7)]
    public void DedicatedTierCannotExceedNativeT7(int tier) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Request(new[] { tier, 0 }));

    [Theory]
    [InlineData(-1)]
    [InlineData(256)]
    public void ModifierRollMustFitNativeByte(int roll) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Request(null, new[] { roll, 255 }));
}
