using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Config;

public sealed class MagebloodConfigDefaultsTests
{
    private static readonly string[] _tableOrder =
    {
        "Quicksilver",
        "Silver",
        "Granite",
        "Diamond",
        "Jade",
        "Bismuth",
        "Basalt",
        "Amethyst",
        "Quartz",
        "Sulphur",
        "Gold",
    };

    private static readonly Dictionary<string, MagebloodStatEntry[]> _rows = new()
    {
        ["Quicksilver"] = new[] { new MagebloodStatEntry("Movespeed", Increased: 30f) },
        ["Silver"] = new[]
        {
            new MagebloodStatEntry("AttackSpeed", Increased: 20f),
            new MagebloodStatEntry("CastSpeed", Increased: 20f),
            new MagebloodStatEntry("Movespeed", Increased: 20f),
        },
        ["Granite"] = new[] { new MagebloodStatEntry("Armour", Added: 500f) },
        ["Diamond"] = new[] { new MagebloodStatEntry("CriticalChance", Increased: 100f) },
        ["Jade"] = new[] { new MagebloodStatEntry("DodgeRating", Added: 500f) },
        ["Bismuth"] = new[] { new MagebloodStatEntry("AllResistances", Added: 0.35f) },
        ["Basalt"] = new[] { new MagebloodStatEntry("DamageTaken", More: -15f, Tag: "Physical") },
        ["Amethyst"] = new[]
        {
            new MagebloodStatEntry("VoidResistance", Added: 0.35f),
            new MagebloodStatEntry("NecroticResistance", Added: 0.35f),
            new MagebloodStatEntry("PoisonResistance", Added: 0.35f),
        },
        ["Quartz"] = new[]
        {
            new MagebloodStatEntry("DodgeRating", Added: 250f),
            new MagebloodStatEntry("Movespeed", Increased: 10f),
        },
        ["Sulphur"] = new[] { new MagebloodStatEntry("Damage", Increased: 40f) },
        ["Gold"] = new[] { new MagebloodStatEntry("IncreasedDropRate", Added: 0.2f) },
    };

    [Fact]
    public void Flasks_InTableOrder()
    {
        Assert.Equal(_tableOrder, MagebloodConfigDefaults.Flasks.Select(f => f.Name));
        Assert.Equal(_tableOrder, MagebloodConfigDefaults.Config.Flasks.Select(f => f.Name));
        Assert.Equal(
            _tableOrder,
            MagebloodConfigDefaults.VersionedFlasks.Select(f => f.Entry.Name)
        );
    }

    [Theory]
    [InlineData("Quicksilver")]
    [InlineData("Silver")]
    [InlineData("Granite")]
    [InlineData("Diamond")]
    [InlineData("Jade")]
    [InlineData("Bismuth")]
    [InlineData("Basalt")]
    [InlineData("Amethyst")]
    [InlineData("Quartz")]
    [InlineData("Sulphur")]
    [InlineData("Gold")]
    public void Flask_Rows_HoldTodaysValues(string name)
    {
        MagebloodFlaskEntry flask = MagebloodConfigDefaults.Flasks.Single(f => f.Name == name);

        Assert.Equal(_rows[name], flask.Stats);
    }

    [Fact]
    public void AllSince_One()
    {
        Assert.All(MagebloodConfigDefaults.VersionedFlasks, flask => Assert.Equal(1, flask.Since));
    }

    [Fact]
    public void MaxResistances_IsFiveSinceTwo()
    {
        Assert.Equal(new MagebloodVersionedValue(5f, 2), MagebloodConfigDefaults.MaxResistances);
        Assert.Equal(5f, MagebloodConfigDefaults.Config.MaxResistances);
    }

    [Fact]
    public void DefaultsVersion_CoversEverySince()
    {
        Assert.All(
            MagebloodConfigDefaults.VersionedFlasks,
            flask => Assert.True(flask.Since <= MagebloodConfigDefaults.DefaultsVersion)
        );
        Assert.True(
            MagebloodConfigDefaults.MaxResistances.Since <= MagebloodConfigDefaults.DefaultsVersion
        );
    }

    [Fact]
    public void Versions()
    {
        Assert.Equal(2, MagebloodConfigDefaults.DefaultsVersion);
        Assert.Equal(1, MagebloodConfigDefaults.CurrentVersion);
        Assert.Equal(1, MagebloodConfigDefaults.UnstampedDefaultsVersion);
        Assert.Equal(
            MagebloodConfigDefaults.CurrentVersion,
            MagebloodConfigDefaults.Config.Version
        );
    }
}
