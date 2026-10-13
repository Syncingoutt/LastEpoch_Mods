using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Support;

namespace LastEpoch_Hud.Tests.Core.ModUI.Sliders;

public sealed class SkillSliderLimitsPrefabTests
{
    [Theory]
    [InlineData(nameof(SkillSliderLimits.SpecializationSlots), "Slider_SpecializationSlots")]
    [InlineData(nameof(SkillSliderLimits.CompanionsMaximum), "Slider_MaximumCompanions")]
    [InlineData(nameof(SkillSliderLimits.WolfSummonLimit), "Slider_Wolf_SummonLimit")]
    [InlineData(nameof(SkillSliderLimits.ScorpionBabyQuantity), "Slider_Scorpions_SummonLimit")]
    [InlineData(
        nameof(SkillSliderLimits.SkeletonFromPassives),
        "Slider_Skeleteon_SummonQuantityFromPassive"
    )]
    [InlineData(
        nameof(SkillSliderLimits.SkeletonFromSkillTree),
        "Slider_Skeleteon_SummonQuantityFromSkillTree"
    )]
    [InlineData(
        nameof(SkillSliderLimits.SkeletonPerCast),
        "Slider_Skeleteon_SummonQuantityPerCast"
    )]
    [InlineData(
        nameof(SkillSliderLimits.SkeletonResummonChance),
        "Slider_Skeleteon_ChanceToResummonOnDeath"
    )]
    [InlineData(nameof(SkillSliderLimits.WraithMaximum), "Slider_Wraiths_SummonMax")]
    [InlineData(nameof(SkillSliderLimits.WraithDelayed), "Slider_Wraiths_Delayed")]
    [InlineData(nameof(SkillSliderLimits.WraithCastSpeed), "Slider_Wraiths_CastSpeed")]
    [InlineData(
        nameof(SkillSliderLimits.MageFromPassives),
        "Slider_Mages_SummonQuantityFromPassive"
    )]
    [InlineData(nameof(SkillSliderLimits.MageFromItems), "Slider_Mages_SummonQuantityFromItems")]
    [InlineData(
        nameof(SkillSliderLimits.MageFromSkillTree),
        "Slider_Mages_SummonQuantityFromSkillTree"
    )]
    [InlineData(nameof(SkillSliderLimits.MagePerCast), "Slider_Mages_SummonPerCast")]
    [InlineData(
        nameof(SkillSliderLimits.MageProjectileChance),
        "Slider_Mages_ChanceForExtraPorjectiles"
    )]
    [InlineData(
        nameof(SkillSliderLimits.BoneGolemPerSkeletons),
        "Slider_BoneGolem_GolemPerSkeletons"
    )]
    [InlineData(
        nameof(SkillSliderLimits.BoneGolemResurrectChance),
        "Slider_BoneGolem_SelfResurectChance"
    )]
    [InlineData(nameof(SkillSliderLimits.BoneGolemFireAura), "Slider_BoneGolem_IncreaseFireAura")]
    [InlineData(nameof(SkillSliderLimits.BoneGolemArmorAura), "Slider_BoneGolem_IncreaseArmorAura")]
    [InlineData(
        nameof(SkillSliderLimits.BoneGolemMoveSpeedAura),
        "Slider_BoneGolem_IncreaseMoveSpeedAura"
    )]
    [InlineData(nameof(SkillSliderLimits.BoneGolemMoveSpeed), "Slider_BoneGolem_IncreaseMoveSpeed")]
    [InlineData(
        nameof(SkillSliderLimits.ZombieMinionDeathChance),
        "Slider_VolatileZombies_ChanceOnMinionDeath"
    )]
    [InlineData(
        nameof(SkillSliderLimits.ZombieInfernalShadeChance),
        "Slider_VolatileZombies_InfernalShadeChance"
    )]
    [InlineData(
        nameof(SkillSliderLimits.ZombieMarrowShardsChance),
        "Slider_VolatileZombies_MarrowShardsChance"
    )]
    [InlineData(nameof(SkillSliderLimits.DreadShadeDuration), "Slider_DreadShades_Duration")]
    [InlineData(nameof(SkillSliderLimits.DreadShadeMaximum), "Slider_DreadShades_Max")]
    [InlineData(nameof(SkillSliderLimits.DreadShadeDecay), "Slider_DreadShades_Decay")]
    [InlineData(nameof(SkillSliderLimits.DreadShadeRadius), "Slider_DreadShades_Radius")]
    public void Range_MatchesPrefab(string field, string slider)
    {
        IReadOnlyDictionary<string, SliderRange> prefab = PrefabSliders.Load(
            PrefabSliders.HudPrefabPath
        );

        Assert.True(prefab.ContainsKey(slider), $"Prefab slider missing: {slider}");
        SliderRange? actual = SliderLimitsReflection.Find(typeof(SkillSliderLimits), field);
        Assert.True(actual.HasValue, $"Limits field missing: {field}");
        Assert.Equal(prefab[slider], actual.Value);
    }
}
