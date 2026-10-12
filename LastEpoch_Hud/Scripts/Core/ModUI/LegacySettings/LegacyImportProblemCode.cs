namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>Why a legacy file or field was not imported.</summary>
public enum LegacyImportProblemCode
{
    EmptyText,
    InvalidJson,
    RootNotObject,
    InvalidValue,
}
