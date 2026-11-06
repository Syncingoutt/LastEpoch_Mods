using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class ModsNotInHudMapTests
{
    private const string Path = "modsNotInHud";

    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[ModsNotInHudMap.Group];
        Assert.Equal(3, section.Count);
        Assert.False((bool)section["PotionReplenishment"]);
        Assert.False((bool)section["ShrinesUnlimited"]);
        Assert.False((bool)section["ShrinesOverride"]["Enabled"]);
        Assert.Equal(0f, (float)section["ShrinesOverride"]["Value"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("Enable_PotionResplenishment", "PotionReplenishment", true)]
    [InlineData("Enable_PotionResplenishment", "PotionReplenishment", false)]
    [InlineData("Shrines_Unlimited", "ShrinesUnlimited", true)]
    [InlineData("Shrines_Unlimited", "ShrinesUnlimited", false)]
    public void Bool_MapsToItsKey(string legacyName, string key, bool value)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap(Path, (legacyName, value)), problems);

        var section = (JObject)sections[ModsNotInHudMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.Equal(value, (bool)only.Value);
        Assert.Empty(problems);
    }

    [Fact]
    public void ShrinesOverride_MapsEnabledAndValue()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(
            LegacyMapImport.Wrap(Path, ("Shrines_Override", true), ("Shrines_Override_id", 7)),
            problems
        );

        var section = (JObject)sections[ModsNotInHudMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("ShrinesOverride", only.Name);
        Assert.True((bool)only.Value["Enabled"]);
        Assert.Equal(7f, (float)only.Value["Value"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData(99)]
    [InlineData(-1)]
    [InlineData(100000)]
    [InlineData(3.5)]
    public void ShrineId_IsNotClamped(double stored)
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(Path, ("Shrines_Override_id", stored)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(
            (float)stored,
            (float)sections[ModsNotInHudMap.Group]["ShrinesOverride"]["Value"]
        );
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(Path, ("Shrines_Override_id", 12)),
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[ModsNotInHudMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("ShrinesOverride", only.Name);
        Assert.Equal(12f, (float)only.Value["Value"]);
        Assert.Null(only.Value["Enabled"]);
    }

    [Fact]
    public void DroppedFields_EmitNothing()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(
            LegacyMapImport.Wrap(
                Path,
                ("Craft_Seal_Tier", 3),
                ("Craft_No_Forgin_Potencial_Cost", false)
            ),
            problems
        );

        Assert.Null(sections[ModsNotInHudMap.Group]);
        Assert.Empty(problems);
    }

    [Fact]
    public void Map_HasNoCraftFields()
    {
        Assert.DoesNotContain(ModsNotInHudMap.All, m => m.LegacyPath.Contains("Craft_"));
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = ModsNotInHudMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(text, ModsNotInHudMap.All, ModsNotInHudMap.Group, problems);
    }
}
