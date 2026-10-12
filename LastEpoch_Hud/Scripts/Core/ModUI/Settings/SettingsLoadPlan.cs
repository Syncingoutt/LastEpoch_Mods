namespace LastEpoch_Hud.Scripts.Core.ModUI.Settings;

/// <summary>What Load must do after reading the settings file.</summary>
public readonly record struct SettingsLoadPlan(
    bool ResetCapLevelToZone,
    bool BackupBeforeWrite,
    bool WriteNow
);
