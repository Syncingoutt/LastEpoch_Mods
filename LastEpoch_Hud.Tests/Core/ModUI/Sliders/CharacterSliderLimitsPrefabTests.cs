using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Support;

namespace LastEpoch_Hud.Tests.Core.ModUI.Sliders;

public sealed class CharacterSliderLimitsPrefabTests
{
    [Theory]
    [InlineData(nameof(CharacterSliderLimits.FactionFavor), "Slider_Character_Factions_Favor")]
    [InlineData(nameof(CharacterSliderLimits.FactionRank), "Slider_Character_Factions_Rank")]
    [InlineData(
        nameof(CharacterSliderLimits.FactionReputation),
        "Slider_Character_Factions_Reputation"
    )]
    [InlineData(nameof(CharacterSliderLimits.BuffMoveSpeed), "Slider_Character_Buffs_MoveSpeed")]
    [InlineData(nameof(CharacterSliderLimits.BuffCastSpeed), "Slider_Character_Buffs_CastingSpeed")]
    [InlineData(
        nameof(CharacterSliderLimits.BuffAttackSpeed),
        "Slider_Character_Buffs_AttackSpeed"
    )]
    [InlineData(nameof(CharacterSliderLimits.BuffDamage), "Slider_Character_Buffs_Damage")]
    [InlineData(nameof(CharacterSliderLimits.BuffManaRegen), "Slider_Character_Buffs_ManaRegen")]
    [InlineData(
        nameof(CharacterSliderLimits.BuffHealthRegen),
        "Slider_Character_Buffs_HealthRegen"
    )]
    [InlineData(
        nameof(CharacterSliderLimits.BuffCriticalChance),
        "Slider_Character_Buffs_CriticalChance"
    )]
    [InlineData(
        nameof(CharacterSliderLimits.BuffCriticalMultiplier),
        "Slider_Character_Buffs_CriticalMultiplier"
    )]
    [InlineData(nameof(CharacterSliderLimits.BuffStrength), "Slider_Character_Buffs_Strenght")]
    [InlineData(
        nameof(CharacterSliderLimits.BuffIntelligence),
        "Slider_Character_Buffs_Intelligence"
    )]
    [InlineData(nameof(CharacterSliderLimits.BuffDexterity), "Slider_Character_Buffs_Dexterity")]
    [InlineData(nameof(CharacterSliderLimits.BuffVitality), "Slider_Character_Buffs_Vitality")]
    [InlineData(nameof(CharacterSliderLimits.BuffAttunement), "Slider_Character_Buffs_Attunement")]
    public void Range_MatchesPrefab(string field, string slider)
    {
        IReadOnlyDictionary<string, SliderRange> prefab = PrefabSliders.Load(
            PrefabSliders.HudPrefabPath
        );

        Assert.True(prefab.ContainsKey(slider), $"Prefab slider missing: {slider}");
        SliderRange? actual = SliderLimitsReflection.Find(typeof(CharacterSliderLimits), field);
        Assert.True(actual.HasValue, $"Limits field missing: {field}");
        Assert.Equal(prefab[slider], actual.Value);
    }
}
