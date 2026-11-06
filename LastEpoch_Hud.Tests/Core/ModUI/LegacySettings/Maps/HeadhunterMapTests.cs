using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class HeadhunterMapTests
{
    private const string Path = "Items.Headhunter";

    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[HeadhunterMap.Group];
        Assert.Equal(3, section.Count);
        Assert.False((bool)section["WeaverWill"]);
        Assert.True((bool)section["UniqueDrop"]);
        Assert.True((bool)section["BaseDrop"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("WeaverWill", true)]
    [InlineData("WeaverWill", false)]
    [InlineData("UniqueDrop", true)]
    [InlineData("UniqueDrop", false)]
    [InlineData("BaseDrop", true)]
    [InlineData("BaseDrop", false)]
    public void Bool_MapsToItsKey(string name, bool value)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap(Path, (name, value)), problems);

        var section = (JObject)sections[HeadhunterMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(name, only.Name);
        Assert.Equal(value, (bool)only.Value);
        Assert.Empty(problems);
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(Path, ("UniqueDrop", false)),
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[HeadhunterMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("UniqueDrop", only.Name);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = HeadhunterMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(text, HeadhunterMap.All, HeadhunterMap.Group, problems);
    }
}
