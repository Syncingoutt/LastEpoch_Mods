using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Core.ModUI.Sliders;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class ScenesMonolithsMapTests
{
    private const string Path = "Scenes.Monoliths";

    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[ScenesMonolithsMap.Group];
        Assert.Equal(11, section.Count);
        Assert.All(
            new[]
            {
                "MaxStabilityOnStart",
                "MaxStabilityOnStabilityChanged",
                "ObjectiveReveal",
                "DropShadeGauntlet",
                "CompleteObjective",
                "NoLostWhenDie",
                "Islands",
            },
            key => Assert.False((bool)section[key])
        );
        Assert.All(
            new (string Key, float Value)[]
            {
                ("MaxStability", 0f),
                ("MobsDensity", 0f),
                ("MobsDefeatOnStart", 0f),
                ("BlessingSlots", 3f),
            },
            row =>
            {
                Assert.False((bool)section[row.Key]["Enabled"]);
                Assert.Equal(row.Value, (float)section[row.Key]["Value"]);
            }
        );
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("MaxStabilityOnStart")]
    [InlineData("MaxStabilityOnStabilityChanged")]
    [InlineData("ObjectiveReveal")]
    [InlineData("DropShadeGauntlet")]
    [InlineData("CompleteObjective")]
    [InlineData("NoLostWhenDie")]
    [InlineData("Islands")]
    public void Bool_MapsToItsKey(string key)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap(Path, ("Enable_" + key, true)), problems);

        var section = (JObject)sections[ScenesMonolithsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value);
        Assert.Empty(problems);
    }

    [Fact]
    public void Bool_CarriesExplicitFalse()
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(Path, ("Enable_Islands", false)),
            new List<LegacyImportProblem>()
        );

        Assert.False((bool)sections[ScenesMonolithsMap.Group]["Islands"]);
    }

    [Theory]
    [InlineData("MaxStability", 100)]
    [InlineData("MobsDensity", 50)]
    [InlineData("MobsDefeatOnStart", 20)]
    [InlineData("BlessingSlots", 4)]
    public void Float_MapsEnabledAndValue(string name, int value)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(
            LegacyMapImport.Wrap(Path, ("Enable_" + name, true), (name, value)),
            problems
        );

        var section = (JObject)sections[ScenesMonolithsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(name, only.Name);
        Assert.True((bool)only.Value["Enabled"]);
        Assert.Equal((float)value, (float)only.Value["Value"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("MaxStability", "MonolithMaxStability")]
    [InlineData("MobsDensity", "MonolithMobsDensity")]
    [InlineData("MobsDefeatOnStart", "MonolithMobsDefeat")]
    [InlineData("BlessingSlots", "MonolithBlessingSlots")]
    public void Float_UsesSliderLimits(string name, string limitsField)
    {
        SliderRange? found = SliderLimitsReflection.Find(typeof(SceneSliderLimits), limitsField);
        Assert.True(found.HasValue, "Limits field missing: " + limitsField);
        SliderRange range = found.Value;

        JObject below = Import(
            LegacyMapImport.Wrap(Path, (name, range.Min - 1f)),
            new List<LegacyImportProblem>()
        );
        JObject above = Import(
            LegacyMapImport.Wrap(Path, (name, range.Max + 1f)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(range.Min, (float)below[ScenesMonolithsMap.Group][name]["Value"]);
        Assert.Equal(range.Max, (float)above[ScenesMonolithsMap.Group][name]["Value"]);
    }

    [Theory]
    [InlineData("BlessingSlots", 0, 3f)]
    [InlineData("BlessingSlots", 9, 5f)]
    [InlineData("MaxStability", 300, 255f)]
    [InlineData("MobsDensity", -5, 0f)]
    public void Float_ClampsToLimits(string name, int stored, float expected)
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(Path, (name, stored)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(expected, (float)sections[ScenesMonolithsMap.Group][name]["Value"]);
    }

    [Fact]
    public void Float_KeepsFraction()
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(Path, ("MobsDefeatOnStart", 12.5d)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(
            12.5f,
            (float)sections[ScenesMonolithsMap.Group]["MobsDefeatOnStart"]["Value"]
        );
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            "{\"Scenes\":{\"Monoliths\":{\"BlessingSlots\":4}}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[ScenesMonolithsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("BlessingSlots", only.Name);
        Assert.Equal(4f, (float)only.Value["Value"]);
        Assert.Null(only.Value["Enabled"]);
    }

    [Theory]
    [InlineData("Scenes.Monoliths", "Monoliths_Corruption")]
    [InlineData("Scenes.Monoliths.Corruptions", "Enable_Monoliths_Corruptions")]
    [InlineData("Scenes.Monoliths.Corruptions", "Fall_Of_The_Outcast")]
    [InlineData("Scenes.Camera", "Enable_Mod")]
    [InlineData("Scenes.Dungeons", "Enable_EnterWithoutKey")]
    public void UnreadFields_EmitNothing(string path, string field)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap(path, (field, 1)), problems);

        var section = sections[ScenesMonolithsMap.Group] as JObject;
        Assert.True(section == null || section.Count == 0);
        Assert.Empty(problems);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = ScenesMonolithsMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(
            text,
            ScenesMonolithsMap.All,
            ScenesMonolithsMap.Group,
            problems
        );
    }
}
