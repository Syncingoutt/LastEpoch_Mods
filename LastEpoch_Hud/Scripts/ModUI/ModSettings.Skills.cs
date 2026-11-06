using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Scripts.ModUI.Settings;

namespace LastEpoch_Hud.Scripts.ModUI;

public static partial class ModSettings
{
#pragma warning disable CA1034 // nested like every settings section
    /// <summary>Skills cheats on Utilities > Character and Skills > QOL.</summary>
    public static class SkillsGeneral
    {
        public static readonly SettingsGroup Group = new(SkillsGeneralMap.Group);

        public static readonly BoolSetting RemoveManaCost = Group.Bool("RemoveManaCost");
        public static readonly BoolSetting RemoveChannelCost = Group.Bool("RemoveChannelCost");
        public static readonly BoolSetting ManaRegenWhileChanneling = Group.Bool(
            "ManaRegenWhileChanneling"
        );
        public static readonly BoolSetting ContinueWhenOutOfMana = Group.Bool(
            "ContinueWhenOutOfMana"
        );
        public static readonly BoolSetting RemoveCooldown = Group.Bool("RemoveCooldown");
        public static readonly BoolSetting UnlockAllSkills = Group.Bool("UnlockAllSkills");
        public static readonly BoolSetting RemoveNodeRequirements = Group.Bool(
            "RemoveNodeRequirements"
        );

        public static readonly FloatSetting SpecializationSlots = Group.Float(
            "SpecializationSlots",
            min: SkillSliderLimits.SpecializationSlots.Min,
            max: SkillSliderLimits.SpecializationSlots.Max
        );

        public static readonly FloatSetting SkillLevel = Group.Float(
            "SkillLevel",
            min: SkillSliderLimits.SkillLevel.Min,
            max: SkillSliderLimits.SkillLevel.Max
        );

        public static readonly FloatSetting SkillLevelMultiplier = Group.Float(
            "SkillLevelMultiplier",
            defaultValue: 2f,
            min: SkillSliderLimits.PointMultiplier.Min,
            max: SkillSliderLimits.PointMultiplier.Max
        );

        public static readonly FloatSetting PassivePoints = Group.Float(
            "PassivePoints",
            min: SkillSliderLimits.PassivePoints.Min,
            max: SkillSliderLimits.PassivePoints.Max
        );

        public static readonly FloatSetting PassivePointMultiplier = Group.Float(
            "PassivePointMultiplier",
            defaultValue: 2f,
            min: SkillSliderLimits.PointMultiplier.Min,
            max: SkillSliderLimits.PointMultiplier.Max
        );
    }

    /// <summary>Movement-skill toggles on Skills > QOL.</summary>
    public static class SkillsMovement
    {
        public static readonly SettingsGroup Group = new(SkillsMovementMap.Group);

        public static readonly BoolSetting NoTarget = Group.Bool("NoTarget");
        public static readonly BoolSetting ImmuneDuringMovement = Group.Bool(
            "ImmuneDuringMovement"
        );
        public static readonly BoolSetting IgnoreSimplePath = Group.Bool("IgnoreSimplePath");
    }

    /// <summary>Companion cheats on Skills > Companions.</summary>
    public static class SkillsCompanions
    {
        public static readonly SettingsGroup Group = new(SkillsCompanionsMap.Group);

        public static readonly FloatSetting MaximumCompanions = Group.Float(
            "MaximumCompanions",
            defaultValue: 255f,
            min: SkillSliderLimits.CompanionsMaximum.Min,
            max: SkillSliderLimits.CompanionsMaximum.Max
        );
        public static readonly BoolSetting WolfSummonMaximum = Group.Bool("WolfSummonMaximum");
        public static readonly FloatSetting WolfSummonLimit = Group.Float(
            "WolfSummonLimit",
            defaultValue: 255f,
            min: SkillSliderLimits.WolfSummonLimit.Min,
            max: SkillSliderLimits.WolfSummonLimit.Max
        );
        public static readonly BoolSetting WolfStunImmunity = Group.Bool("WolfStunImmunity");
        public static readonly FloatSetting ScorpionBabyQuantity = Group.Float(
            "ScorpionBabyQuantity",
            defaultValue: 255f,
            min: SkillSliderLimits.ScorpionBabyQuantity.Min,
            max: SkillSliderLimits.ScorpionBabyQuantity.Max
        );
    }

    /// <summary>Minion cheats on Skills > Minions.</summary>
    public static class SkillsMinions
    {
        public static readonly SettingsGroup Group = new(SkillsMinionsMap.Group);

        public static readonly BoolSetting SkeletonForceArcher = Group.Bool("SkeletonForceArcher");
        public static readonly BoolSetting SkeletonForceBrawler = Group.Bool(
            "SkeletonForceBrawler"
        );
        public static readonly BoolSetting SkeletonForceRogue = Group.Bool("SkeletonForceRogue");
        public static readonly BoolSetting SkeletonForceWarrior = Group.Bool(
            "SkeletonForceWarrior"
        );
        public static readonly FloatSetting SkeletonFromPassives = Group.Float(
            "SkeletonFromPassives",
            min: SkillSliderLimits.SkeletonFromPassives.Min,
            max: SkillSliderLimits.SkeletonFromPassives.Max
        );
        public static readonly FloatSetting SkeletonFromSkillTree = Group.Float(
            "SkeletonFromSkillTree",
            min: SkillSliderLimits.SkeletonFromSkillTree.Min,
            max: SkillSliderLimits.SkeletonFromSkillTree.Max
        );
        public static readonly FloatSetting SkeletonPerCast = Group.Float(
            "SkeletonPerCast",
            min: SkillSliderLimits.SkeletonPerCast.Min,
            max: SkillSliderLimits.SkeletonPerCast.Max
        );
        public static readonly FloatSetting SkeletonResummonChance = Group.Float(
            "SkeletonResummonChance",
            min: SkillSliderLimits.SkeletonResummonChance.Min,
            max: SkillSliderLimits.SkeletonResummonChance.Max
        );

        public static readonly BoolSetting WraithNoLimit = Group.Bool("WraithNoLimit");
        public static readonly BoolSetting WraithNoDecay = Group.Bool("WraithNoDecay");
        public static readonly FloatSetting WraithMaximum = Group.Float(
            "WraithMaximum",
            min: SkillSliderLimits.WraithMaximum.Min,
            max: SkillSliderLimits.WraithMaximum.Max
        );
        public static readonly FloatSetting WraithDelayed = Group.Float(
            "WraithDelayed",
            min: SkillSliderLimits.WraithDelayed.Min,
            max: SkillSliderLimits.WraithDelayed.Max
        );
        public static readonly FloatSetting WraithCastSpeed = Group.Float(
            "WraithCastSpeed",
            min: SkillSliderLimits.WraithCastSpeed.Min,
            max: SkillSliderLimits.WraithCastSpeed.Max
        );

        public static readonly BoolSetting MageNoSingleSummon = Group.Bool("MageNoSingleSummon");
        public static readonly BoolSetting MageForceCryomancer = Group.Bool("MageForceCryomancer");
        public static readonly BoolSetting MageForceDeathKnight = Group.Bool(
            "MageForceDeathKnight"
        );
        public static readonly BoolSetting MageForcePyromancer = Group.Bool("MageForcePyromancer");
        public static readonly BoolSetting MageDoubleProjectiles = Group.Bool(
            "MageDoubleProjectiles"
        );
        public static readonly FloatSetting MageFromPassives = Group.Float(
            "MageFromPassives",
            min: SkillSliderLimits.MageFromPassives.Min,
            max: SkillSliderLimits.MageFromPassives.Max
        );
        public static readonly FloatSetting MageFromItems = Group.Float(
            "MageFromItems",
            min: SkillSliderLimits.MageFromItems.Min,
            max: SkillSliderLimits.MageFromItems.Max
        );
        public static readonly FloatSetting MageFromSkillTree = Group.Float(
            "MageFromSkillTree",
            min: SkillSliderLimits.MageFromSkillTree.Min,
            max: SkillSliderLimits.MageFromSkillTree.Max
        );
        public static readonly FloatSetting MagePerCast = Group.Float(
            "MagePerCast",
            min: SkillSliderLimits.MagePerCast.Min,
            max: SkillSliderLimits.MagePerCast.Max
        );
        public static readonly FloatSetting MageProjectileChance = Group.Float(
            "MageProjectileChance",
            min: SkillSliderLimits.MageProjectileChance.Min,
            max: SkillSliderLimits.MageProjectileChance.Max
        );

        public static readonly BoolSetting BoneGolemTwins = Group.Bool("BoneGolemTwins");
        public static readonly BoolSetting BoneGolemSlamAttack = Group.Bool("BoneGolemSlamAttack");
        public static readonly FloatSetting BoneGolemPerSkeletons = Group.Float(
            "BoneGolemPerSkeletons",
            min: SkillSliderLimits.BoneGolemPerSkeletons.Min,
            max: SkillSliderLimits.BoneGolemPerSkeletons.Max
        );
        public static readonly FloatSetting BoneGolemResurrectChance = Group.Float(
            "BoneGolemResurrectChance",
            min: SkillSliderLimits.BoneGolemResurrectChance.Min,
            max: SkillSliderLimits.BoneGolemResurrectChance.Max
        );
        public static readonly FloatSetting BoneGolemFireAura = Group.Float(
            "BoneGolemFireAura",
            min: SkillSliderLimits.BoneGolemFireAura.Min,
            max: SkillSliderLimits.BoneGolemFireAura.Max
        );
        public static readonly FloatSetting BoneGolemArmorAura = Group.Float(
            "BoneGolemArmorAura",
            min: SkillSliderLimits.BoneGolemArmorAura.Min,
            max: SkillSliderLimits.BoneGolemArmorAura.Max
        );
        public static readonly FloatSetting BoneGolemMoveSpeedAura = Group.Float(
            "BoneGolemMoveSpeedAura",
            min: SkillSliderLimits.BoneGolemMoveSpeedAura.Min,
            max: SkillSliderLimits.BoneGolemMoveSpeedAura.Max
        );
        public static readonly FloatSetting BoneGolemMoveSpeed = Group.Float(
            "BoneGolemMoveSpeed",
            min: SkillSliderLimits.BoneGolemMoveSpeed.Min,
            max: SkillSliderLimits.BoneGolemMoveSpeed.Max
        );

        public static readonly FloatSetting ZombieMinionDeathChance = Group.Float(
            "ZombieMinionDeathChance",
            min: SkillSliderLimits.ZombieMinionDeathChance.Min,
            max: SkillSliderLimits.ZombieMinionDeathChance.Max
        );
        public static readonly FloatSetting ZombieInfernalShadeChance = Group.Float(
            "ZombieInfernalShadeChance",
            min: SkillSliderLimits.ZombieInfernalShadeChance.Min,
            max: SkillSliderLimits.ZombieInfernalShadeChance.Max
        );
        public static readonly FloatSetting ZombieMarrowShardsChance = Group.Float(
            "ZombieMarrowShardsChance",
            min: SkillSliderLimits.ZombieMarrowShardsChance.Min,
            max: SkillSliderLimits.ZombieMarrowShardsChance.Max
        );

        public static readonly BoolSetting DreadShadeNoLimit = Group.Bool("DreadShadeNoLimit");
        public static readonly BoolSetting DreadShadeNoHealthDrain = Group.Bool(
            "DreadShadeNoHealthDrain"
        );
        public static readonly FloatSetting DreadShadeDuration = Group.Float(
            "DreadShadeDuration",
            min: SkillSliderLimits.DreadShadeDuration.Min,
            max: SkillSliderLimits.DreadShadeDuration.Max
        );
        public static readonly FloatSetting DreadShadeMaximum = Group.Float(
            "DreadShadeMaximum",
            min: SkillSliderLimits.DreadShadeMaximum.Min,
            max: SkillSliderLimits.DreadShadeMaximum.Max
        );
        public static readonly FloatSetting DreadShadeDecay = Group.Float(
            "DreadShadeDecay",
            min: SkillSliderLimits.DreadShadeDecay.Min,
            max: SkillSliderLimits.DreadShadeDecay.Max
        );
        public static readonly FloatSetting DreadShadeRadius = Group.Float(
            "DreadShadeRadius",
            min: SkillSliderLimits.DreadShadeRadius.Min,
            max: SkillSliderLimits.DreadShadeRadius.Max
        );
    }

    /// <summary>Summon toggles on Skills > Summon.</summary>
    public static class Summon
    {
        public static readonly SettingsGroup Group = new(SummonMap.Group);

        public static readonly BoolSetting GodMode = Group.Bool("GodMode");
        public static readonly BoolSetting Forever = Group.Bool("Forever");
        public static readonly BoolSetting DontCollide = Group.Bool("DontCollide");
    }
#pragma warning restore CA1034
}
