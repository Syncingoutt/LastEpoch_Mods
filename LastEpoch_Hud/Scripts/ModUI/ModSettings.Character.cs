using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Scripts.Mods.Character;
using LastEpoch_Hud.Scripts.ModUI.Settings;

namespace LastEpoch_Hud.Scripts.ModUI;

public static partial class ModSettings
{
#pragma warning disable CA1034 // nested like every settings section
    /// <summary>Utilities > Buffs values and master flag.</summary>
    public static class PermanentBuffs
    {
        public static readonly SettingsGroup Group = new(PermanentBuffsMap.Group);

        public static readonly BoolSetting EnableMod = Group.Bool("EnableMod", defaultValue: true);

        public static readonly FloatSetting MoveSpeed = Group.Float(
            "MoveSpeed",
            defaultValue: 1f,
            min: CharacterSliderLimits.BuffMoveSpeed.Min,
            max: CharacterSliderLimits.BuffMoveSpeed.Max
        );

        public static readonly FloatSetting CastSpeed = Group.Float(
            "CastSpeed",
            defaultValue: 1f,
            min: CharacterSliderLimits.BuffCastSpeed.Min,
            max: CharacterSliderLimits.BuffCastSpeed.Max
        );

        public static readonly FloatSetting AttackSpeed = Group.Float(
            "AttackSpeed",
            defaultValue: 1f,
            min: CharacterSliderLimits.BuffAttackSpeed.Min,
            max: CharacterSliderLimits.BuffAttackSpeed.Max
        );

        public static readonly FloatSetting Damage = Group.Float(
            "Damage",
            defaultValue: 1f,
            min: CharacterSliderLimits.BuffDamage.Min,
            max: CharacterSliderLimits.BuffDamage.Max
        );

        public static readonly FloatSetting ManaRegen = Group.Float(
            "ManaRegen",
            defaultValue: 1f,
            min: CharacterSliderLimits.BuffManaRegen.Min,
            max: CharacterSliderLimits.BuffManaRegen.Max
        );

        public static readonly FloatSetting HealthRegen = Group.Float(
            "HealthRegen",
            defaultValue: 1f,
            min: CharacterSliderLimits.BuffHealthRegen.Min,
            max: CharacterSliderLimits.BuffHealthRegen.Max
        );

        public static readonly FloatSetting CriticalChance = Group.Float(
            "CriticalChance",
            min: CharacterSliderLimits.BuffCriticalChance.Min,
            max: CharacterSliderLimits.BuffCriticalChance.Max
        );

        public static readonly FloatSetting CriticalMultiplier = Group.Float(
            "CriticalMultiplier",
            min: CharacterSliderLimits.BuffCriticalMultiplier.Min,
            max: CharacterSliderLimits.BuffCriticalMultiplier.Max
        );

        public static readonly FloatSetting Strength = Group.Float(
            "Strength",
            min: CharacterSliderLimits.BuffStrength.Min,
            max: CharacterSliderLimits.BuffStrength.Max
        );

        public static readonly FloatSetting Intelligence = Group.Float(
            "Intelligence",
            min: CharacterSliderLimits.BuffIntelligence.Min,
            max: CharacterSliderLimits.BuffIntelligence.Max
        );

        public static readonly FloatSetting Dexterity = Group.Float(
            "Dexterity",
            min: CharacterSliderLimits.BuffDexterity.Min,
            max: CharacterSliderLimits.BuffDexterity.Max
        );

        public static readonly FloatSetting Vitality = Group.Float(
            "Vitality",
            min: CharacterSliderLimits.BuffVitality.Min,
            max: CharacterSliderLimits.BuffVitality.Max
        );

        public static readonly FloatSetting Attunement = Group.Float(
            "Attunement",
            min: CharacterSliderLimits.BuffAttunement.Min,
            max: CharacterSliderLimits.BuffAttunement.Max
        );

        public static readonly FloatSetting AreaOfEffect = Group.Float(
            "AreaOfEffect",
            defaultValue: 1f,
            min: CharacterSliderLimits.BuffAreaOfEffect.Min,
            max: CharacterSliderLimits.BuffAreaOfEffect.Max
        );

        public static IReadOnlyList<FloatSetting> Buffs { get; } =
            new[]
            {
                MoveSpeed,
                Damage,
                AreaOfEffect,
                AttackSpeed,
                CastSpeed,
                CriticalChance,
                CriticalMultiplier,
                HealthRegen,
                ManaRegen,
                Strength,
                Intelligence,
                Dexterity,
                Vitality,
                Attunement,
            };
    }

    /// <summary>Utilities > Character cheat toggles; some have no UI.</summary>
    public static class CharacterCheats
    {
        public static readonly SettingsGroup Group = new(CharacterCheatsMap.Group);

        public static readonly BoolSetting GodMode = Group.Bool("GodMode");
        public static readonly BoolSetting LowLife = Group.Bool("LowLife");
        public static readonly BoolSetting UnlockAllIdolsSlots = Group.Bool("UnlockAllIdolsSlots");
        public static readonly BoolSetting WaypointsUnlock = Group.Bool("WaypointsUnlock");
        public static readonly BoolSetting TwoHandedWithShield = Group.Bool("TwoHandedWithShield");

        public static readonly FloatSetting AutoPot = Group.Float(
            "AutoPot",
            min: CharacterSliderLimits.AutoPotion.Min,
            max: CharacterSliderLimits.AutoPotion.Max
        );

        static CharacterCheats()
        {
            UnlockAllIdolsSlots.Changed += OnUnlockAllIdolsSlotsChanged;
        }

        private static void OnUnlockAllIdolsSlotsChanged(bool _)
        {
            Character_UnlockAllIdols.Update();
        }
    }
#pragma warning restore CA1034
}
