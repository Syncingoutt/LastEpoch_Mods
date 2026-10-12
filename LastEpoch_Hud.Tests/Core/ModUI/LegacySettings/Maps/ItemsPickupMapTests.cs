using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Core.ModUI.Sliders;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class ItemsPickupMapTests
{
    private const string Configured = "AutoShatter_Configured";

    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[ItemsPickupMap.Group];
        Assert.Equal(20, section.Count);
        Assert.Equal(16, section.Properties().Count(p => p.Value.Type == JTokenType.Boolean));
        Assert.All(
            section.Properties().Where(p => p.Value.Type == JTokenType.Boolean),
            p => Assert.False((bool)p.Value)
        );
        Assert.False((bool)section["AutoStoreTimer"]["Enabled"]);
        Assert.Equal(10f, (float)section["AutoStoreTimer"]["Value"]);
        Assert.Equal(100f, (float)section["AutoShatterChance"]["Value"]);
        Assert.Equal(100f, (float)section["AutoShatterAffixChance"]["Value"]);
        Assert.Equal(0f, (float)section["AutoShatterQuantityChance"]["Value"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("Enable_AutoPickup_Gold", "AutoPickupGold")]
    [InlineData("Enable_AutoPickup_Keys", "AutoPickupKeys")]
    [InlineData("Enable_AutoPickup_Potions", "AutoPickupPotions")]
    [InlineData("Enable_AutoPickup_XpTome", "AutoPickupXpTome")]
    [InlineData("Enable_AutoPickup_FavorTome", "AutoPickupFavorTome")]
    [InlineData("Enable_AutoPickup_MemoryAmber", "AutoPickupMemoryAmber")]
    [InlineData("Enable_AutoPickup_Materials", "AutoPickupMaterials")]
    [InlineData("Enable_AutoPickup_WovenEchoes", "AutoPickupWovenEchoes")]
    [InlineData("Enable_AutoPickup_FromFilter", "AutoPickupFromFilter")]
    [InlineData("Enable_AutoStore_OnDrop", "AutoStoreOnDrop")]
    [InlineData("Enable_AutoStore_OnInventoryOpen", "AutoStoreOnInventoryOpen")]
    [InlineData("Enable_AutoSell_FromFilter", "AutoSellFromFilter")]
    [InlineData("Enable_AutoShatter_FromFilter", "AutoShatterFromFilter")]
    [InlineData("Enable_AutoShatter_UseRune", "AutoShatterUseRune")]
    [InlineData("Enable_RangePickup", "RangePickup")]
    [InlineData("Enable_HideMaterialsNotifications", "HideMaterialsNotifications")]
    public void Bool_MapsToItsKey(string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(Pickup((legacyName, true)), problems);

        var section = (JObject)sections[ItemsPickupMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.True((bool)only.Value);
        Assert.Empty(problems);
    }

    [Fact]
    public void AutoStoreTimer_MapsToItsKey()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(
            Pickup(("Enable_AutoStore_Timer", true), ("AutoStore_Timer", 5)),
            problems
        );

        var section = (JObject)sections[ItemsPickupMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("AutoStoreTimer", only.Name);
        Assert.True((bool)only.Value["Enabled"]);
        Assert.Equal(5f, (float)only.Value["Value"]);
        Assert.Empty(problems);
    }

    [Fact]
    public void AutoStoreTimer_HasNoLimits()
    {
        JObject sections = Import(
            Pickup(("AutoStore_Timer", 3600)),
            new List<LegacyImportProblem>()
        );

        Assert.Equal(3600f, (float)sections[ItemsPickupMap.Group]["AutoStoreTimer"]["Value"]);
    }

    [Theory]
    [InlineData("AutoShatter_Chance", "AutoShatterChance")]
    [InlineData("AutoShatter_AffixChance", "AutoShatterAffixChance")]
    [InlineData("AutoShatter_QuantityChance", "AutoShatterQuantityChance")]
    public void Chance_Configured_MapsToItsKey(string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(Pickup((Configured, true), (legacyName, 40)), problems);

        var section = (JObject)sections[ItemsPickupMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        Assert.Equal(40f, (float)only.Value["Value"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData(
        "AutoShatter_Chance",
        "AutoShatterChance",
        nameof(ItemSliderLimits.AutoShatterChance)
    )]
    [InlineData(
        "AutoShatter_AffixChance",
        "AutoShatterAffixChance",
        nameof(ItemSliderLimits.AutoShatterAffixChance)
    )]
    [InlineData(
        "AutoShatter_QuantityChance",
        "AutoShatterQuantityChance",
        nameof(ItemSliderLimits.AutoShatterQuantityChance)
    )]
    public void Chance_ClampsToOwnLimits(string legacyName, string key, string limitsField)
    {
        SliderRange? found = SliderLimitsReflection.Find(typeof(ItemSliderLimits), limitsField);
        Assert.True(found.HasValue, $"Limits field missing: {limitsField}");
        SliderRange range = found.Value;

        Assert.Equal(range.Min, ImportedChance(legacyName, key, range.Min - 1d));
        Assert.Equal(range.Max, ImportedChance(legacyName, key, range.Max + 1d));
    }

    [Fact]
    public void Chance_ConfiguredFalse_EmitsNoChanceKeys()
    {
        string json = Pickup(
            (Configured, false),
            ("AutoShatter_Chance", 40),
            ("AutoShatter_AffixChance", 40),
            ("AutoShatter_QuantityChance", 40)
        );

        AssertNoChanceKeys(json);
    }

    [Fact]
    public void Chance_ConfiguredMissing_EmitsNoChanceKeys()
    {
        string json = Pickup(
            ("AutoShatter_Chance", 40),
            ("AutoShatter_AffixChance", 40),
            ("AutoShatter_QuantityChance", 40)
        );

        AssertNoChanceKeys(json);
    }

    [Fact]
    public void Chance_Fraction_IsKept()
    {
        Assert.Equal(37.5f, ImportedChance("AutoShatter_Chance", "AutoShatterChance", 37.5d));
    }

    [Fact]
    public void PartialFile_EmitsOnlyPresent()
    {
        JObject sections = Import(
            "{\"Items\":{\"Pickup\":{\"Enable_RangePickup\":true}}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[ItemsPickupMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("RangePickup", only.Name);
        Assert.True((bool)only.Value);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = ItemsPickupMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static void AssertNoChanceKeys(string json)
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(json, problems);

        JToken section = sections[ItemsPickupMap.Group];
        Assert.Null(section?["AutoShatterChance"]);
        Assert.Null(section?["AutoShatterAffixChance"]);
        Assert.Null(section?["AutoShatterQuantityChance"]);
        Assert.Empty(problems);
    }

    private static string Pickup(params (string Name, object Value)[] fields)
    {
        return LegacyMapImport.Wrap("Items.Pickup", fields);
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        return LegacyMapImport.Import(text, ItemsPickupMap.All, ItemsPickupMap.Group, problems);
    }

    private static float ImportedChance(string legacyField, string key, double stored)
    {
        JObject sections = Import(
            Pickup((Configured, true), (legacyField, stored)),
            new List<LegacyImportProblem>()
        );
        return (float)sections[ItemsPickupMap.Group][key]["Value"];
    }
}
