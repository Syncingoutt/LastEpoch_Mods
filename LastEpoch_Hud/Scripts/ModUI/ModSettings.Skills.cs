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
#pragma warning restore CA1034
}
