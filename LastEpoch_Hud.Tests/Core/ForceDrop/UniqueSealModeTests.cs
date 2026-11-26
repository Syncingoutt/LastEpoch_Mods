using LastEpoch_Hud.Scripts.Core.ForceDrop;

namespace LastEpoch_Hud.Tests.Core.ForceDrop;

public class UniqueSealModeTests
{
    [Theory]
    [InlineData(7, ForceDropSeal.Regular)]
    [InlineData(7, ForceDropSeal.Primordial)]
    [InlineData(8, ForceDropSeal.Regular)]
    [InlineData(8, ForceDropSeal.Primordial)]
    [InlineData(9, ForceDropSeal.Regular)]
    [InlineData(9, ForceDropSeal.Primordial)]
    public void LegalUniqueSealsAreRejectedAtTheRequestBoundary(int rarity, ForceDropSeal seal)
    {
        var affix = new ResolvedForceDropAffix(
            123,
            seal == ForceDropSeal.Primordial ? 7 : 6,
            255,
            seal
        );
        Assert.Throws<ArgumentException>(() =>
            Request(rarity, ForceDropMode.Legal, new[] { affix })
        );
        Assert.False(
            ForceDropLegalRules.SlotAllowed(
                seal == ForceDropSeal.Regular ? 4 : 5,
                false,
                true,
                rarity == 8
            )
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(7)]
    [InlineData(8)]
    public void IllegalEquipmentSupportsFourAffixesAndThreeIndependentSeals(int rarity)
    {
        var affixes = Enumerable
            .Range(1, 4)
            .Select(id => new ResolvedForceDropAffix(id, 6, 255, ForceDropSeal.None))
            .Append(new ResolvedForceDropAffix(5, 7, 255, ForceDropSeal.Regular))
            .Append(new ResolvedForceDropAffix(6, 7, 255, ForceDropSeal.Primordial));
        var request = Request(rarity, ForceDropMode.Illegal, affixes);
        Assert.Equal(6, request.Affixes.Count);
        Assert.Equal(4, request.Affixes.Count(a => a.Seal == ForceDropSeal.None));
        Assert.Equal(ForceDropSeal.Corruption, request.Corruption.Seal);
        Assert.Equal(7, Assert.Single(request.Affixes, a => a.Seal == ForceDropSeal.Regular).Tier);
        Assert.Equal(0, request.LegendaryPotential);
    }

    [Theory]
    [InlineData(4, ForceDropMode.Legal, 7)]
    [InlineData(4, ForceDropMode.Illegal, 8)]
    [InlineData(0, ForceDropMode.Legal, 7)]
    [InlineData(0, ForceDropMode.Illegal, 8)]
    [InlineData(5, ForceDropMode.Legal, 8)]
    [InlineData(5, ForceDropMode.Illegal, 8)]
    public void RegularSealTierLimitFollowsCreationMode(
        int slot,
        ForceDropMode mode,
        int maximum
    ) => Assert.Equal(maximum, ForceDropModeRules.MaximumSlotTier(slot, mode));

    static ResolvedForceDrop Request(
        int rarity,
        ForceDropMode mode,
        IEnumerable<ResolvedForceDropAffix> affixes
    ) =>
        new(
            21,
            1,
            rarity >= 7 ? 123 : 0,
            rarity,
            0,
            4,
            0,
            true,
            new int[3],
            new int[8],
            affixes,
            Array.Empty<int>(),
            new ResolvedForceDropAffix(7, 6, 255, ForceDropSeal.Corruption),
            mode
        );
}
