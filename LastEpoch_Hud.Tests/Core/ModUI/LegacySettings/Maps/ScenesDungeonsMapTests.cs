using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class ScenesDungeonsMapTests
{
    private const string Path = "Scenes.Dungeons";

    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[ScenesDungeonsMap.Group];
        Assert.Single(section.Properties());
        Assert.False((bool)section["EnterWithoutKey"]);
        Assert.Empty(problems);
    }

    [Fact]
    public void Bool_MapsToItsKey()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(
            LegacyMapImport.Wrap(Path, ("Enable_EnterWithoutKey", true)),
            problems
        );

        var section = (JObject)sections[ScenesDungeonsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("EnterWithoutKey", only.Name);
        Assert.True((bool)only.Value);
        Assert.Empty(problems);
    }

    [Fact]
    public void Bool_CarriesExplicitFalse()
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(Path, ("Enable_EnterWithoutKey", false)),
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[ScenesDungeonsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("EnterWithoutKey", only.Name);
        Assert.False((bool)only.Value);
    }

    [Theory]
    [InlineData("Scenes.Minimap", "Enable_MaxZoomOut")]
    [InlineData("Scenes.Camera", "Enable_Mod")]
    public void OtherScenesFields_EmitNothing(string path, string field)
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(path, (field, true)),
            new List<LegacyImportProblem>()
        );

        var section = sections[ScenesDungeonsMap.Group] as JObject;
        Assert.True(section == null || section.Count == 0);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = ScenesDungeonsMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(
            text,
            ScenesDungeonsMap.All,
            ScenesDungeonsMap.Group,
            problems
        );
    }
}
