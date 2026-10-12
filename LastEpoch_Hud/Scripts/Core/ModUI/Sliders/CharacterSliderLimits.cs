namespace LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

/// <summary>Slider limits of the Utilities > Character and Buffs sliders, copied from the legacy prefab.</summary>
public static class CharacterSliderLimits
{
    // Source: Slider_Character_Factions_Favor
    public static readonly SliderRange FactionFavor = new(0f, 1000000f, true);

    // Source: Slider_Character_Factions_Rank
    public static readonly SliderRange FactionRank = new(0f, 12f, true);

    // Source: Slider_Character_Factions_Reputation
    public static readonly SliderRange FactionReputation = new(0f, 1000000f, true);

    // Source: Slider_Character_Buffs_MoveSpeed
    public static readonly SliderRange BuffMoveSpeed = new(0f, 20f, false);

    // Source: Slider_Character_Buffs_CastingSpeed
    public static readonly SliderRange BuffCastSpeed = new(0f, 10f, false);

    // Source: Slider_Character_Buffs_AttackSpeed
    public static readonly SliderRange BuffAttackSpeed = new(0f, 10f, false);

    // Source: Slider_Character_Buffs_Damage
    public static readonly SliderRange BuffDamage = new(0f, 255f, false);

    // Source: Slider_Character_Buffs_ManaRegen
    public static readonly SliderRange BuffManaRegen = new(0f, 10f, false);

    // Source: Slider_Character_Buffs_HealthRegen
    public static readonly SliderRange BuffHealthRegen = new(0f, 10f, false);

    // Source: Slider_Character_Buffs_CriticalChance
    public static readonly SliderRange BuffCriticalChance = new(0f, 0.95f, false);

    // Source: Slider_Character_Buffs_CriticalMultiplier
    public static readonly SliderRange BuffCriticalMultiplier = new(0f, 255f, false);

    // Source: Slider_Character_Buffs_Strenght
    public static readonly SliderRange BuffStrength = new(0f, 255f, false);

    // Source: Slider_Character_Buffs_Intelligence
    public static readonly SliderRange BuffIntelligence = new(0f, 255f, false);

    // Source: Slider_Character_Buffs_Dexterity
    public static readonly SliderRange BuffDexterity = new(0f, 255f, false);

    // Source: Slider_Character_Buffs_Vitality
    public static readonly SliderRange BuffVitality = new(0f, 255f, false);

    // Source: Slider_Character_Buffs_Attunement
    public static readonly SliderRange BuffAttunement = new(0f, 255f, false);

    // Source: no prefab slider; former UtilitiesBuffsPage inline max, commit 511ad5e0
    public static readonly SliderRange BuffAreaOfEffect = new(0f, 100f, false);
}
