namespace LastEpoch_Hud.Scripts.Core.ModUI.Settings;

/// <summary>The enabled flag and ordered bounds read from a saved Range entry, and whether they were swapped.</summary>
public readonly record struct SavedRange(bool Enabled, float Min, float Max, bool Swapped);
