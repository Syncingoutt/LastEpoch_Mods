using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Config;

public sealed class MagebloodConfigMergerTests
{
    private const int Version = 3;

    private static readonly IReadOnlyList<MagebloodVersionedFlask> _flasks = new[]
    {
        Versioned("FakeA", 1),
        Versioned("FakeB", 2),
        Versioned("FakeC", 3),
    };

    [Fact]
    public void Merge_Unstamped_CountsAsOne()
    {
        MagebloodMergeResult result = Merge("""{"flasks":[]}""");

        Assert.Equal(new[] { "FakeB", "FakeC" }, FlaskNames(result));
        Assert.Equal(2, result.Added);
        Assert.True(result.Changed);
        Assert.Equal(Version, (int)JObject.Parse(result.Text)["defaultsVersion"]);
    }

    [Theory]
    [InlineData("\"x\"")]
    [InlineData("1.5")]
    [InlineData("null")]
    public void Merge_InvalidStamp_CountsAsOne(string stamp)
    {
        MagebloodMergeResult result = Merge($$"""{"defaultsVersion":{{stamp}},"flasks":[]}""");

        Assert.Equal(new[] { "FakeB", "FakeC" }, FlaskNames(result));
        Assert.Equal(2, result.Added);
    }

    [Fact]
    public void Merge_Newer_AppendedAtEndInOrder()
    {
        MagebloodMergeResult result = Merge(
            """{"defaultsVersion":1,"flasks":[{"name":"FakeOwn","stats":[]}]}"""
        );

        Assert.Equal(new[] { "FakeOwn", "FakeB", "FakeC" }, FlaskNames(result));
        Assert.Equal(2, result.Added);
        Assert.True(result.Changed);
    }

    [Fact]
    public void Merge_OnlyFlasksAboveStamp_Appended()
    {
        MagebloodMergeResult result = Merge("""{"defaultsVersion":2,"flasks":[]}""");

        Assert.Equal(new[] { "FakeC" }, FlaskNames(result));
        Assert.Equal(1, result.Added);
    }

    [Fact]
    public void Merge_AppendedFlask_HoldsDefaultRows()
    {
        MagebloodMergeResult result = Merge("""{"defaultsVersion":2,"flasks":[]}""");

        JToken row = JObject.Parse(result.Text)["flasks"][0]["stats"][0];
        Assert.Equal("FakeA", (string)row["stat"]);
        Assert.Equal(1f, (float)row["added"]);
    }

    [Fact]
    public void Merge_ExistingName_NotDuplicated_AndUntouched()
    {
        MagebloodMergeResult result = Merge(
            """{"defaultsVersion":1,"flasks":[{"name":"FakeB","stats":[{"stat":"FakeOwn","added":9}]}]}"""
        );

        JToken kept = JObject.Parse(result.Text)["flasks"][0];
        Assert.Equal(new[] { "FakeB", "FakeC" }, FlaskNames(result));
        Assert.Equal(1, result.Added);
        Assert.Equal("FakeOwn", (string)kept["stats"][0]["stat"]);
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    public void Merge_StampNotBelowVersion_TextUnchanged(int stamp)
    {
        string json = $$"""{"defaultsVersion":{{stamp}},"flasks":[]}""";

        MagebloodMergeResult result = Merge(json);

        Assert.Equal(json, result.Text);
        Assert.False(result.Changed);
        Assert.Equal(0, result.Added);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("{ not json")]
    [InlineData("[]")]
    [InlineData("""{"defaultsVersion":1,"flasks":5}""")]
    public void Merge_Unusable_TextUnchanged(string json)
    {
        MagebloodMergeResult result = Merge(json);

        Assert.Equal(json, result.Text);
        Assert.False(result.Changed);
        Assert.Equal(0, result.Added);
    }

    [Fact]
    public void Merge_FlasksMissing_StampsOnly()
    {
        MagebloodMergeResult result = Merge("""{"defaultsVersion":1}""");

        var root = JObject.Parse(result.Text);
        Assert.Equal(Version, (int)root["defaultsVersion"]);
        Assert.Null(root["flasks"]);
        Assert.Equal(0, result.Added);
        Assert.True(result.Changed);
    }

    [Fact]
    public void Merge_MaxResistancesMissing_Added()
    {
        MagebloodMergeResult result = MergeWithMax("""{"defaultsVersion":1,"flasks":[]}""");

        var root = JObject.Parse(result.Text);
        Assert.Equal(7f, (float)root["maxResistances"]);
        Assert.Equal(Version, (int)root["defaultsVersion"]);
        Assert.Equal(3, result.Added);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("\"x\"")]
    [InlineData("-1")]
    public void Merge_MaxResistancesAnyValue_Kept(string value)
    {
        MagebloodMergeResult result = MergeWithMax(
            $$"""{"defaultsVersion":1,"maxResistances":{{value}},"flasks":[]}"""
        );

        JToken expected = JObject.Parse($$"""{"v":{{value}}}""")["v"];
        Assert.True(JToken.DeepEquals(expected, JObject.Parse(result.Text)["maxResistances"]));
        Assert.Equal(2, result.Added);
    }

    [Fact]
    public void Merge_FlasksNotList_MaxResistancesNotAdded()
    {
        const string json = """{"defaultsVersion":1,"flasks":5}""";

        MagebloodMergeResult result = MergeWithMax(json);

        Assert.Equal(json, result.Text);
        Assert.False(result.Changed);
        Assert.Equal(0, result.Added);
    }

    [Fact]
    public void Merge_MaxResistancesPresent_Kept()
    {
        MagebloodMergeResult result = MergeWithMax(
            """{"defaultsVersion":1,"maxResistances":9,"flasks":[]}"""
        );

        Assert.Equal(9f, (float)JObject.Parse(result.Text)["maxResistances"]);
        Assert.Equal(2, result.Added);
    }

    [Fact]
    public void Merge_MaxResistancesAtStamp_NotAdded()
    {
        MagebloodMergeResult result = MergeWithMax("""{"defaultsVersion":2,"flasks":[]}""");

        Assert.Null(JObject.Parse(result.Text)["maxResistances"]);
    }

    [Fact]
    public void Merge_FlasksMissing_StillAddsMaxResistances()
    {
        MagebloodMergeResult result = MergeWithMax("""{"defaultsVersion":1}""");

        Assert.Equal(7f, (float)JObject.Parse(result.Text)["maxResistances"]);
        Assert.Equal(1, result.Added);
    }

    private static MagebloodMergeResult Merge(string json)
    {
        return MagebloodConfigMerger.Merge(
            json,
            _flasks,
            new MagebloodVersionedValue(7f, 0),
            Version
        );
    }

    private static MagebloodMergeResult MergeWithMax(string json)
    {
        return MagebloodConfigMerger.Merge(
            json,
            _flasks,
            new MagebloodVersionedValue(7f, 2),
            Version
        );
    }

    private static MagebloodVersionedFlask Versioned(string name, int since)
    {
        return new MagebloodVersionedFlask(
            MagebloodTestData.Flask(name, new MagebloodStatEntry("FakeA", Added: 1f)),
            since
        );
    }

    private static string[] FlaskNames(MagebloodMergeResult result)
    {
        return JObject
            .Parse(result.Text)["flasks"]
            .Select(flask => (string)flask["name"])
            .ToArray();
    }
}
