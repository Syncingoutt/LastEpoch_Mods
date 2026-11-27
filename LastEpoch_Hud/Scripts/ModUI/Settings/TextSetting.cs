using System;

namespace LastEpoch_Hud.Scripts.ModUI.Settings;

/// <summary>Saved free-text value without a widget.</summary>
public sealed class TextSetting
{
    public TextSetting(string defaultValue = "")
    {
        Value = defaultValue ?? "";
    }

    public event Action<string> Changed;

    public string Value { get; internal set; }

    public void Set(string value)
    {
        string text = value ?? "";
        if (Value == text)
        {
            return;
        }

        Value = text;
        ModSettings.MarkDirty();
        Changed?.Invoke(text);
    }
}
