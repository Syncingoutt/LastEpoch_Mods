using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Core.ModUI.Sliders;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class CharacterMultipliersMapTests
{
    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[CharacterMultipliersMap.Group];
        Assert.Equal(9, section.Count);
        foreach (JProperty property in section.Properties())
        {
            Assert.False((bool)property.Value["Enabled"]);
            Assert.Equal(property.Name == "MemoryAmber" ? 1f : 0f, (float)property.Value["Value"]);
        }
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("DensityMultiplier", "Density")]
    [InlineData("ExperienceMultiplier", "Experience")]
    [InlineData("AbilityMultiplier", "Ability")]
    [InlineData("FavorMultiplier", "Favor")]
    [InlineData("ItemDropChance", "ItemDropChance")]
    [InlineData("ItemDropMultiplier", "ItemDropMultiplier")]
    [InlineData("GoldDropChance", "GoldDropChance")]
    [InlineData("GoldDropMultiplier", "GoldDropMultiplier")]
    [InlineData("MemoryAmberMultiplier", "MemoryAmber")]
    public void Multiplier_MapsEnabledAndValue(string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(
            Cheats(("Enable_" + legacyName, true), (legacyName, 3)),
            problems
        );

        var section = (JObject)sections[CharacterMultipliersMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value["Enabled"]);
        Assert.Equal(3f, (float)only.Value["Value"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData(300, 255f)]
    [InlineData(-1, 0f)]
    public void Multiplier_ClampsToLimits(int stored, float expected)
    {
        JObject sections = Import(
            Cheats(("DensityMultiplier", stored)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(expected, (float)sections[CharacterMultipliersMap.Group]["Density"]["Value"]);
    }

    [Fact]
    public void Multipliers_UseSliderLimits()
    {
        SliderRange? found = SliderLimitsReflection.Find(
            typeof(CharacterSliderLimits),
            "Multiplier"
        );
        Assert.True(found.HasValue, "Limits field missing: Multiplier");
        SliderRange range = found.Value;

        IEnumerable<LegacyFieldMapping> values = CharacterMultipliersMap.All.Where(m =>
            m.Part == LegacySettingPart.FloatValue
        );
        Assert.Equal(9, values.Count());
        Assert.All(
            values,
            m =>
            {
                Assert.Equal(range.Min, m.MinLimit);
                Assert.Equal(range.Max, m.MaxLimit);
            }
        );
    }

    [Fact]
    public void MemoryAmber_ClampsToLimits()
    {
        JObject sections = Import(
            Cheats(("MemoryAmberMultiplier", 300)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(255f, (float)sections[CharacterMultipliersMap.Group]["MemoryAmber"]["Value"]);
    }

    [Fact]
    public void MemoryAmber_KeepsFraction()
    {
        JObject sections = Import(
            Cheats(("MemoryAmberMultiplier", 2.5d)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(2.5f, (float)sections[CharacterMultipliersMap.Group]["MemoryAmber"]["Value"]);
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            "{\"Character\":{\"Cheats\":{\"MemoryAmberMultiplier\":7}}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[CharacterMultipliersMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("MemoryAmber", only.Name);
        Assert.Equal(7f, (float)only.Value["Value"]);
        Assert.Null(only.Value["Enabled"]);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = CharacterMultipliersMap.All.Where(m =>
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
            CharacterMultipliersMap.All,
            CharacterMultipliersMap.Group,
            problems
        );
    }
}
