using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Bar;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Bar;

public sealed class MagebloodIconChainTests
{
    [Fact]
    public void For_FullChain_Order()
    {
        IReadOnlyList<MagebloodIconSource> chain = MagebloodIconChain.For(
            "fakeicon",
            "Basalt",
            new[] { "StatA", "StatB" }
        );

        Assert.Equal(
            new[]
            {
                Flask("fakeicon"),
                Buff("fakeicon"),
                Buff("Armour"),
                Buff("StatA"),
                Buff("StatB"),
                Item(),
            },
            chain
        );
    }

    [Fact]
    public void For_BlankIcon_StartsWithDefaultOrStats()
    {
        IReadOnlyList<MagebloodIconSource> chain = MagebloodIconChain.For(
            " ",
            "Basalt",
            new[] { "StatA" }
        );

        Assert.Equal(new[] { Buff("Armour"), Buff("StatA"), Item() }, chain);
    }

    [Fact]
    public void For_NoDefault_NoStep()
    {
        IReadOnlyList<MagebloodIconSource> chain = MagebloodIconChain.For(
            "fakeicon",
            "FakeA",
            new[] { "StatA" }
        );

        Assert.Equal(new[] { Flask("fakeicon"), Buff("fakeicon"), Buff("StatA"), Item() }, chain);
    }

    [Fact]
    public void For_StatEqualToIconOrDefault_Skipped()
    {
        IReadOnlyList<MagebloodIconSource> chain = MagebloodIconChain.For(
            "fakeicon",
            "Basalt",
            new[] { "fakeicon", "Armour", "StatA" }
        );

        Assert.Equal(
            new[] { Flask("fakeicon"), Buff("fakeicon"), Buff("Armour"), Buff("StatA"), Item() },
            chain
        );
    }

    [Fact]
    public void For_IconEqualToDefault_Once()
    {
        IReadOnlyList<MagebloodIconSource> chain = MagebloodIconChain.For("Armour", "Basalt", null);

        Assert.Equal(new[] { Flask("Armour"), Buff("Armour"), Item() }, chain);
    }

    [Fact]
    public void For_DedupeIsCaseSensitive()
    {
        IReadOnlyList<MagebloodIconSource> chain = MagebloodIconChain.For(
            "fakeicon",
            "FakeA",
            new[] { "FAKEICON" }
        );

        Assert.Equal(
            new[] { Flask("fakeicon"), Buff("fakeicon"), Buff("FAKEICON"), Item() },
            chain
        );
    }

    [Fact]
    public void For_DuplicateStats_Once()
    {
        IReadOnlyList<MagebloodIconSource> chain = MagebloodIconChain.For(
            null,
            "FakeA",
            new[] { "StatA", "StatA" }
        );

        Assert.Equal(new[] { Buff("StatA"), Item() }, chain);
    }

    [Fact]
    public void For_NullOrBlankStats_Skipped()
    {
        IReadOnlyList<MagebloodIconSource> chain = MagebloodIconChain.For(
            null,
            "FakeA",
            new[] { null, "", " ", "StatA" }
        );

        Assert.Equal(new[] { Buff("StatA"), Item() }, chain);
    }

    [Fact]
    public void For_NullList_IconsThenItem()
    {
        IReadOnlyList<MagebloodIconSource> chain = MagebloodIconChain.For(
            "fakeicon",
            "FakeA",
            null
        );

        Assert.Equal(new[] { Flask("fakeicon"), Buff("fakeicon"), Item() }, chain);
    }

    [Fact]
    public void For_LastStep_IsItemIcon()
    {
        IReadOnlyList<MagebloodIconSource> chain = MagebloodIconChain.For(null, null, null);

        Assert.Equal(Item(), Assert.Single(chain));
    }

    private static MagebloodIconSource Flask(string name)
    {
        return new MagebloodIconSource(MagebloodIconSourceKind.FlaskIcon, name);
    }

    private static MagebloodIconSource Buff(string name)
    {
        return new MagebloodIconSource(MagebloodIconSourceKind.BuffIcon, name);
    }

    private static MagebloodIconSource Item()
    {
        return new MagebloodIconSource(MagebloodIconSourceKind.ItemIcon, null);
    }
}
