using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>Maps legacy Save.json text to ModSettings group sections.</summary>
public static class LegacySettingsImporter
{
    public static bool TryImport(
        string legacyJson,
        IReadOnlyList<LegacyFieldMapping> map,
        IReadOnlyCollection<string> groups,
        ILegacyKeyNames keys,
        out JObject sections,
        List<LegacyImportProblem> problems
    )
    {
        sections = new JObject();
        if (!TryParseRoot(legacyJson, problems, out JObject root))
        {
            return false;
        }
        foreach (LegacyFieldMapping mapping in map)
        {
            if (groups.Contains(mapping.Group))
            {
                ImportField(root, mapping, keys, sections, problems);
            }
        }
        SwapInvertedRanges(sections, map);
        return true;
    }

    private static bool TryParseRoot(
        string text,
        List<LegacyImportProblem> problems,
        out JObject root
    )
    {
        root = null;
        if (string.IsNullOrWhiteSpace(text))
        {
            AddWholeFileProblem(problems, LegacyImportProblemCode.EmptyText, "Save.json is empty.");
            return false;
        }
        if (!TryParseToken(text, out JToken token, out string error))
        {
            AddWholeFileProblem(problems, LegacyImportProblemCode.InvalidJson, error);
            return false;
        }
        root = token as JObject;
        if (root == null)
        {
            AddWholeFileProblem(
                problems,
                LegacyImportProblemCode.RootNotObject,
                "Save.json root is " + token.Type + "."
            );
        }
        return root != null;
    }

    private static bool TryParseToken(string text, out JToken token, out string error)
    {
        token = null;
        error = null;
        try
        {
            using JsonTextReader reader = new(new StringReader(text))
            {
                DateParseHandling = DateParseHandling.None,
            };
            token = JToken.ReadFrom(reader);
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.Comment)
                {
                    continue;
                }
                error = "Unexpected content after the root value.";
                return false;
            }
            return true;
        }
        catch (JsonReaderException exception)
        {
            error = exception.Message;
            return false;
        }
    }

    private static void AddWholeFileProblem(
        List<LegacyImportProblem> problems,
        LegacyImportProblemCode code,
        string message
    )
    {
        problems.Add(new LegacyImportProblem(code, string.Empty, message));
    }

    private static void ImportField(
        JObject root,
        LegacyFieldMapping mapping,
        ILegacyKeyNames keys,
        JObject sections,
        List<LegacyImportProblem> problems
    )
    {
        JToken token = LegacyJsonPath.Find(root, mapping.LegacyPath);
        if (token == null)
        {
            return;
        }
        if (!TryConvert(token, mapping, keys, out JToken value))
        {
            problems.Add(
                new LegacyImportProblem(
                    LegacyImportProblemCode.InvalidValue,
                    mapping.LegacyPath,
                    "Cannot read " + token.Type + " as " + mapping.Part + "."
                )
            );
            return;
        }
        if (value != null)
        {
            Emit(sections, mapping, value);
        }
    }

    private static bool TryConvert(
        JToken token,
        LegacyFieldMapping mapping,
        ILegacyKeyNames keys,
        out JToken value
    )
    {
        switch (mapping.Part)
        {
            case LegacySettingPart.Bool:
            case LegacySettingPart.FloatEnabled:
            case LegacySettingPart.RangeEnabled:
                return TryConvertBool(token, out value);
            case LegacySettingPart.Dropdown:
                return TryConvertDropdown(token, mapping, out value);
            case LegacySettingPart.Keybind:
                return TryConvertKeybind(token, keys, out value);
            case LegacySettingPart.Text:
                return TryConvertText(token, out value);
            default:
                return TryConvertNumber(token, mapping, out value);
        }
    }

    private static bool TryConvertBool(JToken token, out JToken value)
    {
        value = null;
        if (!LegacyValueReader.TryReadBool(token, out bool flag))
        {
            return false;
        }
        value = new JValue(flag);
        return true;
    }

    private static bool TryConvertNumber(JToken token, LegacyFieldMapping mapping, out JToken value)
    {
        value = null;
        if (!LegacyValueReader.TryReadFloat(token, out float number))
        {
            return false;
        }
        value = new JValue(Clamp(number, mapping));
        return true;
    }

    private static bool TryConvertDropdown(
        JToken token,
        LegacyFieldMapping mapping,
        out JToken value
    )
    {
        value = null;
        if (!LegacyValueReader.TryReadFloat(token, out float number))
        {
            return false;
        }
        double rounded = Math.Round(Clamp(number, mapping), MidpointRounding.ToEven);
        value = new JValue((int)Math.Clamp(rounded, int.MinValue, int.MaxValue));
        return true;
    }

    private static bool TryConvertKeybind(JToken token, ILegacyKeyNames keys, out JToken value)
    {
        value = null;
        if (!LegacyValueReader.TryReadKeyCode(token, keys, out int code))
        {
            return false;
        }
        if (code == 0)
        {
            return true;
        }
        if (!keys.TryGetName(code, out string name))
        {
            return false;
        }
        value = new JValue("kb:" + name);
        return true;
    }

    private static bool TryConvertText(JToken token, out JToken value)
    {
        value = null;
        if (!LegacyValueReader.TryReadText(token, out string text))
        {
            return false;
        }
        value = new JValue(text);
        return true;
    }

    private static float Clamp(float value, LegacyFieldMapping mapping)
    {
        if (!mapping.HasLimits)
        {
            return value;
        }
        return Math.Clamp(value, mapping.MinLimit, mapping.MaxLimit);
    }

    private static string SubKey(LegacySettingPart part)
    {
        switch (part)
        {
            case LegacySettingPart.FloatEnabled:
            case LegacySettingPart.RangeEnabled:
                return "Enabled";
            case LegacySettingPart.FloatValue:
                return "Value";
            case LegacySettingPart.RangeMin:
                return "Min";
            case LegacySettingPart.RangeMax:
                return "Max";
            default:
                return null;
        }
    }

    private static void Emit(JObject sections, LegacyFieldMapping mapping, JToken value)
    {
        JObject section = GetOrAddObject(sections, mapping.Group);
        string subKey = SubKey(mapping.Part);
        if (subKey == null)
        {
            section[mapping.Key] = value;
            return;
        }
        GetOrAddObject(section, mapping.Key)[subKey] = value;
    }

    private static JObject GetOrAddObject(JObject parent, string name)
    {
        if (parent[name] is JObject existing)
        {
            return existing;
        }
        JObject created = new();
        parent[name] = created;
        return created;
    }

    private static void SwapInvertedRanges(JObject sections, IReadOnlyList<LegacyFieldMapping> map)
    {
        foreach (LegacyFieldMapping mapping in map)
        {
            if (mapping.Part != LegacySettingPart.RangeMin)
            {
                continue;
            }
            if (sections[mapping.Group]?[mapping.Key] is JObject range)
            {
                SwapIfInverted(range);
            }
        }
    }

    private static void SwapIfInverted(JObject range)
    {
        if (range["Min"] == null || range["Max"] == null)
        {
            return;
        }
        float min = (float)range["Min"];
        float max = (float)range["Max"];
        if (min <= max)
        {
            return;
        }
        range["Min"] = max;
        range["Max"] = min;
    }
}
