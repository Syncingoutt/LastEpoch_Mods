using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Config;

public sealed class MagebloodConfigWriterTests
{
    [Fact]
    public void Write_Defaults_RoundTrips()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            MagebloodConfigWriter.Write(MagebloodConfigDefaults.Config)
        );

        IReadOnlyList<MagebloodFlaskEntry> expected = MagebloodConfigDefaults.Config.Flasks;
        Assert.True(result.IsReadable);
        Assert.Empty(result.Problems);
        Assert.Equal(MagebloodConfigDefaults.Config.Version, result.Config.Version);
        Assert.Equal(expected.Select(f => f.Name), result.Config.Flasks.Select(f => f.Name));
        Assert.Equal(expected.Select(f => f.Icon), result.Config.Flasks.Select(f => f.Icon));
        Assert.Equal(
            expected.SelectMany(f => f.Stats),
            result.Config.Flasks.SelectMany(f => f.Stats)
        );
    }

    [Fact]
    public void Write_OmitsNullIconTagAndZeroMore()
    {
        MagebloodConfig config = MagebloodTestData.Config(
            MagebloodTestData.Flask("FakeFlask", new MagebloodStatEntry("FakeA"))
        );

        JToken flask = JObject.Parse(MagebloodConfigWriter.Write(config))["flasks"][0];

        Assert.Null(flask["icon"]);
        JToken row = flask["stats"][0];
        Assert.Equal("FakeA", (string)row["stat"]);
        Assert.NotNull(row["added"]);
        Assert.NotNull(row["increased"]);
        Assert.Null(row["tag"]);
        Assert.Null(row["more"]);
    }

    [Fact]
    public void Write_KeepsIconTagAndMore_WhenSet()
    {
        var flask = new MagebloodFlaskEntry
        {
            Name = "FakeFlask",
            Icon = "IconA",
            Stats = new[] { new MagebloodStatEntry("FakeA", 1f, 2f, -3f, "FakeTag") },
        };

        JToken written = JObject.Parse(
            MagebloodConfigWriter.Write(MagebloodTestData.Config(flask))
        )["flasks"][0];

        Assert.Equal("IconA", (string)written["icon"]);
        Assert.Equal("FakeTag", (string)written["stats"][0]["tag"]);
        Assert.Equal(-3f, (float)written["stats"][0]["more"]);
    }

    [Fact]
    public void Write_StampsVersions()
    {
        var root = JObject.Parse(MagebloodConfigWriter.Write(MagebloodTestData.Config()));

        Assert.Equal(1, (int)root["version"]);
        Assert.Equal(MagebloodConfigDefaults.DefaultsVersion, (int)root["defaultsVersion"]);
    }
}
