namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>One import problem. Message is for the log only.</summary>
public readonly record struct LegacyImportProblem(
    LegacyImportProblemCode Code,
    string Path,
    string Message
);
