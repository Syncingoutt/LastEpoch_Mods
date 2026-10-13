using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class DamageMeterMapTests
{
    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[DamageMeterMap.Group];
        Assert.Equal(2, section.Count);
        Assert.Equal(JTokenType.Integer, section["DamageType"].Type);
        Assert.Equal(0, (int)section["DamageType"]);
        Assert.False((bool)section["SeparateHitAndDot"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData(2, 2)]
    [InlineData(5, 2)]
    [InlineData(-1, 0)]
    public void Dropdown_ClampsToLastOption(int raw, int expected)
    {
        JObject sections = Import(
            LegacyMapImport.Wrap("DamageMeter", ("DamageType", raw)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(expected, (int)sections[DamageMeterMap.Group]["DamageType"]);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Bool_CarriesValue(bool value)
    {
        JObject sections = Import(
            LegacyMapImport.Wrap("DamageMeter", ("SeparateHitAndDot", value)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(value, (bool)sections[DamageMeterMap.Group]["SeparateHitAndDot"]);
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            "{\"DamageMeter\":{\"DamageType\":1}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[DamageMeterMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("DamageType", only.Name);
    }

    [Fact]
    public void OtherSections_EmitNothing()
    {
        JObject sections = Import(
            LegacyMapImport.Wrap("KeyBinds", ("DamageMeter", 288)),
            new List<LegacyImportProblem>()
        );

        Assert.Null(sections[DamageMeterMap.Group]);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = DamageMeterMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(text, DamageMeterMap.All, DamageMeterMap.Group, problems);
    }
}
