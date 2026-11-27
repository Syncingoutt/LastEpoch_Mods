using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class MjolnerMapTests
{
    private const string Path = "Items.Mjolner";

    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[MjolnerMap.Group];
        Assert.Equal(12, section.Count);
        Assert.True((bool)section["EnableMod"]);
        Assert.True((bool)section["UniqueDrop"]);
        Assert.False((bool)section["WeaverWill"]);
        Assert.True((bool)section["ProcAnyLightningSpell"]);
        Assert.Equal(0.3f, (float)section["MinTriggerChance"]["Value"]);
        Assert.Equal(0.5f, (float)section["MaxTriggerChance"]["Value"]);
        Assert.Equal(95f, (float)section["IntRequirement"]["Value"]);
        Assert.Equal(105f, (float)section["StrRequirement"]["Value"]);
        Assert.Equal(250f, (float)section["SocketedCooldown"]["Value"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void DefaultFixture_GivesSkillNamesFromFixture(int index)
    {
        var fixture = JObject.Parse(LegacyDefaultSave.Text);
        string expected = (string)fixture["Items"]["Mjolner"]["SockectedSkill_" + index];

        JObject sections = Import(LegacyDefaultSave.Text, new List<LegacyImportProblem>());

        JToken skill = sections[MjolnerMap.Group]["SocketedSkill" + index];
        Assert.Equal(JTokenType.String, skill.Type);
        Assert.Equal(expected, (string)skill);
    }

    [Theory]
    [InlineData("enable", "EnableMod", true)]
    [InlineData("enable", "EnableMod", false)]
    [InlineData("UniqueDrop", "UniqueDrop", true)]
    [InlineData("UniqueDrop", "UniqueDrop", false)]
    [InlineData("WeaverWill", "WeaverWill", true)]
    [InlineData("WeaverWill", "WeaverWill", false)]
    [InlineData("ProcAnyLightningSpell", "ProcAnyLightningSpell", true)]
    [InlineData("ProcAnyLightningSpell", "ProcAnyLightningSpell", false)]
    public void Bool_MapsToItsKey(string legacyName, string key, bool value)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap(Path, (legacyName, value)), problems);

        var section = (JObject)sections[MjolnerMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.Equal(value, (bool)only.Value);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("MinTriggerChance", 2.5)]
    [InlineData("IntRequirement", -3)]
    [InlineData("StrRequirement", 100000)]
    [InlineData("SocketedCooldown", 100)]
    public void Number_IsNotClamped(string name, double raw)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap(Path, (name, raw)), problems);

        var section = (JObject)sections[MjolnerMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(name, only.Name);
        Assert.Equal((float)raw, (float)only.Value["Value"]);
        Assert.Null(only.Value["Enabled"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData(0, "Skill A")]
    [InlineData(1, "Skill A")]
    [InlineData(2, "Skill A")]
    [InlineData(0, "")]
    [InlineData(1, "")]
    [InlineData(2, "")]
    public void SkillName_MapsToText(int index, string raw)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(
            LegacyMapImport.Wrap(Path, ("SockectedSkill_" + index, raw)),
            problems
        );

        var section = (JObject)sections[MjolnerMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("SocketedSkill" + index, only.Name);
        Assert.Equal(raw, (string)only.Value);
        Assert.Empty(problems);
    }

    [Fact]
    public void SkillName_NotString_RecordsProblem()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap(Path, ("SockectedSkill_0", 5)), problems);

        Assert.Null(sections[MjolnerMap.Group]);
        LegacyImportProblem problem = Assert.Single(problems);
        Assert.Equal(LegacyImportProblemCode.InvalidValue, problem.Code);
        Assert.Equal("Items.Mjolner.SockectedSkill_0", problem.Path);
    }

    [Fact]
    public void BaseDrop_EmitsNothing()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap(Path, ("BaseDrop", false)), problems);

        Assert.Null(sections[MjolnerMap.Group]);
        Assert.Empty(problems);
    }

    [Fact]
    public void Map_HasNoBaseDrop()
    {
        Assert.DoesNotContain(
            MjolnerMap.All,
            m => m.LegacyPath.EndsWith("BaseDrop", StringComparison.Ordinal)
        );
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            LegacyMapImport.Wrap(Path, ("SocketedCooldown", 900)),
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[MjolnerMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("SocketedCooldown", only.Name);
        Assert.Equal(900f, (float)only.Value["Value"]);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = MjolnerMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(text, MjolnerMap.All, MjolnerMap.Group, problems);
    }
}
