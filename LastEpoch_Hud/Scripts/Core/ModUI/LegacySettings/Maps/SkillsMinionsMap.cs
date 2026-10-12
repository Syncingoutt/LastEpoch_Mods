using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Skills.Minions fields mapped to the SkillsMinions group.</summary>
public static class SkillsMinionsMap
{
    public const string Group = "SkillsMinions";

    private const string Prefix = "Skills.Minions.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } = Build();

    private static LegacyFieldMapping[] Build()
    {
        List<LegacyFieldMapping> all = new();
        AddSkeletons(all);
        AddWraiths(all);
        AddMages(all);
        AddBoneGolems(all);
        AddVolatileZombies(all);
        AddDreadShades(all);
        return all.ToArray();
    }

    private static void AddSkeletons(List<LegacyFieldMapping> all)
    {
        AddBool(all, "Skeletons.Enable_forceArcher", "SkeletonForceArcher");
        AddBool(all, "Skeletons.Enable_forceBrawler", "SkeletonForceBrawler");
        AddBool(all, "Skeletons.Enable_forceRogue", "SkeletonForceRogue");
        AddBool(all, "Skeletons.Enable_forceWarrior", "SkeletonForceWarrior");
        AddFloat(
            all,
            "Skeletons.Enable_additionalSkeletonsFromPassives",
            "Skeletons.additionalSkeletonsFromPassives",
            "SkeletonFromPassives",
            SkillSliderLimits.SkeletonFromPassives
        );
        AddFloat(
            all,
            "Skeletons.Enable_additionalSkeletonsFromSkillTree",
            "Skeletons.additionalSkeletonsFromSkillTree",
            "SkeletonFromSkillTree",
            SkillSliderLimits.SkeletonFromSkillTree
        );
        AddFloat(
            all,
            "Skeletons.Enable_additionalSkeletonsPerCast",
            "Skeletons.additionalSkeletonsPerCast",
            "SkeletonPerCast",
            SkillSliderLimits.SkeletonPerCast
        );
        AddFloat(
            all,
            "Skeletons.Enable_chanceToResummonOnDeath",
            "Skeletons.chanceToResummonOnDeath",
            "SkeletonResummonChance",
            SkillSliderLimits.SkeletonResummonChance
        );
    }

    private static void AddWraiths(List<LegacyFieldMapping> all)
    {
        AddBool(all, "Wraiths.Enable_limitedTo2Wraiths", "WraithNoLimit");
        AddBool(all, "Wraiths.Enable_wraithsDoNotDecay", "WraithNoDecay");
        AddFloat(
            all,
            "Wraiths.Enable_additionalMaxWraiths",
            "Wraiths.additionalMaxWraiths",
            "WraithMaximum",
            SkillSliderLimits.WraithMaximum
        );
        AddFloat(
            all,
            "Wraiths.Enable_delayedWraiths",
            "Wraiths.delayedWraiths",
            "WraithDelayed",
            SkillSliderLimits.WraithDelayed
        );
        AddFloat(
            all,
            "Wraiths.Enable_increasedCastSpeed",
            "Wraiths.increasedCastSpeed",
            "WraithCastSpeed",
            SkillSliderLimits.WraithCastSpeed
        );
    }

    private static void AddMages(List<LegacyFieldMapping> all)
    {
        AddBool(all, "Mages.Enable_singleSummon", "MageNoSingleSummon");
        AddBool(all, "Mages.Enable_forceCryomancer", "MageForceCryomancer");
        AddBool(all, "Mages.Enable_forceDeathKnight", "MageForceDeathKnight");
        AddBool(all, "Mages.Enable_forcePyromancer", "MageForcePyromancer");
        AddBool(all, "Mages.Enable_doubleProjectiles", "MageDoubleProjectiles");
        AddFloat(
            all,
            "Mages.Enable_additionalSkeletonsFromPassives",
            "Mages.additionalSkeletonsFromPassives",
            "MageFromPassives",
            SkillSliderLimits.MageFromPassives
        );
        AddFloat(
            all,
            "Mages.Enable_additionalSkeletonsFromItems",
            "Mages.additionalSkeletonsFromItems",
            "MageFromItems",
            SkillSliderLimits.MageFromItems
        );
        AddFloat(
            all,
            "Mages.Enable_additionalSkeletonsFromSkillTree",
            "Mages.additionalSkeletonsFromSkillTree",
            "MageFromSkillTree",
            SkillSliderLimits.MageFromSkillTree
        );
        AddFloat(
            all,
            "Mages.Enable_additionalSkeletonsPerCast",
            "Mages.additionalSkeletonsPerCast",
            "MagePerCast",
            SkillSliderLimits.MagePerCast
        );
        AddFloat(
            all,
            "Mages.Enable_chanceForTwoExtraProjectiles",
            "Mages.chanceForTwoExtraProjectiles",
            "MageProjectileChance",
            SkillSliderLimits.MageProjectileChance
        );
    }

    private static void AddBoneGolems(List<LegacyFieldMapping> all)
    {
        AddBool(all, "BoneGolems.Enable_twins", "BoneGolemTwins");
        AddBool(all, "BoneGolems.Enable_hasSlamAttack", "BoneGolemSlamAttack");
        AddFloat(
            all,
            "BoneGolems.Enable_addedGolemsPer4Skeletons",
            "BoneGolems.addedGolemsPer4Skeletons",
            "BoneGolemPerSkeletons",
            SkillSliderLimits.BoneGolemPerSkeletons
        );
        AddFloat(
            all,
            "BoneGolems.Enable_selfResurrectChance",
            "BoneGolems.selfResurrectChance",
            "BoneGolemResurrectChance",
            SkillSliderLimits.BoneGolemResurrectChance
        );
        AddFloat(
            all,
            "BoneGolems.Enable_increasedFireAuraArea",
            "BoneGolems.increasedFireAuraArea",
            "BoneGolemFireAura",
            SkillSliderLimits.BoneGolemFireAura
        );
        AddFloat(
            all,
            "BoneGolems.Enable_undeadArmorAura",
            "BoneGolems.undeadArmorAura",
            "BoneGolemArmorAura",
            SkillSliderLimits.BoneGolemArmorAura
        );
        AddFloat(
            all,
            "BoneGolems.Enable_undeadMovespeedAura",
            "BoneGolems.undeadMovespeedAura",
            "BoneGolemMoveSpeedAura",
            SkillSliderLimits.BoneGolemMoveSpeedAura
        );
        AddFloat(
            all,
            "BoneGolems.Enable_increasedMoveSpeed",
            "BoneGolems.increasedMoveSpeed",
            "BoneGolemMoveSpeed",
            SkillSliderLimits.BoneGolemMoveSpeed
        );
    }

    private static void AddVolatileZombies(List<LegacyFieldMapping> all)
    {
        AddFloat(
            all,
            "VolatileZombies.Enable_chanceToCastFromMinionDeath",
            "VolatileZombies.chanceToCastFromMinionDeath",
            "ZombieMinionDeathChance",
            SkillSliderLimits.ZombieMinionDeathChance
        );
        AddFloat(
            all,
            "VolatileZombies.Enable_chanceToCastInfernalShadeOnDeath",
            "VolatileZombies.chanceToCastInfernalShadeOnDeath",
            "ZombieInfernalShadeChance",
            SkillSliderLimits.ZombieInfernalShadeChance
        );
        AddFloat(
            all,
            "VolatileZombies.Enable_chanceToCastMarrowShardsOnDeath",
            "VolatileZombies.chanceToCastMarrowShardsOnDeath",
            "ZombieMarrowShardsChance",
            SkillSliderLimits.ZombieMarrowShardsChance
        );
    }

    private static void AddDreadShades(List<LegacyFieldMapping> all)
    {
        AddBool(all, "DreadShades.Enable_DisableLimit", "DreadShadeNoLimit");
        AddBool(all, "DreadShades.Enable_DisableHealthDrain", "DreadShadeNoHealthDrain");
        AddFloat(
            all,
            "DreadShades.Enable_Duration",
            "DreadShades.Duration",
            "DreadShadeDuration",
            SkillSliderLimits.DreadShadeDuration
        );
        AddFloat(
            all,
            "DreadShades.Enable_Max",
            "DreadShades.max",
            "DreadShadeMaximum",
            SkillSliderLimits.DreadShadeMaximum
        );
        AddFloat(
            all,
            "DreadShades.Enable_ReduceDecay",
            "DreadShades.decay",
            "DreadShadeDecay",
            SkillSliderLimits.DreadShadeDecay
        );
        AddFloat(
            all,
            "DreadShades.Enable_Radius",
            "DreadShades.radius",
            "DreadShadeRadius",
            SkillSliderLimits.DreadShadeRadius
        );
    }

    private static void AddBool(List<LegacyFieldMapping> all, string legacyPath, string key)
    {
        all.Add(new LegacyFieldMapping(Prefix + legacyPath, Group, key, LegacySettingPart.Bool));
    }

    private static void AddFloat(
        List<LegacyFieldMapping> all,
        string enabledPath,
        string valuePath,
        string key,
        SliderRange limits
    )
    {
        all.Add(
            new LegacyFieldMapping(Prefix + enabledPath, Group, key, LegacySettingPart.FloatEnabled)
        );
        all.Add(
            new LegacyFieldMapping(
                Prefix + valuePath,
                Group,
                key,
                LegacySettingPart.FloatValue,
                limits.Min,
                limits.Max
            )
        );
    }
}
