using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Core.ModUI.Sliders;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class ScenesCameraMapTests
{
    private const string Path = "Scenes.Camera";

    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[ScenesCameraMap.Group];
        Assert.Equal(10, section.Count);
        Assert.All(new[] { "EnableMod", "LoadOnStart" }, key => Assert.False((bool)section[key]));
        Assert.All(
            new (string Key, float Value)[]
            {
                ("ZoomMinimum", -7f),
                ("ZoomPerScroll", 0f),
                ("ZoomSpeed", 2f),
                ("DefaultRotation", 95f),
                ("OffsetMinimum", -1f),
                ("OffsetMaximum", 0f),
                ("AngleMinimum", 35f),
                ("AngleMaximum", 49f),
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
    [InlineData("Enable_Mod", "EnableMod")]
    [InlineData("Enable_LoadOnStart", "LoadOnStart")]
    public void Bool_MapsToItsKey(string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap(Path, (legacyName, true)), problems);

        var section = (JObject)sections[ScenesCameraMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("ZoomMinimum", -3)]
    [InlineData("ZoomPerScroll", 3)]
    [InlineData("ZoomSpeed", 3)]
    [InlineData("DefaultRotation", 3)]
    [InlineData("OffsetMinimum", 3)]
    [InlineData("OffsetMaximum", 3)]
    [InlineData("AngleMinimum", 3)]
    [InlineData("AngleMaximum", 3)]
    public void Float_MapsEnabledAndValue(string name, int value)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(
            LegacyMapImport.Wrap(Path, ("Enable_" + name, true), (name, value)),
            problems
        );

        var section = (JObject)sections[ScenesCameraMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(name, only.Name);
        Assert.True((bool)only.Value["Enabled"]);
        Assert.Equal((float)value, (float)only.Value["Value"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("ZoomMinimum", "CameraZoomMinimum")]
    [InlineData("ZoomPerScroll", "CameraZoomPerScroll")]
    [InlineData("ZoomSpeed", "CameraZoomSpeed")]
    [InlineData("DefaultRotation", "CameraDefaultRotation")]
    [InlineData("OffsetMinimum", "CameraOffsetMinimum")]
    [InlineData("OffsetMaximum", "CameraOffsetMaximum")]
    [InlineData("AngleMinimum", "CameraAngleMinimum")]
    [InlineData("AngleMaximum", "CameraAngleMaximum")]
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

        Assert.Equal(range.Min, (float)below[ScenesCameraMap.Group][name]["Value"]);
        Assert.Equal(range.Max, (float)above[ScenesCameraMap.Group][name]["Value"]);
    }

    [Theory]
    [InlineData("ZoomMinimum", 5, 0f)]
    [InlineData("ZoomMinimum", -300, -255f)]
    [InlineData("ZoomPerScroll", 25, 20f)]
    [InlineData("AngleMaximum", 300, 255f)]
    public void Float_ClampsToLimits(string name, int stored, float expected)
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(Path, (name, stored)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(expected, (float)sections[ScenesCameraMap.Group][name]["Value"]);
    }

    [Fact]
    public void Float_KeepsFraction()
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(Path, ("OffsetMaximum", 0.31d)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(0.31f, (float)sections[ScenesCameraMap.Group]["OffsetMaximum"]["Value"]);
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            "{\"Scenes\":{\"Camera\":{\"ZoomSpeed\":3.5}}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[ScenesCameraMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("ZoomSpeed", only.Name);
        Assert.Equal(3.5f, (float)only.Value["Value"]);
        Assert.Null(only.Value["Enabled"]);
    }

    [Theory]
    [InlineData("Scenes.Minimap", "Enable_MaxZoomOut")]
    [InlineData("Scenes.Dungeons", "Enable_EnterWithoutKey")]
    public void OtherScenesFields_EmitNothing(string path, string field)
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(path, (field, true)),
            new List<LegacyImportProblem>()
        );

        var section = sections[ScenesCameraMap.Group] as JObject;
        Assert.True(section == null || section.Count == 0);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = ScenesCameraMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(text, ScenesCameraMap.All, ScenesCameraMap.Group, problems);
    }
}
