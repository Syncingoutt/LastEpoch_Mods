using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Core.ModUI.Sliders;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class SkillsGeneralMapTests
{
    private static readonly string[] _boolKeys =
    {
        "RemoveManaCost",
        "RemoveChannelCost",
        "ManaRegenWhileChanneling",
        "ContinueWhenOutOfMana",
        "RemoveCooldown",
        "UnlockAllSkills",
        "RemoveNodeRequirements",
    };

    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[SkillsGeneralMap.Group];
        Assert.Equal(12, section.Count);
        Assert.All(_boolKeys, key => Assert.False((bool)section[key]));
        Assert.All(
            new[] { "SpecializationSlots", "SkillLevel", "PassivePoints" },
            key => AssertFloat(section, key, 0f)
        );
        Assert.All(
            new[] { "SkillLevelMultiplier", "PassivePointMultiplier" },
            key => AssertFloat(section, key, 2f)
        );
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("Enable_RemoveManaCost", "RemoveManaCost")]
    [InlineData("Enable_RemoveChannelCost", "RemoveChannelCost")]
    [InlineData("Enable_NoManaRegenWhileChanneling", "ManaRegenWhileChanneling")]
    [InlineData("Enable_StopWhenOutOfMana", "ContinueWhenOutOfMana")]
    [InlineData("Enable_RemoveCooldown", "RemoveCooldown")]
    [InlineData("Enable_AllSkills", "UnlockAllSkills")]
    [InlineData("Disable_NodeRequirement", "RemoveNodeRequirements")]
    public void Bool_MapsToItsKey(string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(Skills((legacyName, true)), problems);

        var section = (JObject)sections[SkillsGeneralMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("SpecializationSlots")]
    [InlineData("SkillLevel")]
    [InlineData("SkillLevelMultiplier")]
    [InlineData("PassivePoints")]
    [InlineData("PassivePointMultiplier")]
    public void Float_MapsEnabledAndValue(string name)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(Skills(("Enable_" + name, true), (name, 3)), problems);

        var section = (JObject)sections[SkillsGeneralMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(name, only.Name);
        Assert.True((bool)only.Value["Enabled"]);
        Assert.Equal(3f, (float)only.Value["Value"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("SpecializationSlots", "SpecializationSlots")]
    [InlineData("SkillLevel", "SkillLevel")]
    [InlineData("PassivePoints", "PassivePoints")]
    [InlineData("SkillLevelMultiplier", "PointMultiplier")]
    [InlineData("PassivePointMultiplier", "PointMultiplier")]
    public void Float_UsesSliderLimits(string name, string limitsField)
    {
        SliderRange? found = SliderLimitsReflection.Find(typeof(SkillSliderLimits), limitsField);
        Assert.True(found.HasValue, "Limits field missing: " + limitsField);
        SliderRange range = found.Value;

        JObject below = Import(Skills((name, range.Min - 1f)), new List<LegacyImportProblem>());
        JObject above = Import(Skills((name, range.Max + 1f)), new List<LegacyImportProblem>());

        Assert.Equal(range.Min, (float)below[SkillsGeneralMap.Group][name]["Value"]);
        Assert.Equal(range.Max, (float)above[SkillsGeneralMap.Group][name]["Value"]);
    }

    [Theory]
    [InlineData("SpecializationSlots", 9, 5f)]
    [InlineData("SkillLevel", 300, 255f)]
    [InlineData("SkillLevel", -1, 0f)]
    [InlineData("PassivePoints", 300, 255f)]
    [InlineData("SkillLevelMultiplier", 20, 10f)]
    [InlineData("PassivePointMultiplier", -1, 0f)]
    public void Float_ClampsToLimits(string name, int stored, float expected)
    {
        JObject sections = Import(Skills((name, stored)), new List<LegacyImportProblem>());

        Assert.Equal(expected, (float)sections[SkillsGeneralMap.Group][name]["Value"]);
    }

    [Theory]
    [InlineData("SkillLevel", 12.5d)]
    [InlineData("SpecializationSlots", 2.5d)]
    public void Float_KeepsFraction(string name, double stored)
    {
        JObject sections = Import(Skills((name, stored)), new List<LegacyImportProblem>());

        Assert.Equal((float)stored, (float)sections[SkillsGeneralMap.Group][name]["Value"]);
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            "{\"Skills\":{\"PassivePoints\":7}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[SkillsGeneralMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("PassivePoints", only.Name);
        Assert.Equal(7f, (float)only.Value["Value"]);
        Assert.Null(only.Value["Enabled"]);
    }

    [Theory]
    [InlineData("Skills.SkillTree", "Enable_Slots")]
    [InlineData("Skills.SkillTree", "Slots")]
    [InlineData("Skills.SkillTree", "Enable_Level")]
    [InlineData("Skills.SkillTree", "Level")]
    [InlineData("Skills.PassiveTree", "Enable_PointsEarnt")]
    [InlineData("Skills.PassiveTree", "PointsEarnt")]
    [InlineData("Skills.Companion", "Enable_Companions")]
    [InlineData("Skills.Companion", "Maximum")]
    public void UnmappedSubsections_EmitNothing(string path, string field)
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(path, (field, true)),
            new List<LegacyImportProblem>()
        );

        var section = sections[SkillsGeneralMap.Group] as JObject;
        Assert.True(section == null || section.Count == 0);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = SkillsGeneralMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static void AssertFloat(JObject section, string key, float value)
    {
        Assert.False((bool)section[key]["Enabled"]);
        Assert.Equal(value, (float)section[key]["Value"]);
    }

    private static string Skills(params (string Name, object Value)[] fields)
    {
        return LegacyMapImport.Wrap("Skills", fields);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(text, SkillsGeneralMap.All, SkillsGeneralMap.Group, problems);
    }
}
