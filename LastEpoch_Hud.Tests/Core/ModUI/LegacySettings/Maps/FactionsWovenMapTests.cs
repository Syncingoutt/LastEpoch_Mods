using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Core.ModUI.Sliders;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class FactionsWovenMapTests
{
    private const string Path = "Factions.TheWoven";

    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[FactionsWovenMap.Group];
        Assert.Equal(3, section.Count);
        AssertFloat(section, "TreePoints", 0f);
        AssertFloat(section, "PointMultiplier", 2f);
        Assert.False((bool)section["FreeRespec"]);
        Assert.Empty(problems);
    }

    [Fact]
    public void Bool_MapsToItsKey()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(Woven(("Enable_FreeRespe", true)), problems);

        var section = (JObject)sections[FactionsWovenMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("FreeRespec", only.Name);
        Assert.True((bool)only.Value);
        Assert.Empty(problems);
    }

    [Fact]
    public void Bool_CarriesExplicitFalse()
    {
        JObject sections = Import(
            Woven(("Enable_FreeRespe", false)),
            new List<LegacyImportProblem>()
        );

        Assert.False((bool)sections[FactionsWovenMap.Group]["FreeRespec"]);
    }

    [Theory]
    [InlineData("TreePoints", 50)]
    [InlineData("PointMultiplier", 3)]
    public void Float_MapsEnabledAndValue(string name, int value)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(Woven(("Enable_" + name, true), (name, value)), problems);

        var section = (JObject)sections[FactionsWovenMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(name, only.Name);
        Assert.True((bool)only.Value["Enabled"]);
        Assert.Equal((float)value, (float)only.Value["Value"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("TreePoints", "WeaverTreePoints")]
    [InlineData("PointMultiplier", "WeaverPointMultiplier")]
    public void Float_UsesSliderLimits(string name, string limitsField)
    {
        SliderRange? found = SliderLimitsReflection.Find(
            typeof(CharacterSliderLimits),
            limitsField
        );
        Assert.True(found.HasValue, "Limits field missing: " + limitsField);
        SliderRange range = found.Value;

        JObject below = Import(Woven((name, range.Min - 1f)), new List<LegacyImportProblem>());
        JObject above = Import(Woven((name, range.Max + 1f)), new List<LegacyImportProblem>());

        Assert.Equal(range.Min, (float)below[FactionsWovenMap.Group][name]["Value"]);
        Assert.Equal(range.Max, (float)above[FactionsWovenMap.Group][name]["Value"]);
    }

    [Theory]
    [InlineData("TreePoints", 500, 200f)]
    [InlineData("TreePoints", -5, 0f)]
    [InlineData("PointMultiplier", 20, 10f)]
    [InlineData("PointMultiplier", -1, 0f)]
    public void Float_ClampsToLimits(string name, int stored, float expected)
    {
        JObject sections = Import(Woven((name, stored)), new List<LegacyImportProblem>());

        Assert.Equal(expected, (float)sections[FactionsWovenMap.Group][name]["Value"]);
    }

    [Fact]
    public void Float_KeepsFraction()
    {
        JObject sections = Import(Woven(("TreePoints", 12.5d)), new List<LegacyImportProblem>());

        Assert.Equal(12.5f, (float)sections[FactionsWovenMap.Group]["TreePoints"]["Value"]);
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            "{\"Factions\":{\"TheWoven\":{\"PointMultiplier\":4}}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[FactionsWovenMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("PointMultiplier", only.Name);
        Assert.Equal(4f, (float)only.Value["Value"]);
        Assert.Null(only.Value["Enabled"]);
    }

    [Theory]
    [InlineData("Skills", "Enable_PassivePointMultiplier")]
    [InlineData("Items.Pickup", "Enable_AutoPickup_WovenEchoes")]
    public void OtherSections_EmitNothing(string path, string field)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap(path, (field, true)), problems);

        var section = sections[FactionsWovenMap.Group] as JObject;
        Assert.True(section == null || section.Count == 0);
        Assert.Empty(problems);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = FactionsWovenMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static void AssertFloat(JObject section, string key, float value)
    {
        Assert.False((bool)section[key]["Enabled"]);
        Assert.Equal(value, (float)section[key]["Value"]);
    }

    private static string Woven(params (string Name, object Value)[] fields)
    {
        return LegacyMapImport.Wrap(Path, fields);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(text, FactionsWovenMap.All, FactionsWovenMap.Group, problems);
    }
}
