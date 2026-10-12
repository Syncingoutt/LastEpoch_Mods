namespace LastEpoch_Hud.Scripts.Core.ModUI.Settings;

/// <summary>How the settings file looked at load.</summary>
public enum SettingsFileState
{
    Missing,
    Loaded,
    Corrupt,
    Unreadable,
}
