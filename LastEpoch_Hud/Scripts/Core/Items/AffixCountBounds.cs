namespace LastEpoch_Hud.Scripts.Core.Items;

/// <summary>Whole affix-count range for a drop and whether it replaces the game's roll.</summary>
public readonly record struct AffixCountBounds(int Min, int Max, bool Overrides);
