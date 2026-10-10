using System.Collections.Generic;
using System.Linq;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu;

public sealed class MagebloodFileEditorTests
{
    private const string Base =
        """{"version":1,"fakeExtra":{"x":[1,2]},"maxResistances":5,"flasks":[{"name":"FakeBroken"},{"name":"FakeA","stats":[{"stat":"FakeA","increased":20},{"stat":"FakeB","added":2}]},{"name":"FakeB","icon":"FakeIcon","stats":[{"stat":"FakeA","added":1}]},{"name":"FakeCustom","stats":[{"stat":"FakeB","more":-10}],"fakeKey":true}]}""";

    private const string NoFlasks = """{"version":1}""";
    private const string FlasksText = """{"flasks":"x"}""";

    private const string DuplicateFlasks =
        """{"flasks":[{"name":"FakeA","stats":[{"stat":"FakeA","increased":20}]},{"name":"FakeX","stats":[{"stat":"FakeA","added":1}]},{"name":"FakeA","stats":[{"stat":"FakeA","increased":99}]},{"name":"FakeY","stats":[{"stat":"FakeA","added":1}]}]}""";

    private static readonly MagebloodValueTarget _fakeAIncreased = MagebloodValueTarget.ForStat(
        "FakeA",
        "FakeA",
        MagebloodStatField.Increased
    );

    [Fact]
    public void TrySetValue_MaxResPresent_ReplacesOnlyThatValue()
    {
        bool ok = MagebloodFileEditor.TrySetValue(
            Base,
            MagebloodValueTarget.ForMaxResistances(),
            85f,
            out string text
        );

        var expected = JObject.Parse(Base);
        expected["maxResistances"] = 85f;
        var root = JObject.Parse(text);
        Assert.True(ok);
        Assert.True(JToken.DeepEquals(expected, root));
        Assert.Equal(Keys(expected), Keys(root));
    }

    [Fact]
    public void TrySetValue_MaxResAbsent_AddsKey()
    {
        var input = JObject.Parse(Base);
        input.Remove("maxResistances");

        bool ok = MagebloodFileEditor.TrySetValue(
            input.ToString(),
            MagebloodValueTarget.ForMaxResistances(),
            85f,
            out string text
        );

        var expected = JObject.Parse(input.ToString());
        expected["maxResistances"] = 85f;
        Assert.True(ok);
        Assert.True(JToken.DeepEquals(expected, JObject.Parse(text)));
    }

    [Fact]
    public void TrySetValue_MaxResWithoutFlasks_Writes()
    {
        bool ok = MagebloodFileEditor.TrySetValue(
            NoFlasks,
            MagebloodValueTarget.ForMaxResistances(),
            85f,
            out string text
        );

        Assert.True(ok);
        Assert.Equal(85f, (float)JObject.Parse(text)["maxResistances"]);
    }

    [Fact]
    public void TrySetValue_StatPresent_ChangesOnlyThatField()
    {
        bool ok = MagebloodFileEditor.TrySetValue(Base, _fakeAIncreased, 40f, out string text);

        var expected = JObject.Parse(Base);
        expected["flasks"][1]["stats"][0]["increased"] = 40f;
        Assert.True(ok);
        Assert.True(JToken.DeepEquals(expected, JObject.Parse(text)));
    }

    [Theory]
    [InlineData(MagebloodStatField.Added, "added")]
    [InlineData(MagebloodStatField.Increased, "increased")]
    [InlineData(MagebloodStatField.More, "more")]
    public void TrySetValue_StatField_WritesMatchingKeyOnly(MagebloodStatField field, string key)
    {
        var target = MagebloodValueTarget.ForStat("FakeA", "FakeB", field);

        bool ok = MagebloodFileEditor.TrySetValue(Base, target, 7f, out string text);

        var expected = JObject.Parse(Base);
        expected["flasks"][1]["stats"][1][key] = 7f;
        Assert.True(ok);
        Assert.True(JToken.DeepEquals(expected, JObject.Parse(text)));
    }

    [Fact]
    public void TrySetValue_StatAbsent_KeepsEverythingElse()
    {
        var target = MagebloodValueTarget.ForStat("FakeA", "FakeA", MagebloodStatField.More);

        bool ok = MagebloodFileEditor.TrySetValue(Base, target, -15f, out string text);

        var expected = JObject.Parse(Base);
        expected["flasks"][1]["stats"][0]["more"] = -15f;
        Assert.True(ok);
        Assert.True(JToken.DeepEquals(expected, JObject.Parse(text)));
    }

    [Fact]
    public void TrySetValue_BrokenEarlierSameName_EditsKeptFlask()
    {
        const string json =
            """{"flasks":[{"name":"FakeA"},{"name":"FakeA","stats":[{"stat":"FakeA","increased":20}]}]}""";

        bool ok = MagebloodFileEditor.TrySetValue(json, _fakeAIncreased, 40f, out string text);

        var expected = JObject.Parse(json);
        expected["flasks"][1]["stats"][0]["increased"] = 40f;
        Assert.True(ok);
        Assert.True(JToken.DeepEquals(expected, JObject.Parse(text)));
    }

    [Theory]
    [InlineData("FakeA", "FakeCustom")]
    [InlineData("FakeCustom", "FakeA")]
    public void TrySwapFlasks_TwoKept_ExchangesPositions(string first, string second)
    {
        bool ok = MagebloodFileEditor.TrySwapFlasks(Base, first, second, out string text);

        var input = JObject.Parse(Base);
        var root = JObject.Parse(text);
        IEnumerable<string> names = root["flasks"].Select(f => (string)f["name"]);
        Assert.True(ok);
        Assert.Equal(new[] { "FakeBroken", "FakeCustom", "FakeB", "FakeA" }, names);
        Assert.True(JToken.DeepEquals(input["flasks"][1], root["flasks"][3]));
        Assert.True(JToken.DeepEquals(input["flasks"][3], root["flasks"][1]));
        Assert.True(JToken.DeepEquals(input["fakeExtra"], root["fakeExtra"]));
    }

    [Theory]
    [InlineData("", MagebloodValueKind.MaxResistances, null, null, MagebloodStatField.Added, 1f)]
    [InlineData("  ", MagebloodValueKind.MaxResistances, null, null, MagebloodStatField.Added, 1f)]
    [InlineData("{", MagebloodValueKind.MaxResistances, null, null, MagebloodStatField.Added, 1f)]
    [InlineData("[]", MagebloodValueKind.MaxResistances, null, null, MagebloodStatField.Added, 1f)]
    [InlineData(
        NoFlasks,
        MagebloodValueKind.StatValue,
        "FakeA",
        "FakeA",
        MagebloodStatField.Added,
        1f
    )]
    [InlineData(
        FlasksText,
        MagebloodValueKind.StatValue,
        "FakeA",
        "FakeA",
        MagebloodStatField.Added,
        1f
    )]
    [InlineData(
        Base,
        MagebloodValueKind.StatValue,
        "FakeMissing",
        "FakeA",
        MagebloodStatField.Added,
        1f
    )]
    [InlineData(
        Base,
        MagebloodValueKind.StatValue,
        "FakeA",
        "FakeMissing",
        MagebloodStatField.Added,
        1f
    )]
    [InlineData(
        Base,
        MagebloodValueKind.StatValue,
        "FakeBroken",
        "FakeA",
        MagebloodStatField.Added,
        1f
    )]
    [InlineData(
        Base,
        MagebloodValueKind.MaxResistances,
        null,
        null,
        MagebloodStatField.Added,
        float.NaN
    )]
    [InlineData(
        Base,
        MagebloodValueKind.MaxResistances,
        null,
        null,
        MagebloodStatField.Added,
        float.PositiveInfinity
    )]
    [InlineData(
        Base,
        MagebloodValueKind.StatValue,
        "FakeA",
        "FakeA",
        MagebloodStatField.Increased,
        0f
    )]
    [InlineData(Base, MagebloodValueKind.MaxResistances, null, null, MagebloodStatField.Added, -1f)]
    [InlineData(Base, MagebloodValueKind.StatValue, "FakeA", "FakeA", (MagebloodStatField)99, 1f)]
    [InlineData(Base, (MagebloodValueKind)99, null, null, MagebloodStatField.Added, 1f)]
    public void TrySetValue_Refused_ReturnsFalseAndNull(
        string json,
        MagebloodValueKind kind,
        string flask,
        string row,
        MagebloodStatField field,
        float value
    )
    {
        var target = new MagebloodValueTarget(kind, flask, row, field);

        bool ok = MagebloodFileEditor.TrySetValue(json, target, value, out string text);

        Assert.False(ok);
        Assert.Null(text);
    }

    [Theory]
    [InlineData(Base, "FakeA", "FakeA")]
    [InlineData(Base, "FakeA", "FakeMissing")]
    [InlineData(Base, "FakeA", "FakeBroken")]
    [InlineData("{", "FakeA", "FakeB")]
    [InlineData(NoFlasks, "FakeA", "FakeB")]
    [InlineData(FlasksText, "FakeA", "FakeB")]
    [InlineData("", "FakeA", "FakeB")]
    [InlineData("[]", "FakeA", "FakeB")]
    public void TrySwapFlasks_Refused_ReturnsFalseAndNull(string json, string first, string second)
    {
        bool ok = MagebloodFileEditor.TrySwapFlasks(json, first, second, out string text);

        Assert.False(ok);
        Assert.Null(text);
    }

    [Theory]
    [InlineData("FakeA", "FakeY")]
    [InlineData("FakeY", "FakeA")]
    public void TrySwapFlasks_WouldPromoteDuplicate_ReturnsFalse(string first, string second)
    {
        bool ok = MagebloodFileEditor.TrySwapFlasks(
            DuplicateFlasks,
            first,
            second,
            out string text
        );

        Assert.False(ok);
        Assert.Null(text);
    }

    [Fact]
    public void TrySwapFlasks_DuplicateNotCrossed_Swaps()
    {
        bool ok = MagebloodFileEditor.TrySwapFlasks(
            DuplicateFlasks,
            "FakeA",
            "FakeX",
            out string text
        );

        IEnumerable<string> names = JObject.Parse(text)["flasks"].Select(f => (string)f["name"]);
        Assert.True(ok);
        Assert.Equal(new[] { "FakeX", "FakeA", "FakeA", "FakeY" }, names);
    }

    [Fact]
    public void TrySetValue_RoundTrip_ParseShowsEditWithoutProblems()
    {
        const string json =
            """{"version":1,"maxResistances":5,"flasks":[{"name":"FakeA","stats":[{"stat":"FakeA","increased":20}]}]}""";

        bool ok = MagebloodFileEditor.TrySetValue(json, _fakeAIncreased, 40f, out string text);

        MagebloodConfigParseResult result = MagebloodConfigParser.Parse(text);
        Assert.True(ok);
        Assert.Empty(result.Problems);
        Assert.Equal(40f, result.Config.Flasks[0].Stats[0].Increased);
        Assert.Contains("\n", text);
    }

    private static string[] Keys(JObject root)
    {
        return root.Properties().Select(p => p.Name).ToArray();
    }
}
