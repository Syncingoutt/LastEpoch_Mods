using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings;

public sealed class LegacySettingsImporterTests
{
    [Fact]
    public void TryImport_ValidFile_ReturnsTrue()
    {
        bool ok = LegacySettingsImporter.TryImport(
            Wrap("true"),
            new[] { Field("F", LegacySettingPart.Bool) },
            new[] { "G" },
            new FakeKeyNames(),
            out JObject _,
            new List<LegacyImportProblem>()
        );

        Assert.True(ok);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("1", true)]
    [InlineData("0", false)]
    [InlineData("2", true)]
    [InlineData("-1", true)]
    [InlineData("\"true\"", true)]
    [InlineData("\" False \"", false)]
    public void TryImport_Bool_EmitsValue(string raw, bool expected)
    {
        JObject sections = ImportOne(raw, Field("F", LegacySettingPart.Bool));

        Assert.Equal(expected, (bool)sections["G"]["K"]);
    }

    [Theory]
    [InlineData("\"yes\"")]
    [InlineData("1.5")]
    [InlineData("null")]
    [InlineData("[]")]
    public void TryImport_BoolInvalid_RecordsProblem(string raw)
    {
        List<LegacyImportProblem> problems = new();

        JObject sections = Import(
            Wrap(raw),
            new[] { Field("F", LegacySettingPart.Bool) },
            problems
        );

        Assert.False(sections.ContainsKey("G"));
        LegacyImportProblem problem = Assert.Single(problems);
        Assert.Equal(LegacyImportProblemCode.InvalidValue, problem.Code);
        Assert.Equal("F", problem.Path);
    }

    [Fact]
    public void TryImport_FloatParts_MergeIntoOneObject()
    {
        LegacyFieldMapping[] map =
        {
            Field("E", LegacySettingPart.FloatEnabled),
            Field("V", LegacySettingPart.FloatValue),
        };

        JObject sections = Import("{\"E\":true,\"V\":2.5}", map, new List<LegacyImportProblem>());

        Assert.True(
            JToken.DeepEquals(
                JObject.Parse("{\"G\":{\"K\":{\"Enabled\":true,\"Value\":2.5}}}"),
                sections
            )
        );
    }

    [Fact]
    public void TryImport_FloatValueOnly_EmitsOnlyValue()
    {
        LegacyFieldMapping[] map =
        {
            Field("E", LegacySettingPart.FloatEnabled),
            Field("V", LegacySettingPart.FloatValue),
        };

        JObject sections = Import("{\"V\":2.5}", map, new List<LegacyImportProblem>());

        var setting = (JObject)sections["G"]["K"];
        Assert.Equal(new[] { "Value" }, setting.Properties().Select(p => p.Name));
    }

    [Theory]
    [InlineData("3", 3f)]
    [InlineData("3.5", 3.5f)]
    [InlineData("\"3.5\"", 3.5f)]
    public void TryImport_Number_AcceptsIntegerFloatAndString(string raw, float expected)
    {
        JObject sections = ImportOne(raw, Field("F", LegacySettingPart.FloatValue));

        Assert.Equal(expected, (float)sections["G"]["K"]["Value"]);
    }

    [Theory]
    [InlineData("\"NaN\"")]
    [InlineData("\"Infinity\"")]
    [InlineData("1e300")]
    [InlineData("true")]
    [InlineData("\"abc\"")]
    [InlineData("null")]
    [InlineData("[]")]
    [InlineData("{}")]
    public void TryImport_NumberInvalid_RecordsProblemAndSkips(string raw)
    {
        LegacyFieldMapping mapping = Field("F", LegacySettingPart.FloatValue);
        List<LegacyImportProblem> problems = new();

        JObject sections = Import(Wrap(raw), new[] { mapping }, problems);

        Assert.False(sections.ContainsKey("G"));
        LegacyImportProblem problem = Assert.Single(problems);
        Assert.Equal(LegacyImportProblemCode.InvalidValue, problem.Code);
        Assert.Equal("F", problem.Path);
    }

    [Theory]
    [InlineData("15", 10f)]
    [InlineData("-3", 0f)]
    public void TryImport_Number_ClampsToLimits(string raw, float expected)
    {
        JObject sections = ImportOne(
            raw,
            Field("F", LegacySettingPart.FloatValue, minLimit: 0f, maxLimit: 10f)
        );

        Assert.Equal(expected, (float)sections["G"]["K"]["Value"]);
    }

    [Theory]
    [InlineData("5", 0f)]
    [InlineData("-15", -10f)]
    public void TryImport_Number_ClampsNegativeLimits(string raw, float expected)
    {
        JObject sections = ImportOne(
            raw,
            Field("F", LegacySettingPart.FloatValue, minLimit: -10f, maxLimit: 0f)
        );

        Assert.Equal(expected, (float)sections["G"]["K"]["Value"]);
    }

    [Fact]
    public void TryImport_Number_ClampIsSilent()
    {
        LegacyFieldMapping mapping = Field(
            "F",
            LegacySettingPart.FloatValue,
            minLimit: 0f,
            maxLimit: 10f
        );
        List<LegacyImportProblem> problems = new();

        Import(Wrap("15"), new[] { mapping }, problems);

        Assert.Empty(problems);
    }

    [Fact]
    public void TryImport_NoLimits_KeepsValue()
    {
        JObject sections = ImportOne("1000", Field("F", LegacySettingPart.FloatValue));

        Assert.Equal(1000f, (float)sections["G"]["K"]["Value"]);
    }

    [Fact]
    public void TryImport_RangeParts_MergeIntoOneObject()
    {
        JObject sections = Import(
            "{\"E\":true,\"Mn\":2,\"Mx\":5}",
            RangeMap(),
            new List<LegacyImportProblem>()
        );

        Assert.True(
            JToken.DeepEquals(
                JObject.Parse("{\"G\":{\"K\":{\"Enabled\":true,\"Min\":2.0,\"Max\":5.0}}}"),
                sections
            )
        );
    }

    [Fact]
    public void TryImport_RangeInverted_Swaps()
    {
        JObject sections = Import(
            "{\"Mn\":8,\"Mx\":3}",
            RangeMap(),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(3f, (float)sections["G"]["K"]["Min"]);
        Assert.Equal(8f, (float)sections["G"]["K"]["Max"]);
    }

    [Fact]
    public void TryImport_RangeInvertedAfterClamp_Swaps()
    {
        JObject sections = Import(
            "{\"Mn\":15,\"Mx\":4}",
            RangeMap(0f, 10f),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(4f, (float)sections["G"]["K"]["Min"]);
        Assert.Equal(10f, (float)sections["G"]["K"]["Max"]);
    }

    [Fact]
    public void TryImport_RangeOnlyMin_NoSwap()
    {
        JObject sections = Import("{\"Mn\":8}", RangeMap(), new List<LegacyImportProblem>());

        var range = (JObject)sections["G"]["K"];
        Assert.Equal(8f, (float)range["Min"]);
        Assert.False(range.ContainsKey("Max"));
    }

    [Theory]
    [InlineData("2", 2)]
    [InlineData("2.5", 2)]
    [InlineData("3.5", 4)]
    [InlineData("\"1\"", 1)]
    public void TryImport_Dropdown_EmitsRoundedInt(string raw, int expected)
    {
        JToken value = ImportOne(raw, Field("F", LegacySettingPart.Dropdown))["G"]["K"];

        Assert.Equal(JTokenType.Integer, value.Type);
        Assert.Equal(expected, (int)value);
    }

    [Theory]
    [InlineData("1e20", int.MaxValue)]
    [InlineData("-1e20", int.MinValue)]
    [InlineData("1e10", int.MaxValue)]
    [InlineData("-1e10", int.MinValue)]
    public void TryImport_Dropdown_HugeValue_ClampsToIntRange(string raw, int expected)
    {
        JToken value = ImportOne(raw, Field("F", LegacySettingPart.Dropdown))["G"]["K"];

        Assert.Equal(JTokenType.Integer, value.Type);
        Assert.Equal(expected, (int)value);
    }

    [Fact]
    public void TryImport_DropdownInvalid_RecordsProblem()
    {
        List<LegacyImportProblem> problems = new();

        JObject sections = Import(
            Wrap("\"NaN\""),
            new[] { Field("F", LegacySettingPart.Dropdown) },
            problems
        );

        Assert.False(sections.ContainsKey("G"));
        LegacyImportProblem problem = Assert.Single(problems);
        Assert.Equal(LegacyImportProblemCode.InvalidValue, problem.Code);
        Assert.Equal("F", problem.Path);
    }

    [Fact]
    public void TryImport_Dropdown_ClampsToLimits()
    {
        JToken value = ImportOne(
            "7",
            Field("F", LegacySettingPart.Dropdown, minLimit: 0f, maxLimit: 3f)
        )["G"]["K"];

        Assert.Equal(3, (int)value);
    }

    [Theory]
    [InlineData("284", "kb:F3")]
    [InlineData("\"F7\"", "kb:F7")]
    [InlineData("\"f7\"", "kb:F7")]
    [InlineData("\"288\"", "kb:F7")]
    [InlineData("\" F3 \"", "kb:F3")]
    [InlineData("\" 288 \"", "kb:F7")]
    public void TryImport_Keybind_EmitsBinding(string raw, string expected)
    {
        JObject sections = ImportOne(raw, Field("F", LegacySettingPart.Keybind));

        Assert.Equal(expected, (string)sections["G"]["K"]);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("\"None\"")]
    public void TryImport_KeyCodeZero_EmitsNoKey(string raw)
    {
        LegacyFieldMapping mapping = new("K.B", "G", "B", LegacySettingPart.Keybind);
        List<LegacyImportProblem> problems = new();

        JObject sections = Import("{\"K\":{\"B\":" + raw + "}}", new[] { mapping }, problems);

        Assert.False(sections.ContainsKey("G"));
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("99999")]
    [InlineData("\"NotAKey\"")]
    [InlineData("true")]
    [InlineData("1.5")]
    [InlineData("3000000000")]
    [InlineData("4294967580")]
    [InlineData("null")]
    public void TryImport_KeybindInvalid_RecordsProblem(string raw)
    {
        LegacyFieldMapping mapping = Field("F", LegacySettingPart.Keybind);
        List<LegacyImportProblem> problems = new();

        JObject sections = Import(Wrap(raw), new[] { mapping }, problems);

        Assert.False(sections.ContainsKey("G"));
        LegacyImportProblem problem = Assert.Single(problems);
        Assert.Equal(LegacyImportProblemCode.InvalidValue, problem.Code);
        Assert.Equal("F", problem.Path);
    }

    [Theory]
    [InlineData("\"Skill A\"", "Skill A")]
    [InlineData("\"2024-01-01T00:00:00\"", "2024-01-01T00:00:00")]
    [InlineData("\"\"", "")]
    public void TryImport_Text_EmitsString(string raw, string expected)
    {
        JObject sections = ImportOne(raw, Field("F", LegacySettingPart.Text));

        Assert.Equal(JTokenType.String, sections["G"]["K"].Type);
        Assert.Equal(expected, (string)sections["G"]["K"]);
    }

    [Theory]
    [InlineData("5")]
    [InlineData("null")]
    public void TryImport_TextInvalid_RecordsProblem(string raw)
    {
        LegacyFieldMapping mapping = Field("F", LegacySettingPart.Text);
        List<LegacyImportProblem> problems = new();

        Import(Wrap(raw), new[] { mapping }, problems);

        LegacyImportProblem problem = Assert.Single(problems);
        Assert.Equal(LegacyImportProblemCode.InvalidValue, problem.Code);
        Assert.Equal("F", problem.Path);
    }

    [Fact]
    public void TryImport_BadField_OtherFieldsStillImport()
    {
        LegacyFieldMapping[] map =
        {
            Field("A", LegacySettingPart.Bool, "KA"),
            Field("B", LegacySettingPart.FloatValue, "KB"),
        };
        List<LegacyImportProblem> problems = new();

        JObject sections = Import("{\"A\":[],\"B\":4}", map, problems);

        Assert.False(((JObject)sections["G"]).ContainsKey("KA"));
        Assert.Equal(4f, (float)sections["G"]["KB"]["Value"]);
        Assert.Equal("A", Assert.Single(problems).Path);
    }

    [Theory]
    [InlineData(null, LegacyImportProblemCode.EmptyText)]
    [InlineData("", LegacyImportProblemCode.EmptyText)]
    [InlineData("   ", LegacyImportProblemCode.EmptyText)]
    [InlineData("{ bad", LegacyImportProblemCode.InvalidJson)]
    [InlineData("{} x", LegacyImportProblemCode.InvalidJson)]
    [InlineData("null", LegacyImportProblemCode.RootNotObject)]
    [InlineData("[]", LegacyImportProblemCode.RootNotObject)]
    [InlineData("5", LegacyImportProblemCode.RootNotObject)]
    public void TryImport_WholeFileFailure_ReturnsFalse(
        string text,
        LegacyImportProblemCode expected
    )
    {
        List<LegacyImportProblem> problems = new();

        bool ok = LegacySettingsImporter.TryImport(
            text,
            new[] { Field("F", LegacySettingPart.Bool) },
            new[] { "G" },
            new FakeKeyNames(),
            out JObject sections,
            problems
        );

        Assert.False(ok);
        Assert.Empty(sections);
        LegacyImportProblem problem = Assert.Single(problems);
        Assert.Equal(expected, problem.Code);
        Assert.Equal(string.Empty, problem.Path);
    }

    [Fact]
    public void TryImport_MissingField_EmitsNothing()
    {
        List<LegacyImportProblem> problems = new();

        JObject sections = Import("{}", new[] { Field("F", LegacySettingPart.Bool) }, problems);

        Assert.Empty(sections);
        Assert.Empty(problems);
    }

    [Fact]
    public void TryImport_IntermediateNotObject_EmitsNothing()
    {
        List<LegacyImportProblem> problems = new();

        JObject sections = Import(
            "{\"A\":5}",
            new[] { Field("A.B", LegacySettingPart.Bool) },
            problems
        );

        Assert.Empty(sections);
        Assert.Empty(problems);
    }

    [Fact]
    public void TryImport_PathCaseInsensitive()
    {
        JObject sections = Import(
            "{\"a\":{\"b\":true}}",
            new[] { Field("A.B", LegacySettingPart.Bool) },
            new List<LegacyImportProblem>()
        );

        Assert.True((bool)sections["G"]["K"]);
    }

    [Fact]
    public void TryImport_PathExactMatchWins()
    {
        JObject sections = Import(
            "{\"a\":{\"b\":false},\"A\":{\"B\":true}}",
            new[] { Field("A.B", LegacySettingPart.Bool) },
            new List<LegacyImportProblem>()
        );

        Assert.True((bool)sections["G"]["K"]);
    }

    [Theory]
    [InlineData("{\"F\":true} /* c */")]
    [InlineData("{\"F\":true} // note")]
    public void TryImport_TrailingComment_Accepted(string text)
    {
        List<LegacyImportProblem> problems = new();

        bool ok = LegacySettingsImporter.TryImport(
            text,
            new[] { Field("F", LegacySettingPart.Bool) },
            new[] { "G" },
            new FakeKeyNames(),
            out JObject sections,
            problems
        );

        Assert.True(ok);
        Assert.Empty(problems);
        Assert.True((bool)sections["G"]["K"]);
    }

    [Fact]
    public void TryImport_OnlyRequestedGroups()
    {
        LegacyFieldMapping[] map =
        {
            Field("A", LegacySettingPart.Bool, group: "G1"),
            Field("B", LegacySettingPart.Bool, group: "G2"),
        };

        JObject sections = Import(
            "{\"A\":true,\"B\":true}",
            map,
            new List<LegacyImportProblem>(),
            new[] { "G1" }
        );

        Assert.True(sections.ContainsKey("G1"));
        Assert.False(sections.ContainsKey("G2"));
    }

    [Fact]
    public void TryImport_GroupWithoutFields_NoSection()
    {
        LegacyFieldMapping[] map =
        {
            Field("A", LegacySettingPart.Bool, group: "G1"),
            Field("B", LegacySettingPart.Bool, group: "G2"),
        };

        JObject sections = Import("{\"A\":true}", map, new List<LegacyImportProblem>());

        Assert.False(sections.ContainsKey("G2"));
    }

    [Fact]
    public void TryImport_Problems_AppendedNotCleared()
    {
        LegacyImportProblem existing = new(LegacyImportProblemCode.InvalidValue, "old", "m");
        List<LegacyImportProblem> problems = new() { existing };

        Import(Wrap("[]"), new[] { Field("F", LegacySettingPart.Bool) }, problems);

        Assert.Equal(2, problems.Count);
        Assert.Equal(existing, problems[0]);
    }

    private static LegacyFieldMapping Field(
        string path,
        LegacySettingPart part,
        string key = "K",
        string group = "G",
        float minLimit = 0f,
        float maxLimit = 0f
    )
    {
        return new LegacyFieldMapping(path, group, key, part, minLimit, maxLimit);
    }

    private static string Wrap(string raw)
    {
        return "{\"F\":" + raw + "}";
    }

    private static JObject Import(
        string json,
        LegacyFieldMapping[] map,
        List<LegacyImportProblem> problems,
        string[] groups = null
    )
    {
        groups ??= map.Select(m => m.Group).Distinct().ToArray();
        LegacySettingsImporter.TryImport(
            json,
            map,
            groups,
            new FakeKeyNames(),
            out JObject sections,
            problems
        );
        return sections;
    }

    private static JObject ImportOne(string raw, LegacyFieldMapping mapping)
    {
        return Import(Wrap(raw), new[] { mapping }, new List<LegacyImportProblem>());
    }

    private static LegacyFieldMapping[] RangeMap(float minLimit = 0f, float maxLimit = 0f)
    {
        return new[]
        {
            Field("E", LegacySettingPart.RangeEnabled),
            Field("Mn", LegacySettingPart.RangeMin, minLimit: minLimit, maxLimit: maxLimit),
            Field("Mx", LegacySettingPart.RangeMax, minLimit: minLimit, maxLimit: maxLimit),
        };
    }
}
