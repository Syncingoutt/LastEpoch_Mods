using LastEpoch_Hud.Scripts.Core.CustomItems.Affixes;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Affixes;

public sealed class CustomUniqueAffixesTests
{
    private const CustomItemTag N = CustomItemTag.None;
    private const CustomItemModType Add = CustomItemModType.Added;
    private const CustomItemModType Inc = CustomItemModType.Increased;

    [Fact]
    public void HeadhunterImplicits_HoldTodaysValues()
    {
        CustomBaseImplicit[] expected = [new(CustomItemStat.Health, N, Add, 0, 25f, 40f)];

        Assert.Equal(expected, CustomUniqueAffixes.HeadhunterImplicits);
    }

    [Fact]
    public void HeadhunterMods_HoldTodaysValues()
    {
        CustomUniqueMod[] expected =
        [
            new(CustomItemStat.Strength, N, Add, 40f, 55f, true),
            new(CustomItemStat.Dexterity, N, Add, 40f, 55f, true),
            new(CustomItemStat.Health, N, Add, 50f, 60f, true),
            new(CustomItemStat.Damage, N, Inc, 0.2f, 0.3f, true),
        ];

        Assert.Equal(expected, CustomUniqueAffixes.HeadhunterMods);
    }

    [Fact]
    public void HeadhunterTooltip_HoldsTodaysOrder()
    {
        byte[] expected = [0, 1, 2, 3, 128];

        Assert.Equal(expected, CustomUniqueAffixes.HeadhunterTooltip);
    }

    [Fact]
    public void SandsOfSilkImplicits_HoldTodaysValues()
    {
        CustomBaseImplicit[] expected = [new(CustomItemStat.DodgeRating, N, Add, 0, 153f, 204f)];

        Assert.Equal(expected, CustomUniqueAffixes.SandsOfSilkImplicits);
    }

    [Fact]
    public void SandsOfSilkMods_HoldTodaysValues()
    {
        CustomUniqueMod[] expected =
        [
            new(CustomItemStat.DodgeRating, N, Inc, 0.5f, 1f, true),
            new(CustomItemStat.Mana, N, Add, 50f, 80f, true),
            new(CustomItemStat.Dexterity, N, Add, 10f, 20f, true),
            new(CustomItemStat.Intelligence, N, Add, 10f, 20f, true),
            new(CustomItemStat.FireResistance, N, Inc, 0.1f, 0.15f, true),
            new(CustomItemStat.IncreasedCooldownRecoverySpeed, N, Inc, 0.15f, 0.3f, true),
        ];

        Assert.Equal(expected, CustomUniqueAffixes.SandsOfSilkMods);
    }

    [Fact]
    public void SandsOfSilkTooltip_HoldsTodaysOrder()
    {
        byte[] expected = [0, 1, 2, 3, 4, 5];

        Assert.Equal(expected, CustomUniqueAffixes.SandsOfSilkTooltip);
    }

    [Fact]
    public void MjolnerMods_HoldTodaysValues()
    {
        CustomUniqueMod[] expected =
        [
            new(CustomItemStat.Damage, CustomItemTag.Lightning, Inc, 0.8f, 1.0f, true),
            new(CustomItemStat.Damage, CustomItemTag.Physical, Inc, 0.8f, 1.2f, true),
        ];

        Assert.Equal(expected, CustomUniqueAffixes.MjolnerMods);
    }

    [Fact]
    public void MjolnerTooltip_HoldsTodaysValues()
    {
        byte[] expected = [0, 1, 128];

        Assert.Equal(expected, CustomUniqueAffixes.MjolnerTooltip);
    }

    [Fact]
    public void EssentiaSanguisImplicits_HoldTodaysValues()
    {
        CustomBaseImplicit[] expected = [new(CustomItemStat.DodgeRating, N, Add, 0, 50f, 50f)];

        Assert.Equal(expected, CustomUniqueAffixes.EssentiaSanguisImplicits);
    }

    [Fact]
    public void EssentiaSanguisMods_HoldTodaysValues()
    {
        CustomUniqueMod[] expected =
        [
            new(CustomItemStat.DodgeRating, N, Inc, 0.5f, 0.7f, true),
            new(CustomItemStat.HealthLeech, N, Add, 0.5f, 1f, true),
            new(CustomItemStat.Damage, CustomItemTag.Lightning, Add, 30f, 50f, true),
            new(CustomItemStat.Intelligence, N, Add, 15f, 25f, true),
            new(CustomItemStat.LightningResistance, N, Inc, 0.25f, 0.35f, true),
        ];

        Assert.Equal(expected, CustomUniqueAffixes.EssentiaSanguisMods);
    }

    [Fact]
    public void EssentiaSanguisTooltip_HoldsTodaysOrder()
    {
        byte[] expected = [0, 1, 2, 3, 4, 128];

        Assert.Equal(expected, CustomUniqueAffixes.EssentiaSanguisTooltip);
    }

    [Fact]
    public void MagebloodImplicits_HoldTodaysValues()
    {
        CustomBaseImplicit[] expected =
        [
            new(CustomItemStat.Strength, N, Add, 0, 25f, 35f),
            new(CustomItemStat.Dexterity, N, Add, 0, 30f, 50f),
        ];

        Assert.Equal(expected, CustomUniqueAffixes.MagebloodImplicits);
    }

    [Fact]
    public void MagebloodMods_AreEmpty()
    {
        Assert.Empty(CustomUniqueAffixes.MagebloodMods);
    }

    [Fact]
    public void MagebloodTooltip_IsDescriptionOnly()
    {
        byte[] expected = [128];

        Assert.Equal(expected, CustomUniqueAffixes.MagebloodTooltip);
    }

    [Fact]
    public void AllImplicits_ValueNotAboveMax()
    {
        IEnumerable<CustomBaseImplicit> all = CustomUniqueAffixes
            .HeadhunterImplicits.Concat(CustomUniqueAffixes.SandsOfSilkImplicits)
            .Concat(CustomUniqueAffixes.EssentiaSanguisImplicits);

        Assert.All(all, implicitStat => Assert.True(implicitStat.Value <= implicitStat.MaxValue));
    }

    [Fact]
    public void AllMods_ValueNotAboveMax()
    {
        IEnumerable<CustomUniqueMod> all = CustomUniqueAffixes
            .HeadhunterMods.Concat(CustomUniqueAffixes.SandsOfSilkMods)
            .Concat(CustomUniqueAffixes.MjolnerMods)
            .Concat(CustomUniqueAffixes.EssentiaSanguisMods);

        Assert.All(all, mod => Assert.True(mod.Value <= mod.MaxValue));
    }

    [Fact]
    public void DescriptionEntry_Is128()
    {
        Assert.Equal(128, CustomUniqueAffixes.DescriptionEntry);
    }
}
