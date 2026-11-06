using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Core.ModUI.Sliders;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class PermanentBuffsMapTests
{
    private const string EnablePrefix = "Enable_";

    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[PermanentBuffsMap.Group];
        Assert.Equal(15, section.Count);
        Assert.True((bool)section["EnableMod"]);
        string[] one =
        {
            "MoveSpeed",
            "CastSpeed",
            "AttackSpeed",
            "Damage",
            "ManaRegen",
            "HealthRegen",
            "AreaOfEffect",
        };
        string[] zero =
        {
            "CriticalChance",
            "CriticalMultiplier",
            "Strength",
            "Intelligence",
            "Dexterity",
            "Vitality",
            "Attunement",
        };
        Assert.All(one, key => AssertBuff(section, key, false, 1f));
        Assert.All(zero, key => AssertBuff(section, key, false, 0f));
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("MoveSpeed", "MoveSpeed")]
    [InlineData("CastSpeed", "CastSpeed")]
    [InlineData("AttackSpeed", "AttackSpeed")]
    [InlineData("Damage", "Damage")]
    [InlineData("ManaRegen", "ManaRegen")]
    [InlineData("HealthRegen", "HealthRegen")]
    [InlineData("CriticalChance", "CriticalChance")]
    [InlineData("CriticalMultiplier", "CriticalMultiplier")]
    [InlineData("Str", "Strength")]
    [InlineData("Int", "Intelligence")]
    [InlineData("Dex", "Dexterity")]
    [InlineData("Vit", "Vitality")]
    [InlineData("Att", "Attunement")]
    [InlineData("AoE", "AreaOfEffect")]
    public void Field_MapsToItsKey(string legacyName, string key)
    {
        var problems = new List<LegacyImportProblem>();
        string json = Buffs(
            (EnablePrefix + legacyName + "_Buff", true),
            (legacyName + "_Buff_Value", 0.37)
        );

        JObject sections = Import(json, problems);

        var section = (JObject)sections[PermanentBuffsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal(key, only.Name);
        AssertBuff(section, key, true, 0.37f);
        Assert.Empty(problems);
    }

    [Fact]
    public void EnableMod_MapsToEnableMod()
    {
        JObject sections = Import(
            Buffs((EnablePrefix + "Mod", false)),
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[PermanentBuffsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("EnableMod", only.Name);
        Assert.False((bool)only.Value);
    }

    [Theory]
    [InlineData("MoveSpeed", "MoveSpeed", 50.0, 20f)]
    [InlineData("Str", "Strength", 300.0, 255f)]
    [InlineData("CriticalChance", "CriticalChance", 1.5, 0.95f)]
    [InlineData("AoE", "AreaOfEffect", -1.0, 0f)]
    public void Value_ClampsToLimits(string legacyName, string key, double stored, float expected)
    {
        JObject sections = Import(
            Buffs((legacyName + "_Buff_Value", stored)),
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[PermanentBuffsMap.Group];
        Assert.Equal(expected, (float)section[key]["Value"]);
    }

    [Theory]
    [InlineData("MoveSpeed", "MoveSpeed", nameof(CharacterSliderLimits.BuffMoveSpeed))]
    [InlineData("CastSpeed", "CastSpeed", nameof(CharacterSliderLimits.BuffCastSpeed))]
    [InlineData("AttackSpeed", "AttackSpeed", nameof(CharacterSliderLimits.BuffAttackSpeed))]
    [InlineData("Damage", "Damage", nameof(CharacterSliderLimits.BuffDamage))]
    [InlineData("ManaRegen", "ManaRegen", nameof(CharacterSliderLimits.BuffManaRegen))]
    [InlineData("HealthRegen", "HealthRegen", nameof(CharacterSliderLimits.BuffHealthRegen))]
    [InlineData(
        "CriticalChance",
        "CriticalChance",
        nameof(CharacterSliderLimits.BuffCriticalChance)
    )]
    [InlineData(
        "CriticalMultiplier",
        "CriticalMultiplier",
        nameof(CharacterSliderLimits.BuffCriticalMultiplier)
    )]
    [InlineData("Str", "Strength", nameof(CharacterSliderLimits.BuffStrength))]
    [InlineData("Int", "Intelligence", nameof(CharacterSliderLimits.BuffIntelligence))]
    [InlineData("Dex", "Dexterity", nameof(CharacterSliderLimits.BuffDexterity))]
    [InlineData("Vit", "Vitality", nameof(CharacterSliderLimits.BuffVitality))]
    [InlineData("Att", "Attunement", nameof(CharacterSliderLimits.BuffAttunement))]
    [InlineData("AoE", "AreaOfEffect", nameof(CharacterSliderLimits.BuffAreaOfEffect))]
    public void Value_ClampsToOwnRange(string legacyName, string key, string limitsField)
    {
        SliderRange range = FindRange(limitsField);

        Assert.Equal(range.Max, ImportedValue(legacyName, key, range.Max + 1d));
        Assert.Equal(range.Min, ImportedValue(legacyName, key, range.Min - 1d));
    }

    [Fact]
    public void PartialFile_EmitsOnlyPresent()
    {
        JObject sections = Import(
            "{\"Character\":{\"PermanentBuffs\":{\"Damage_Buff_Value\":3.5}}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[PermanentBuffsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("Damage", only.Name);
        var damage = (JObject)only.Value;
        JProperty value = Assert.Single(damage.Properties());
        Assert.Equal("Value", value.Name);
        Assert.Equal(3.5f, (float)value.Value);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = PermanentBuffsMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static void AssertBuff(JObject section, string key, bool enabled, float value)
    {
        Assert.Equal(enabled, (bool)section[key]["Enabled"]);
        Assert.Equal(value, (float)section[key]["Value"]);
    }

    private static string Buffs(params (string Name, object Value)[] fields)
    {
        JObject buffs = new();
        foreach ((string name, object value) in fields)
        {
            buffs[name] = JToken.FromObject(value);
        }
        return new JObject
        {
            ["Character"] = new JObject { ["PermanentBuffs"] = buffs },
        }.ToString();
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        Assert.True(
            LegacySettingsImporter.TryImport(
                text,
                PermanentBuffsMap.All,
                new[] { PermanentBuffsMap.Group },
                new FakeKeyNames(),
                out JObject sections,
                problems
            )
        );
        return sections;
    }

    private static SliderRange FindRange(string limitsField)
    {
        SliderRange? range = SliderLimitsReflection.Find(
            typeof(CharacterSliderLimits),
            limitsField
        );
        Assert.True(range.HasValue, $"Limits field missing: {limitsField}");
        return range.Value;
    }

    private static float ImportedValue(string legacyName, string key, double stored)
    {
        JObject sections = Import(
            Buffs((legacyName + "_Buff_Value", stored)),
            new List<LegacyImportProblem>()
        );
        return (float)sections[PermanentBuffsMap.Group][key]["Value"];
    }
}
