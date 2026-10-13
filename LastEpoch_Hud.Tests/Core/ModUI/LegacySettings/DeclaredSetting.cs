namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings;

/// <summary>A setting read from the built mod: its kind, declared defaults and limits.</summary>
internal readonly record struct DeclaredSetting(
    string Kind,
    bool BoolDefault,
    float FloatValue,
    bool FloatEnabled,
    float MinLimit,
    float MaxLimit,
    bool RangeEnabled,
    float RangeMin,
    float RangeMax,
    int DropdownDefault = 0,
    string KeybindDefault = null
);
