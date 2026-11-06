using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Core.ModUI.Sliders;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class SkillsCompanionsMapTests
{
    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[SkillsCompanionsMap.Group];
        Assert.Equal(5, section.Count);
        Assert.All(
            new[] { "WolfSummonMaximum", "WolfStunImmunity" },
            key => Assert.False((bool)section[key])
        );
        Assert.All(
            new[] { "MaximumCompanions", "WolfSummonLimit", "ScorpionBabyQuantity" },
            key =>
            {
                Assert.False((bool)section[key]["Enabled"]);
                Assert.Equal(255f, (float)section[key]["Value"]);
            }
        );
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("Skills.Companion.Wolf", "Enable_SummonMax", "WolfSummonMaximum")]
    [InlineData("Skills.Companion.Wolf", "Enable_StunImmunity", "WolfStunImmunity")]
    public void Bool_MapsToItsKey(string sectionPath, string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap(sectionPath, (legacyName, true)), problems);

        var section = (JObject)sections[SkillsCompanionsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("Skills.Companion", "Enable_Limit", "Limit", "MaximumCompanions")]
    [InlineData("Skills.Companion.Wolf", "Enable_SummonLimit", "SummonLimit", "WolfSummonLimit")]
    [InlineData(
        "Skills.Companion.Scorpion",
        "Enable_BabyQuantity",
        "BabyQuantity",
        "ScorpionBabyQuantity"
    )]
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

        var section = (JObject)sections[SkillsCompanionsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value["Enabled"]);
        Assert.Equal(3f, (float)only.Value["Value"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("Skills.Companion", "Limit", "MaximumCompanions", "CompanionsMaximum")]
    [InlineData("Skills.Companion.Wolf", "SummonLimit", "WolfSummonLimit", "WolfSummonLimit")]
    [InlineData(
        "Skills.Companion.Scorpion",
        "BabyQuantity",
        "ScorpionBabyQuantity",
        "ScorpionBabyQuantity"
    )]
    public void Float_UsesSliderLimits(
        string sectionPath,
        string valueName,
        string key,
        string limitsField
    )
    {
        SliderRange? found = SliderLimitsReflection.Find(typeof(SkillSliderLimits), limitsField);
        Assert.True(found.HasValue, "Limits field missing: " + limitsField);
        SliderRange range = found.Value;

        JObject below = Import(
            LegacyMapImport.Wrap(sectionPath, (valueName, range.Min - 1f)),
            new List<LegacyImportProblem>()
        );
        JObject above = Import(
            LegacyMapImport.Wrap(sectionPath, (valueName, range.Max + 1f)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(range.Min, (float)below[SkillsCompanionsMap.Group][key]["Value"]);
        Assert.Equal(range.Max, (float)above[SkillsCompanionsMap.Group][key]["Value"]);
    }

    [Theory]
    [InlineData("Skills.Companion", "Limit", "MaximumCompanions", 300, 255f)]
    [InlineData("Skills.Companion.Wolf", "SummonLimit", "WolfSummonLimit", -1, 0f)]
    [InlineData("Skills.Companion.Scorpion", "BabyQuantity", "ScorpionBabyQuantity", 300, 255f)]
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

        Assert.Equal(expected, (float)sections[SkillsCompanionsMap.Group][key]["Value"]);
    }

    [Fact]
    public void Float_KeepsFraction()
    {
        JObject sections = Import(
            LegacyMapImport.Wrap("Skills.Companion", ("Limit", 12.5d)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(
            12.5f,
            (float)sections[SkillsCompanionsMap.Group]["MaximumCompanions"]["Value"]
        );
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            "{\"Skills\":{\"Companion\":{\"Wolf\":{\"SummonLimit\":7}}}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[SkillsCompanionsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("WolfSummonLimit", only.Name);
        Assert.Equal(7f, (float)only.Value["Value"]);
        Assert.Null(only.Value["Enabled"]);
    }

    [Theory]
    [InlineData("Skills", "Enable_RemoveManaCost")]
    [InlineData("Skills.Minions.Skeletons", "Enable_forceArcher")]
    public void OtherSkillsFields_EmitNothing(string path, string field)
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(path, (field, true)),
            new List<LegacyImportProblem>()
        );

        var section = sections[SkillsCompanionsMap.Group] as JObject;
        Assert.True(section == null || section.Count == 0);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = SkillsCompanionsMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(
            text,
            SkillsCompanionsMap.All,
            SkillsCompanionsMap.Group,
            problems
        );
    }
}
