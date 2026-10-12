namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings;

/// <summary>A setting read from the built mod: its kind and, for Bool, its declared default.</summary>
internal readonly record struct DeclaredSetting(string Kind, bool BoolDefault);
