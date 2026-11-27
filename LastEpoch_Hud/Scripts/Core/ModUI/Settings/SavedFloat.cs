namespace LastEpoch_Hud.Scripts.Core.ModUI.Settings;

/// <summary>The enabled flag, clamped value and unclamped raw value read from a saved Float entry.</summary>
public readonly record struct SavedFloat(bool Enabled, float Value, float Raw, bool Clamped);
