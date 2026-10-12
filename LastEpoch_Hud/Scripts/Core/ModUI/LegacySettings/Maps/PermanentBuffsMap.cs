using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Character.PermanentBuffs fields mapped to the PermanentBuffs group.</summary>
public static class PermanentBuffsMap
{
    public const string Group = "PermanentBuffs";

    private const string Prefix = "Character.PermanentBuffs.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } = Build();

    private static LegacyFieldMapping[] Build()
    {
        List<LegacyFieldMapping> all = new()
        {
            new(Prefix + "Enable_Mod", Group, "EnableMod", LegacySettingPart.Bool),
        };
        AddBuff(all, "MoveSpeed", "MoveSpeed", CharacterSliderLimits.BuffMoveSpeed);
        AddBuff(all, "CastSpeed", "CastSpeed", CharacterSliderLimits.BuffCastSpeed);
        AddBuff(all, "AttackSpeed", "AttackSpeed", CharacterSliderLimits.BuffAttackSpeed);
        AddBuff(all, "Damage", "Damage", CharacterSliderLimits.BuffDamage);
        AddBuff(all, "ManaRegen", "ManaRegen", CharacterSliderLimits.BuffManaRegen);
        AddBuff(all, "HealthRegen", "HealthRegen", CharacterSliderLimits.BuffHealthRegen);
        AddBuff(all, "CriticalChance", "CriticalChance", CharacterSliderLimits.BuffCriticalChance);
        AddBuff(
            all,
            "CriticalMultiplier",
            "CriticalMultiplier",
            CharacterSliderLimits.BuffCriticalMultiplier
        );
        AddBuff(all, "Str", "Strength", CharacterSliderLimits.BuffStrength);
        AddBuff(all, "Int", "Intelligence", CharacterSliderLimits.BuffIntelligence);
        AddBuff(all, "Dex", "Dexterity", CharacterSliderLimits.BuffDexterity);
        AddBuff(all, "Vit", "Vitality", CharacterSliderLimits.BuffVitality);
        AddBuff(all, "Att", "Attunement", CharacterSliderLimits.BuffAttunement);
        AddBuff(all, "AoE", "AreaOfEffect", CharacterSliderLimits.BuffAreaOfEffect);
        return all.ToArray();
    }

    private static void AddBuff(
        List<LegacyFieldMapping> all,
        string legacyName,
        string key,
        SliderRange limits
    )
    {
        all.Add(
            new(
                Prefix + "Enable_" + legacyName + "_Buff",
                Group,
                key,
                LegacySettingPart.FloatEnabled
            )
        );
        all.Add(
            new(
                Prefix + legacyName + "_Buff_Value",
                Group,
                key,
                LegacySettingPart.FloatValue,
                limits.Min,
                limits.Max
            )
        );
    }
}
