namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>One legacy Save.json field mapped to one part of a new setting. RequiresTruePath: legacy bool path that must be true to import.</summary>
public readonly record struct LegacyFieldMapping(
    string LegacyPath,
    string Group,
    string Key,
    LegacySettingPart Part,
    float MinLimit = 0f,
    float MaxLimit = 0f,
    string RequiresTruePath = null
)
{
    public bool HasLimits => MaxLimit > MinLimit;
}
