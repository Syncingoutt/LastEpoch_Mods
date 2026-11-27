using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Core.ModUI.Sliders;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class ItemsCraftingSlotMapTests
{
    private const string Section = "Items.CraftingSlot";
    private const string Prefix = Section + ".";

    private static readonly (string Legacy, string Key, string Limits)[] _pairs = BuildPairs();

    public static IEnumerable<object[]> FieldRows =>
        _pairs.Select(p => new object[] { p.Legacy, p.Key });

    public static IEnumerable<object[]> LimitRows =>
        _pairs.Select(p => new object[] { p.Legacy, p.Limits });

    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[ItemsCraftingSlotMap.Group];
        Assert.Equal(25, section.Count);
        Assert.False((bool)section["EnableMod"]);
        foreach ((string _, string key, string _) in _pairs)
        {
            Assert.False((bool)section[key]["Enabled"]);
            Assert.Equal(ExpectedDefault(key), (float)section[key]["Value"]);
        }
        Assert.Empty(problems);
    }

    [Theory]
    [MemberData(nameof(FieldRows))]
    public void Field_MapsToItsKey(string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();
        string json = LegacyMapImport.Wrap(
            Section,
            ("Enable_" + legacyName, true),
            (legacyName, 3)
        );

        JObject sections = Import(json, problems);

        var section = (JObject)sections[ItemsCraftingSlotMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value["Enabled"]);
        Assert.Equal(3f, (float)only.Value["Value"]);
        Assert.Empty(problems);
    }

    [Fact]
    public void EnableMod_MapsToEnableMod()
    {
        var problems = new List<LegacyImportProblem>();
        string json = LegacyMapImport.Wrap(Section, ("Enable_Mod", true));

        JObject sections = Import(json, problems);

        var section = (JObject)sections[ItemsCraftingSlotMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("EnableMod", only.Name);
        Assert.True((bool)only.Value);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("ForginPotencial", "ForgingPotential", 256d, 255f)]
    [InlineData("Implicit_0", "Implicit0", 300d, 255f)]
    [InlineData("Seal_Value", "SealValue", -1d, 0f)]
    [InlineData("Affix_1_Value", "Affix1Value", 300d, 255f)]
    [InlineData("UniqueMod_7", "UniqueMod7", -5d, 0f)]
    [InlineData("Seal_Tier", "SealTier", 255d, 6f)]
    [InlineData("Affix_0_Tier", "Affix0Tier", 9d, 7f)]
    [InlineData("Affix_3_Tier", "Affix3Tier", -1d, 0f)]
    [InlineData("LegendaryPotencial", "LegendaryPotential", 5d, 4f)]
    [InlineData("WeaverWill", "WeaverWill", 40d, 28f)]
    public void Value_ClampsToLimits(string legacyName, string key, double stored, float expected)
    {
        Assert.Equal(expected, Imported(legacyName, key, stored));
    }

    [Theory]
    [MemberData(nameof(LimitRows))]
    public void Value_UsesSliderLimits(string legacyName, string limitsField)
    {
        SliderRange? found = SliderLimitsReflection.Find(typeof(ItemSliderLimits), limitsField);
        Assert.True(found.HasValue, $"Limits field missing: {limitsField}");
        SliderRange range = found.Value;

        LegacyFieldMapping mapping = ItemsCraftingSlotMap.All.Single(m =>
            m.LegacyPath == Prefix + legacyName && m.Part == LegacySettingPart.FloatValue
        );

        Assert.Equal(range.Min, mapping.MinLimit);
        Assert.Equal(range.Max, mapping.MaxLimit);
    }

    [Theory]
    [InlineData("Seal_Tier", "SealTier", 2.5d)]
    [InlineData("Affix_0_Tier", "Affix0Tier", 3.7d)]
    public void WholeNumberFraction_IsKept(string legacyName, string key, double stored)
    {
        Assert.Equal((float)stored, Imported(legacyName, key, stored));
    }

    [Fact]
    public void PartialFile_EmitsOnlyGivenField()
    {
        JObject sections = Import(
            "{\"Items\":{\"CraftingSlot\":{\"Enable_WeaverWill\":true}}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[ItemsCraftingSlotMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("WeaverWill", only.Name);
        JProperty enabled = Assert.Single(((JObject)only.Value).Properties());
        Assert.Equal("Enabled", enabled.Name);
        Assert.True((bool)enabled.Value);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = ItemsCraftingSlotMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems) =>
        LegacyMapImport.Import(
            text,
            ItemsCraftingSlotMap.All,
            ItemsCraftingSlotMap.Group,
            problems
        );

    private static float Imported(string legacyName, string key, double stored)
    {
        string json = LegacyMapImport.Wrap(Section, (legacyName, stored));
        JObject sections = Import(json, new List<LegacyImportProblem>());
        return (float)sections[ItemsCraftingSlotMap.Group][key]["Value"];
    }

    private static float ExpectedDefault(string key)
    {
        if (key.EndsWith("Tier"))
        {
            return 6f;
        }
        if (key == "LegendaryPotential")
        {
            return 4f;
        }
        return key == "WeaverWill" ? 28f : 255f;
    }

    private static (string Legacy, string Key, string Limits)[] BuildPairs()
    {
        List<(string, string, string)> all = new()
        {
            (
                "ForginPotencial",
                "ForgingPotential",
                nameof(ItemSliderLimits.CraftingSlotForgingPotential)
            ),
            ("Seal_Tier", "SealTier", nameof(ItemSliderLimits.CraftingSlotSealTier)),
            ("Seal_Value", "SealValue", nameof(ItemSliderLimits.CraftingSlotSealValue)),
            (
                "LegendaryPotencial",
                "LegendaryPotential",
                nameof(ItemSliderLimits.CraftingSlotLegendaryPotential)
            ),
            ("WeaverWill", "WeaverWill", nameof(ItemSliderLimits.CraftingSlotWeaverWill)),
        };
        for (int n = 0; n < 3; n++)
        {
            all.Add(
                ($"Implicit_{n}", $"Implicit{n}", nameof(ItemSliderLimits.CraftingSlotImplicit))
            );
        }
        for (int n = 0; n < 4; n++)
        {
            all.Add(
                ($"Affix_{n}_Tier", $"Affix{n}Tier", nameof(ItemSliderLimits.CraftingSlotAffixTier))
            );
            all.Add(
                (
                    $"Affix_{n}_Value",
                    $"Affix{n}Value",
                    nameof(ItemSliderLimits.CraftingSlotAffixValue)
                )
            );
        }
        for (int n = 0; n < 8; n++)
        {
            all.Add(
                ($"UniqueMod_{n}", $"UniqueMod{n}", nameof(ItemSliderLimits.CraftingSlotUniqueMod))
            );
        }
        return all.ToArray();
    }
}
