using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Core.ModUI.Sliders;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class CharacterCheatsMapTests
{
    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[CharacterCheatsMap.Group];
        Assert.Equal(6, section.Count);
        Assert.Equal(5, section.Properties().Count(p => p.Value.Type == JTokenType.Boolean));
        Assert.All(
            section.Properties().Where(p => p.Value.Type == JTokenType.Boolean),
            p => Assert.False((bool)p.Value)
        );
        Assert.False((bool)section["AutoPot"]["Enabled"]);
        Assert.Equal(0f, (float)section["AutoPot"]["Value"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("Enable_GodMode", "GodMode")]
    [InlineData("Enable_LowLife", "LowLife")]
    [InlineData("Enable_UnlockAllIdolsSlots", "UnlockAllIdolsSlots")]
    [InlineData("Enable_WaypointsUnlock", "WaypointsUnlock")]
    [InlineData("Enable_TwoHandedWithShield", "TwoHandedWithShield")]
    public void Bool_MapsToItsKey(string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(Cheats((legacyName, true)), problems);

        var section = (JObject)sections[CharacterCheatsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value);
        Assert.Empty(problems);
    }

    [Fact]
    public void AutoPot_MapsEnabledAndValue()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(Cheats(("Enable_AutoPot", true), ("autoPot", 3)), problems);

        var section = (JObject)sections[CharacterCheatsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("AutoPot", only.Name);
        Assert.True((bool)only.Value["Enabled"]);
        Assert.Equal(3f, (float)only.Value["Value"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData(300, 255f)]
    [InlineData(-1, 0f)]
    public void AutoPot_ClampsToLimits(int stored, float expected)
    {
        JObject sections = Import(Cheats(("autoPot", stored)), new List<LegacyImportProblem>());

        Assert.Equal(expected, (float)sections[CharacterCheatsMap.Group]["AutoPot"]["Value"]);
    }

    [Fact]
    public void AutoPot_UsesSliderLimits()
    {
        SliderRange? found = SliderLimitsReflection.Find(
            typeof(CharacterSliderLimits),
            "AutoPotion"
        );
        Assert.True(found.HasValue, "Limits field missing: AutoPotion");
        SliderRange range = found.Value;

        Assert.Equal(range.Min, ImportedAutoPot(range.Min - 1d));
        Assert.Equal(range.Max, ImportedAutoPot(range.Max + 1d));
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            "{\"Character\":{\"Cheats\":{\"autoPot\":100.5}}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[CharacterCheatsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("AutoPot", only.Name);
        Assert.Equal(100.5f, (float)only.Value["Value"]);
    }

    [Fact]
    public void CanChooseBlessing_IsDropped()
    {
        JObject sections = Import(
            Cheats(("Enable_CanChooseBlessing", true)),
            new List<LegacyImportProblem>()
        );

        JToken section = sections[CharacterCheatsMap.Group];
        Assert.True(section == null || !section.HasValues);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = CharacterCheatsMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static string Cheats(params (string Name, object Value)[] fields)
    {
        return LegacyMapImport.Wrap("Character.Cheats", fields);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(
            text,
            CharacterCheatsMap.All,
            CharacterCheatsMap.Group,
            problems
        );
    }

    private static float ImportedAutoPot(double stored)
    {
        JObject sections = Import(Cheats(("autoPot", stored)), new List<LegacyImportProblem>());
        return (float)sections[CharacterCheatsMap.Group]["AutoPot"]["Value"];
    }
}
