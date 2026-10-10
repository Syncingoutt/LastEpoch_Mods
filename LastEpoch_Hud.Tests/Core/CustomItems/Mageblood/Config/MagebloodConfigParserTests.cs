using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;
using Code = LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config.MagebloodConfigProblemCode;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Config;

public sealed class MagebloodConfigParserTests
{
    private const string GoodFlask = """{"name":"FakeGood","stats":[{"stat":"FakeA","added":1}]}""";
    private const string GoodRow = """{"stat":"FakeB","added":1}""";

    [Fact]
    public void Parse_ValidFile_ReadsFlasksInOrder()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            """
            {"version":1,"flasks":[
              {"name":"FakeFirst","icon":"IconA","stats":[
                {"stat":"FakeA","tag":"FakeTag","added":1.5,"increased":20,"more":-10},
                {"stat":"FakeB","increased":5}]},
              {"name":"FakeSecond","stats":[{"stat":"FakeA","added":2}]}]}
            """
        );

        Assert.True(result.IsReadable);
        Assert.Empty(result.Problems);
        Assert.Equal(1, result.Config.Version);
        Assert.Equal(new[] { "FakeFirst", "FakeSecond" }, Names(result));
        Assert.Equal("IconA", result.Config.Flasks[0].Icon);
        Assert.Equal(
            new[]
            {
                new MagebloodStatEntry("FakeA", 1.5f, 20f, -10f, "FakeTag"),
                new MagebloodStatEntry("FakeB", 0f, 5f, 0f, null),
            },
            result.Config.Flasks[0].Stats
        );
        Assert.Equal(
            new[] { new MagebloodStatEntry("FakeA", 2f, 0f, 0f, null) },
            result.Config.Flasks[1].Stats
        );
    }

    [Fact]
    public void Parse_AbsentOptionalFields_UseDefaults()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            """{"version":1,"flasks":[{"name":"FakeFlask","stats":[{"stat":"FakeA","added":1}]}]}"""
        );

        Assert.Null(result.Config.Flasks[0].Icon);
        Assert.Equal(
            new MagebloodStatEntry("FakeA", 1f, 0f, 0f, null),
            result.Config.Flasks[0].Stats[0]
        );
        Assert.Empty(result.Problems);
    }

    [Fact]
    public void Parse_EmptyList_HasNoFlasks()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            """{"version":1,"flasks":[]}"""
        );

        Assert.True(result.IsReadable);
        Assert.Empty(result.Config.Flasks);
        Assert.Empty(result.Problems);
    }

    [Theory]
    [InlineData(null, Code.EmptyFile)]
    [InlineData("", Code.EmptyFile)]
    [InlineData("   ", Code.EmptyFile)]
    [InlineData("{ not json", Code.InvalidJson)]
    [InlineData("[]", Code.RootNotObject)]
    [InlineData("5", Code.RootNotObject)]
    public void Parse_UnusableText_ReturnsDefaults(string json, Code code)
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(json);

        Assert.False(result.IsReadable);
        Assert.Equal(code, Assert.Single(result.Problems).Code);
        Assert.Equal(DefaultNames(), Names(result));
        Assert.Equal(MagebloodConfigDefaults.Config.Version, result.Config.Version);
    }

    [Theory]
    [InlineData("\"x\"")]
    [InlineData("1.5")]
    [InlineData("null")]
    public void Parse_VersionNotInt_Reports(string version)
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            $$"""{"version":{{version}},"flasks":[]}"""
        );

        Assert.Contains(result.Problems, p => p.Code == Code.NotWholeNumber && p.Path == "version");
    }

    [Fact]
    public void Parse_VersionUnsupported_StillReadable()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            $$"""{"version":2,"flasks":[{{GoodFlask}}]}"""
        );

        Assert.True(result.IsReadable);
        Assert.Contains(
            result.Problems,
            p => p.Code == Code.UnsupportedVersion && p.Path == "version"
        );
        Assert.Equal(new[] { "FakeGood" }, Names(result));
    }

    [Fact]
    public void Parse_FlasksMissing_ReturnsDefaults()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse("""{"version":1}""");

        Assert.True(result.IsReadable);
        Assert.Empty(result.Problems);
        Assert.Equal(DefaultNames(), Names(result));
    }

    [Fact]
    public void Parse_FlasksNotList_ReportsNotList()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            """{"version":1,"flasks":5}"""
        );

        Assert.Contains(result.Problems, p => p.Code == Code.NotList && p.Path == "flasks");
        Assert.Equal(DefaultNames(), Names(result));
    }

    [Theory]
    [InlineData("5", Code.NotObject, "flasks[0]")]
    [InlineData("""{"stats":[{"stat":"FakeA","added":1}]}""", Code.MissingName, "flasks[0]")]
    [InlineData(
        """{"name":5,"stats":[{"stat":"FakeA","added":1}]}""",
        Code.EmptyOrNotText,
        "flasks[0].name"
    )]
    [InlineData(
        """{"name":"  ","stats":[{"stat":"FakeA","added":1}]}""",
        Code.EmptyOrNotText,
        "flasks[0].name"
    )]
    [InlineData("""{"name":"FakeBad"}""", Code.NoStats, "flasks[0]")]
    [InlineData("""{"name":"FakeBad","stats":5}""", Code.NotList, "flasks[0].stats")]
    [InlineData("""{"name":"FakeBad","stats":[]}""", Code.NoStats, "flasks[0].stats")]
    [InlineData("""{"name":"FakeBad","stats":[5]}""", Code.NoStats, "flasks[0].stats")]
    public void Parse_BadFlask_IsSkipped(string badFlask, Code code, string path)
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            $$"""{"version":1,"flasks":[{{badFlask}},{{GoodFlask}}]}"""
        );

        Assert.Contains(result.Problems, p => p.Code == code && p.Path == path);
        Assert.Equal(new[] { "FakeGood" }, Names(result));
    }

    [Fact]
    public void Parse_DuplicateFlaskName_SecondIsSkipped()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            $$"""
            {"version":1,"flasks":[{{GoodFlask}},
              {"name":"FakeGood","stats":[{"stat":"FakeB","added":9}]}]}
            """
        );

        Assert.Contains(
            result.Problems,
            p => p.Code == Code.DuplicateFlask && p.Path == "flasks[1].name"
        );
        Assert.Equal(new[] { "FakeGood" }, Names(result));
        Assert.Equal("FakeA", result.Config.Flasks[0].Stats[0].Stat);
    }

    [Fact]
    public void Parse_DuplicateOfSkippedFlask_IsKept()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            $$"""{"version":1,"flasks":[{"name":"FakeGood","stats":[]},{{GoodFlask}}]}"""
        );

        Assert.DoesNotContain(result.Problems, p => p.Code == Code.DuplicateFlask);
        Assert.Equal(new[] { "FakeGood" }, Names(result));
        Assert.Equal("FakeA", result.Config.Flasks[0].Stats[0].Stat);
    }

    [Fact]
    public void Parse_FlaskNamesDifferingInCase_BothKept()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            """
            {"version":1,"flasks":[
              {"name":"FakeX","stats":[{"stat":"FakeA","added":1}]},
              {"name":"fakex","stats":[{"stat":"FakeA","added":1}]}]}
            """
        );

        Assert.Empty(result.Problems);
        Assert.Equal(new[] { "FakeX", "fakex" }, Names(result));
    }

    [Theory]
    [InlineData("5", Code.NotObject, "flasks[0].stats[0]")]
    [InlineData("""{"added":1}""", Code.MissingStat, "flasks[0].stats[0]")]
    [InlineData("""{"stat":5,"added":1}""", Code.EmptyOrNotText, "flasks[0].stats[0].stat")]
    [InlineData(
        """{"stat":"FakeA","tag":5,"added":1}""",
        Code.EmptyOrNotText,
        "flasks[0].stats[0].tag"
    )]
    [InlineData(
        """{"stat":"FakeA","added":"x"}""",
        Code.NotFiniteNumber,
        "flasks[0].stats[0].added"
    )]
    [InlineData(
        """{"stat":"FakeA","increased":"x"}""",
        Code.NotFiniteNumber,
        "flasks[0].stats[0].increased"
    )]
    [InlineData("""{"stat":"FakeA","more":"x"}""", Code.NotFiniteNumber, "flasks[0].stats[0].more")]
    [InlineData("""{"stat":"FakeA"}""", Code.NoValue, "flasks[0].stats[0]")]
    [InlineData(
        """{"stat":"FakeA","added":0,"increased":0,"more":0}""",
        Code.NoValue,
        "flasks[0].stats[0]"
    )]
    public void Parse_BadRow_IsSkipped_FlaskKept(string badRow, Code code, string path)
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            $$"""{"version":1,"flasks":[{"name":"FakeFlask","stats":[{{badRow}},{{GoodRow}}]}]}"""
        );

        Assert.Contains(result.Problems, p => p.Code == code && p.Path == path);
        Assert.Equal(new[] { "FakeFlask" }, Names(result));
        Assert.Equal(new[] { "FakeB" }, result.Config.Flasks[0].Stats.Select(row => row.Stat));
    }

    [Fact]
    public void Parse_DuplicateStatRow_SecondIsSkipped()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            """
            {"version":1,"flasks":[{"name":"FakeFlask","stats":[
              {"stat":"FakeA","added":1},{"stat":"FakeA","increased":2}]}]}
            """
        );

        Assert.Contains(
            result.Problems,
            p => p.Code == Code.DuplicateStat && p.Path == "flasks[0].stats[1].stat"
        );
        Assert.Equal(
            new[] { new MagebloodStatEntry("FakeA", 1f, 0f, 0f, null) },
            result.Config.Flasks[0].Stats
        );
    }

    [Fact]
    public void Parse_DuplicateOfSkippedRow_IsKept()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            """
            {"version":1,"flasks":[{"name":"FakeFlask","stats":[
              {"stat":"FakeA"},{"stat":"FakeA","added":1}]}]}
            """
        );

        Assert.DoesNotContain(result.Problems, p => p.Code == Code.DuplicateStat);
        Assert.Equal(
            new[] { new MagebloodStatEntry("FakeA", 1f, 0f, 0f, null) },
            result.Config.Flasks[0].Stats
        );
    }

    [Fact]
    public void Parse_SameStatDifferentTag_BothKept()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            """
            {"version":1,"flasks":[{"name":"FakeFlask","stats":[
              {"stat":"FakeA","added":1},{"stat":"FakeA","tag":"FakeTag","added":1}]}]}
            """
        );

        Assert.Empty(result.Problems);
        Assert.Equal(2, result.Config.Flasks[0].Stats.Count);
    }

    [Fact]
    public void Parse_BadIcon_ReportsAndKeepsFlask()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            """{"version":1,"flasks":[{"name":"FakeFlask","icon":5,"stats":[{"stat":"FakeA","added":1}]}]}"""
        );

        Assert.Contains(
            result.Problems,
            p => p.Code == Code.EmptyOrNotText && p.Path == "flasks[0].icon"
        );
        Assert.Equal(new[] { "FakeFlask" }, Names(result));
        Assert.Null(result.Config.Flasks[0].Icon);
    }

    [Fact]
    public void Parse_MaxResistances_Read()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            """{"version":1,"maxResistances":7.5,"flasks":[]}"""
        );

        Assert.Equal(7.5f, result.Config.MaxResistances);
        Assert.Empty(result.Problems);
    }

    [Fact]
    public void Parse_MaxResistancesMissing_Default()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            """{"version":1,"flasks":[]}"""
        );

        Assert.Equal(5f, result.Config.MaxResistances);
        Assert.Empty(result.Problems);
    }

    [Theory]
    [InlineData("\"x\"")]
    [InlineData("null")]
    [InlineData("true")]
    public void Parse_MaxResistancesNotNumber_Reports(string value)
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            $$"""{"version":1,"maxResistances":{{value}},"flasks":[]}"""
        );

        Assert.Contains(
            result.Problems,
            p => p.Code == Code.NotFiniteNumber && p.Path == "maxResistances"
        );
        Assert.Equal(5f, result.Config.MaxResistances);
    }

    [Fact]
    public void Parse_MaxResistancesNegative_Reports()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            """{"version":1,"maxResistances":-1,"flasks":[]}"""
        );

        Assert.Contains(
            result.Problems,
            p => p.Code == Code.Negative && p.Path == "maxResistances"
        );
        Assert.Equal(5f, result.Config.MaxResistances);
    }

    [Fact]
    public void Parse_MaxResistancesZero_Kept()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(
            """{"version":1,"maxResistances":0,"flasks":[]}"""
        );

        Assert.Equal(0f, result.Config.MaxResistances);
        Assert.Empty(result.Problems);
    }

    [Fact]
    public void Parse_Unusable_DefaultMaxResistances()
    {
        MagebloodConfigParseResult result = MagebloodConfigParser.Parse("");

        Assert.Equal(5f, result.Config.MaxResistances);
    }

    private static string[] Names(MagebloodConfigParseResult result)
    {
        return result.Config.Flasks.Select(flask => flask.Name).ToArray();
    }

    private static string[] DefaultNames()
    {
        return MagebloodConfigDefaults.Flasks.Select(flask => flask.Name).ToArray();
    }
}
