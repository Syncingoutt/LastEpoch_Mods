using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class SkillsMovementMapTests
{
    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[SkillsMovementMap.Group];
        Assert.Equal(3, section.Count);
        Assert.All(
            new[] { "NoTarget", "ImmuneDuringMovement", "IgnoreSimplePath" },
            key => Assert.False((bool)section[key])
        );
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("Enable_NoTarget", "NoTarget")]
    [InlineData("Enable_ImmuneDuringMovement", "ImmuneDuringMovement")]
    [InlineData("Disable_SimplePath", "IgnoreSimplePath")]
    public void Bool_MapsToItsKey(string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(Movement((legacyName, true)), problems);

        var section = (JObject)sections[SkillsMovementMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value);
        Assert.Empty(problems);
    }

    [Fact]
    public void Bool_CarriesExplicitFalse()
    {
        JObject sections = Import(
            Movement(("Disable_SimplePath", false)),
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[SkillsMovementMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("IgnoreSimplePath", only.Name);
        Assert.False((bool)only.Value);
    }

    [Fact]
    public void RootSkillsBool_EmitsNothing()
    {
        JObject sections = Import(
            LegacyMapImport.Wrap("Skills", ("Enable_RemoveManaCost", true)),
            new List<LegacyImportProblem>()
        );

        var section = sections[SkillsMovementMap.Group] as JObject;
        Assert.True(section == null || section.Count == 0);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = SkillsMovementMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static string Movement(params (string Name, object Value)[] fields)
    {
        return LegacyMapImport.Wrap("Skills.MovementSkills", fields);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(
            text,
            SkillsMovementMap.All,
            SkillsMovementMap.Group,
            problems
        );
    }
}
