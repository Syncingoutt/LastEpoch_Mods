namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>KeyCode numbers and names (Unity in the mod, a fake in tests).</summary>
public interface ILegacyKeyNames
{
    /// <summary>Canonical name of a defined code.</summary>
    bool TryGetName(int code, out string name);

    /// <summary>Code of a name, ignoring case.</summary>
    bool TryGetCode(string name, out int code);
}
