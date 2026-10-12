namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>What one legacy import attempt did.</summary>
public enum LegacyImportOutcome
{
    NoLegacyFile,
    NothingMissing,
    Imported,
    FileFailed,
    FieldsFailed,
}
