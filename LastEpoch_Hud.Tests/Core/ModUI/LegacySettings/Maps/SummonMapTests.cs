using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class SummonMapTests
{
    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[SummonMap.Group];
        Assert.Equal(3, section.Count);
        Assert.All(section.Properties(), p => Assert.False((bool)p.Value));
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("Enable_GodMode", "GodMode")]
    [InlineData("Enable_Forever", "Forever")]
    [InlineData("Enable_DontCollide", "DontCollide")]
    public void Bool_MapsToItsKey(string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap("Summon", (legacyName, true)), problems);

        var section = (JObject)sections[SummonMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value);
        Assert.Empty(problems);
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            "{\"Summon\":{\"Enable_Forever\":true}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[SummonMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("Forever", only.Name);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = SummonMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(text, SummonMap.All, SummonMap.Group, problems);
    }
}
