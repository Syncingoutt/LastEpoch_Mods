using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Core.ModUI.Sliders;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class SkillsMinionsMapTests
{
    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[SkillsMinionsMap.Group];
        Assert.Equal(40, section.Count);
        var bools = section.Properties().Where(p => p.Value.Type == JTokenType.Boolean).ToList();
        var floats = section.Properties().Where(p => p.Value.Type == JTokenType.Object).ToList();
        Assert.Equal(15, bools.Count);
        Assert.Equal(25, floats.Count);
        Assert.All(bools, p => Assert.False((bool)p.Value));
        Assert.All(
            floats,
            p =>
            {
                Assert.False((bool)p.Value["Enabled"]);
                Assert.Equal(0f, (float)p.Value["Value"]);
            }
        );
        Assert.Empty(problems);
    }

    [Theory]
    [MemberData(nameof(SkillsMinionsRows.Bools), MemberType = typeof(SkillsMinionsRows))]
    public void Bool_MapsToItsKey(string sectionPath, string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap(sectionPath, (legacyName, true)), problems);

        var section = (JObject)sections[SkillsMinionsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value);
        Assert.Empty(problems);
    }

    [Theory]
    [MemberData(nameof(SkillsMinionsRows.Floats), MemberType = typeof(SkillsMinionsRows))]
    public void Float_MapsEnabledAndValue(
        string sectionPath,
        string enabledName,
        string valueName,
        string key
    )
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(
            LegacyMapImport.Wrap(sectionPath, (enabledName, true), (valueName, 3)),
            problems
        );

        var section = (JObject)sections[SkillsMinionsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value["Enabled"]);
        Assert.Equal(3f, (float)only.Value["Value"]);
        Assert.Empty(problems);
    }

    [Theory]
    [MemberData(nameof(SkillsMinionsRows.FloatValues), MemberType = typeof(SkillsMinionsRows))]
    public void Float_UsesSliderLimits(string sectionPath, string valueName, string key)
    {
        SliderRange? found = SliderLimitsReflection.Find(typeof(SkillSliderLimits), key);
        Assert.True(found.HasValue, "Limits field missing: " + key);
        SliderRange range = found.Value;

        JObject below = Import(
            LegacyMapImport.Wrap(sectionPath, (valueName, range.Min - 1f)),
            new List<LegacyImportProblem>()
        );
        JObject above = Import(
            LegacyMapImport.Wrap(sectionPath, (valueName, range.Max + 1f)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(range.Min, (float)below[SkillsMinionsMap.Group][key]["Value"]);
        Assert.Equal(range.Max, (float)above[SkillsMinionsMap.Group][key]["Value"]);
    }

    [Theory]
    [InlineData(
        "Skills.Minions.Skeletons",
        "additionalSkeletonsPerCast",
        "SkeletonPerCast",
        300,
        255f
    )]
    [InlineData("Skills.Minions.Wraiths", "increasedCastSpeed", "WraithCastSpeed", -1, 0f)]
    [InlineData("Skills.Minions.DreadShades", "max", "DreadShadeMaximum", 300, 255f)]
    public void Float_ClampsToLimits(
        string sectionPath,
        string valueName,
        string key,
        int stored,
        float expected
    )
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(sectionPath, (valueName, stored)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(expected, (float)sections[SkillsMinionsMap.Group][key]["Value"]);
    }

    [Fact]
    public void Float_KeepsFraction()
    {
        JObject sections = Import(
            LegacyMapImport.Wrap("Skills.Minions.Skeletons", ("additionalSkeletonsPerCast", 2.5d)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(2.5f, (float)sections[SkillsMinionsMap.Group]["SkeletonPerCast"]["Value"]);
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            "{\"Skills\":{\"Minions\":{\"DreadShades\":{\"max\":7}}}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[SkillsMinionsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("DreadShadeMaximum", only.Name);
        Assert.Equal(7f, (float)only.Value["Value"]);
        Assert.Null(only.Value["Enabled"]);
    }

    [Theory]
    [InlineData("Skills.Minions.Mages", "Enable_onlySummonOneMage")]
    [InlineData("Skills.Companion.Wolf", "Enable_SummonMax")]
    public void OtherFields_EmitNothing(string path, string field)
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(path, (field, true)),
            new List<LegacyImportProblem>()
        );

        var section = sections[SkillsMinionsMap.Group] as JObject;
        Assert.True(section == null || section.Count == 0);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = SkillsMinionsMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(text, SkillsMinionsMap.All, SkillsMinionsMap.Group, problems);
    }
}
