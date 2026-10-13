using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class KeyBindsMapTests
{
    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[KeyBindsMap.Group];
        Assert.Equal(2, section.Count);
        Assert.Equal("kb:F3", (string)section["BankStashs"]);
        Assert.Equal("kb:F7", (string)section["DamageMeter"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("BankStashs", 97)]
    [InlineData("DamageMeter", 97)]
    [InlineData("BankStashs", "A")]
    public void Keybind_MapsToItsKey(string name, object raw)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap("KeyBinds", (name, raw)), problems);

        var section = (JObject)sections[KeyBindsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(name, only.Name);
        Assert.Equal("kb:A", (string)only.Value);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("BankStashs")]
    [InlineData("DamageMeter")]
    public void UnboundKey_EmitsNoKey(string name)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyMapImport.Wrap("KeyBinds", (name, 0)), problems);

        Assert.Null(sections[KeyBindsMap.Group]);
        Assert.Empty(problems);
    }

    [Fact]
    public void UnknownCode_RecordsProblem()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(
            LegacyMapImport.Wrap("KeyBinds", ("DamageMeter", 99999)),
            problems
        );

        Assert.Null(sections[KeyBindsMap.Group]);
        LegacyImportProblem problem = Assert.Single(problems);
        Assert.Equal(LegacyImportProblemCode.InvalidValue, problem.Code);
        Assert.Equal("KeyBinds.DamageMeter", problem.Path);
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            "{\"KeyBinds\":{\"DamageMeter\":97}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[KeyBindsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("DamageMeter", only.Name);
    }

    [Fact]
    public void OtherSections_EmitNothing()
    {
        JObject sections = Import(
            LegacyMapImport.Wrap("DamageMeter", ("DamageType", 1)),
            new List<LegacyImportProblem>()
        );

        Assert.Null(sections[KeyBindsMap.Group]);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = KeyBindsMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(text, KeyBindsMap.All, KeyBindsMap.Group, problems);
    }
}
