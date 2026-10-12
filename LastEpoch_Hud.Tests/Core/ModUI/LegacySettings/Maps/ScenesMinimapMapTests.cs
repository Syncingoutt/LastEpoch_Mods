using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class ScenesMinimapMapTests
{
    private const string Path = "Scenes.Minimap";

    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[ScenesMinimapMap.Group];
        Assert.Equal(2, section.Count);
        Assert.All(
            new[] { "MaxZoomOut", "RemoveFogOfWar" },
            key => Assert.False((bool)section[key])
        );
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("Enable_MaxZoomOut", "MaxZoomOut")]
    [InlineData("Enable_RemoveFogOfWar", "RemoveFogOfWar")]
    public void Bool_MapsToItsKey(string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap(Path, (legacyName, true)), problems);

        var section = (JObject)sections[ScenesMinimapMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value);
        Assert.Empty(problems);
    }

    [Fact]
    public void Bool_CarriesExplicitFalse()
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(Path, ("Enable_RemoveFogOfWar", false)),
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[ScenesMinimapMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("RemoveFogOfWar", only.Name);
        Assert.False((bool)only.Value);
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            "{\"Scenes\":{\"Minimap\":{\"Enable_MaxZoomOut\":true}}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[ScenesMinimapMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("MaxZoomOut", only.Name);
    }

    [Theory]
    [InlineData("Scenes.Dungeons", "Enable_EnterWithoutKey")]
    [InlineData("Scenes.Camera", "Enable_Mod")]
    public void OtherScenesFields_EmitNothing(string path, string field)
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(path, (field, true)),
            new List<LegacyImportProblem>()
        );

        var section = sections[ScenesMinimapMap.Group] as JObject;
        Assert.True(section == null || section.Count == 0);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = ScenesMinimapMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(text, ScenesMinimapMap.All, ScenesMinimapMap.Group, problems);
    }
}
