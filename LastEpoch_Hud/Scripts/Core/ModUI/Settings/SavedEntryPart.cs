using System;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Scripts.Core.ModUI.Settings;

/// <summary>Reads one part of a saved setting entry, keeping the current value when absent.</summary>
public static class SavedEntryPart
{
    public static bool Bool(JObject entry, string part, bool current)
    {
        return entry[part] == null ? current : entry.Value<bool>(part);
    }

    public static float Float(JObject entry, string part, float current)
    {
        return entry[part] == null ? current : entry.Value<float>(part);
    }

    public static bool TryReadText(JToken token, out string value)
    {
        value = token.Type == JTokenType.String ? token.Value<string>() : null;
        return value != null;
    }

    public static SavedFloat ReadFloat(
        JObject entry,
        bool enabled,
        float value,
        float min,
        float max,
        bool hasLimits
    )
    {
        float raw = Float(entry, "Value", value);
        bool on = Bool(entry, "Enabled", enabled);
        if (float.IsNaN(raw))
        {
            return new SavedFloat(on, value, raw, true);
        }

        float clamped = hasLimits ? Math.Clamp(raw, min, max) : raw;
        return new SavedFloat(on, clamped, raw, clamped != raw);
    }

    public static SavedRange ReadRange(
        JObject entry,
        bool enabled,
        float min,
        float max,
        float minLimit,
        float maxLimit
    )
    {
        float lo = Math.Clamp(Float(entry, "Min", min), minLimit, maxLimit);
        float hi = Math.Clamp(Float(entry, "Max", max), minLimit, maxLimit);
        bool on = Bool(entry, "Enabled", enabled);
        return lo > hi ? new SavedRange(on, hi, lo, true) : new SavedRange(on, lo, hi, false);
    }
}
