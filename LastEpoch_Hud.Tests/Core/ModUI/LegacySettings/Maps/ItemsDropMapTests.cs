using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Core.ModUI.Sliders;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class ItemsDropMapTests
{
    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[ItemsDropMap.Group];
        Assert.Equal(15, section.Count);
        Assert.All(
            new[] { "ForceUnique", "ForceSet", "ForceLegendary", "ForceSeal" },
            key => Assert.False((bool)section[key])
        );
        AssertRange(section, "Implicits", false, 0f, 255f);
        AssertRange(section, "ForgingPotential", false, 0f, 255f);
        AssertRange(section, "SealTier", false, 0f, 6f);
        AssertRange(section, "SealValue", false, 0f, 255f);
        AssertRange(section, "AffixCount", false, 0f, 4f);
        AssertRange(section, "IdolAffixCount", false, 0f, 2f);
        AssertRange(section, "AffixTiers", false, 0f, 6f);
        AssertRange(section, "AffixValues", false, 0f, 255f);
        AssertRange(section, "UniqueMods", false, 0f, 255f);
        AssertRange(section, "LegendaryPotential", false, 0f, 4f);
        AssertRange(section, "WeaverWill", false, 0f, 28f);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("ForceUnique", "ForceUnique")]
    [InlineData("ForceSet", "ForceSet")]
    [InlineData("ForceLegendary", "ForceLegendary")]
    [InlineData("ForceSeal", "ForceSeal")]
    public void Bool_MapsToItsKey(string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(Drop(("Enable_" + legacyName, true)), problems);

        var section = (JObject)sections[ItemsDropMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("Implicits", "Implicits")]
    [InlineData("ForginPotencial", "ForgingPotential")]
    [InlineData("SealTier", "SealTier")]
    [InlineData("SealValue", "SealValue")]
    [InlineData("AffixCount", "AffixCount")]
    [InlineData("IdolAffixCount", "IdolAffixCount")]
    [InlineData("AffixTiers", "AffixTiers")]
    [InlineData("AffixValues", "AffixValues")]
    [InlineData("UniqueMods", "UniqueMods")]
    [InlineData("LegendaryPotencial", "LegendaryPotential")]
    [InlineData("WeaverWill", "WeaverWill")]
    public void Range_MapsToItsKey(string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();
        string json = Drop(
            ("Enable_" + legacyName, true),
            (legacyName + "_Min", 1),
            (legacyName + "_Max", 2)
        );

        JObject sections = Import(json, problems);

        var section = (JObject)sections[ItemsDropMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        AssertRange(section, key, true, 1f, 2f);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("Implicits", "Implicits", nameof(ItemSliderLimits.DropImplicits))]
    [InlineData(
        "ForginPotencial",
        "ForgingPotential",
        nameof(ItemSliderLimits.DropForgingPotential)
    )]
    [InlineData("SealTier", "SealTier", nameof(ItemSliderLimits.DropSealTier))]
    [InlineData("SealValue", "SealValue", nameof(ItemSliderLimits.DropSealValue))]
    [InlineData("AffixCount", "AffixCount", nameof(ItemSliderLimits.DropAffixCount))]
    [InlineData("IdolAffixCount", "IdolAffixCount", nameof(ItemSliderLimits.DropIdolAffixCount))]
    [InlineData("AffixTiers", "AffixTiers", nameof(ItemSliderLimits.DropAffixTiers))]
    [InlineData("AffixValues", "AffixValues", nameof(ItemSliderLimits.DropAffixValues))]
    [InlineData("UniqueMods", "UniqueMods", nameof(ItemSliderLimits.DropUniqueMods))]
    [InlineData(
        "LegendaryPotencial",
        "LegendaryPotential",
        nameof(ItemSliderLimits.DropLegendaryPotential)
    )]
    [InlineData("WeaverWill", "WeaverWill", nameof(ItemSliderLimits.DropWeaverWill))]
    public void Range_ClampsToOwnLimits(string legacyName, string key, string limitsField)
    {
        SliderRange? found = SliderLimitsReflection.Find(typeof(ItemSliderLimits), limitsField);
        Assert.True(found.HasValue, $"Limits field missing: {limitsField}");
        SliderRange range = found.Value;

        Assert.Equal(range.Min, Imported(legacyName + "_Min", key, "Min", range.Min - 1d));
        Assert.Equal(range.Max, Imported(legacyName + "_Max", key, "Max", range.Max + 1d));
        Assert.Equal(range.Max, Imported(legacyName + "_Min", key, "Min", range.Max + 1d));
        Assert.Equal(range.Min, Imported(legacyName + "_Max", key, "Max", range.Min - 1d));
    }

    [Fact]
    public void AffixCountMax_UsesLimitSix()
    {
        Assert.Equal(5f, Imported("AffixCount_Max", "AffixCount", "Max", 5d));
    }

    [Fact]
    public void IdolAffixCountMax_ClampsToTwo()
    {
        Assert.Equal(2f, Imported("IdolAffixCount_Max", "IdolAffixCount", "Max", 3d));
    }

    [Fact]
    public void Fraction_IsKept()
    {
        Assert.Equal(3.5f, Imported("SealTier_Max", "SealTier", "Max", 3.5d));
    }

    [Fact]
    public void PartialFile_EmitsOnlyPresent()
    {
        JObject sections = Import(
            "{\"Items\":{\"Drop\":{\"SealValue_Max\":100}}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[ItemsDropMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("SealValue", only.Name);
        var sealValue = (JObject)only.Value;
        JProperty max = Assert.Single(sealValue.Properties());
        Assert.Equal("Max", max.Name);
        Assert.Equal(100f, (float)max.Value);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = ItemsDropMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static void AssertRange(JObject section, string key, bool enabled, float min, float max)
    {
        Assert.Equal(enabled, (bool)section[key]["Enabled"]);
        Assert.Equal(min, (float)section[key]["Min"]);
        Assert.Equal(max, (float)section[key]["Max"]);
    }

    private static string Drop(params (string Name, object Value)[] fields)
    {
        JObject drop = new();
        foreach ((string name, object value) in fields)
        {
            drop[name] = JToken.FromObject(value);
        }
        return new JObject { ["Items"] = new JObject { ["Drop"] = drop } }.ToString();
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        Assert.True(
            LegacySettingsImporter.TryImport(
                text,
                ItemsDropMap.All,
                new[] { ItemsDropMap.Group },
                new FakeKeyNames(),
                out JObject sections,
                problems
            )
        );
        return sections;
    }

    private static float Imported(string legacyField, string key, string part, double stored)
    {
        JObject sections = Import(Drop((legacyField, stored)), new List<LegacyImportProblem>());
        return (float)sections[ItemsDropMap.Group][key][part];
    }
}
