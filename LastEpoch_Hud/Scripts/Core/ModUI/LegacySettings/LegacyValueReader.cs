using System.Globalization;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>Converts legacy tokens the way Newtonsoft filled the legacy struct.</summary>
internal static class LegacyValueReader
{
    public static bool TryReadBool(JToken token, out bool value)
    {
        value = false;
        if (token is not JValue jValue)
        {
            return false;
        }
        switch (jValue.Value)
        {
            case bool flag:
                value = flag;
                return true;
            case long number:
                value = number != 0;
                return true;
            case string text:
                return bool.TryParse(text.Trim(), out value);
            default:
                return false;
        }
    }

    public static bool TryReadFloat(JToken token, out float value)
    {
        value = 0f;
        if (!TryReadDouble(token, out double number))
        {
            return false;
        }
        value = (float)number;
        return float.IsFinite(value);
    }

    public static bool TryReadKeyCode(JToken token, ILegacyKeyNames keys, out int code)
    {
        code = 0;
        if (token is not JValue jValue)
        {
            return false;
        }
        if (jValue.Value is long number)
        {
            code = (int)number;
            return number >= int.MinValue && number <= int.MaxValue;
        }
        if (jValue.Value is not string text)
        {
            return false;
        }
        text = text.Trim();
        if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out code))
        {
            return true;
        }
        return keys.TryGetCode(text, out code);
    }

    public static bool TryReadText(JToken token, out string value)
    {
        value = null;
        if (token is not JValue { Value: string text })
        {
            return false;
        }
        value = text;
        return true;
    }

    private static bool TryReadDouble(JToken token, out double number)
    {
        number = 0;
        if (token is not JValue jValue)
        {
            return false;
        }
        switch (jValue.Value)
        {
            case long integer:
                number = integer;
                return true;
            case double real:
                number = real;
                return true;
            case string text:
                return double.TryParse(
                    text,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out number
                );
            default:
                return false;
        }
    }
}
