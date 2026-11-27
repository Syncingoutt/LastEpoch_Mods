using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Core.ModUI.Sliders;
using LastEpoch_Hud.Tests.Support;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings;

public sealed class LegacySettingsMapTargetTests
{
    private const string ModSettingsPrefix = "LastEpoch_Hud.Scripts.ModUI.ModSettings/";
    private const string SettingsGroupType = "SettingsGroup";
    private const string FloatKind = "Float";
    private const string BoolKind = "Bool";

    private static readonly object _nullFloat = new();

    private static readonly string[] _kinds =
    {
        BoolKind,
        FloatKind,
        "Range",
        "Dropdown",
        "Keybind",
        "Text",
    };

    [Fact]
    public void All_TargetsExistWithMatchingKind()
    {
        GameEnvironment.SkipWithoutModBuild();
        Dictionary<(string Group, string Key), DeclaredSetting> declared = ReadDeclaredSettings();

        IEnumerable<string> bad = LegacySettingsMap
            .All.Where(m =>
                !declared.TryGetValue((m.Group, m.Key), out DeclaredSetting setting)
                || setting.Kind != KindOf(m.Part)
            )
            .Select(m => m.Group + "." + m.Key + " (" + m.Part + ")");

        Assert.Empty(bad);
    }

    [Fact]
    public void All_DeclaredDefaultsEqualLegacyDefaults()
    {
        GameEnvironment.SkipWithoutModBuild();
        Dictionary<(string Group, string Key), DeclaredSetting> declared = ReadDeclaredSettings();
        LegacyFieldMapping[] defaulted = LegacySettingsMap
            .All.Where(m => HasDefaultPart(m.Part))
            .ToArray();
        string[] groups = LegacySettingsMap.All.Select(m => m.Group).Distinct().ToArray();
        Assert.True(
            LegacySettingsImporter.TryImport(
                LegacyDefaultSave.Text,
                LegacySettingsMap.All,
                groups,
                new FakeKeyNames(),
                out JObject sections,
                new List<LegacyImportProblem>()
            )
        );

        IEnumerable<string> bad = defaulted
            .Where(m => !HasDefault(sections, declared, m))
            .Select(m => m.Group + "." + m.Key + " (" + m.Part + ")");

        Assert.Empty(bad);
    }

    [Fact]
    public void All_ValueLimitsEqualDeclaredLimits()
    {
        GameEnvironment.SkipWithoutModBuild();
        Dictionary<(string Group, string Key), DeclaredSetting> declared = ReadDeclaredSettings();

        IEnumerable<string> bad = LegacySettingsMap
            .All.Where(m =>
                m.Part
                    is LegacySettingPart.FloatValue
                        or LegacySettingPart.RangeMin
                        or LegacySettingPart.RangeMax
            )
            .Where(m =>
                !declared.TryGetValue((m.Group, m.Key), out DeclaredSetting setting)
                || setting.MinLimit != m.MinLimit
                || setting.MaxLimit != m.MaxLimit
            )
            .Select(m => m.Group + "." + m.Key);

        Assert.Empty(bad);
    }

    private static bool HasDefaultPart(LegacySettingPart part)
    {
        return part == LegacySettingPart.Bool
            || part == LegacySettingPart.FloatEnabled
            || part == LegacySettingPart.FloatValue
            || part == LegacySettingPart.RangeEnabled
            || part == LegacySettingPart.RangeMin
            || part == LegacySettingPart.RangeMax
            || part == LegacySettingPart.Dropdown
            || part == LegacySettingPart.Keybind
            || part == LegacySettingPart.Text;
    }

    private static bool HasDefault(
        JObject sections,
        Dictionary<(string Group, string Key), DeclaredSetting> declared,
        LegacyFieldMapping mapping
    )
    {
        if (!declared.TryGetValue((mapping.Group, mapping.Key), out DeclaredSetting setting))
        {
            return false;
        }
        JToken imported = sections[mapping.Group]?[mapping.Key];
        return mapping.Part switch
        {
            LegacySettingPart.Bool => (bool?)imported == setting.BoolDefault,
            LegacySettingPart.FloatEnabled => (bool?)imported?["Enabled"] == setting.FloatEnabled,
            LegacySettingPart.RangeEnabled => (bool?)imported?["Enabled"] == setting.RangeEnabled,
            LegacySettingPart.RangeMin => (float?)imported?["Min"] == setting.RangeMin,
            LegacySettingPart.RangeMax => (float?)imported?["Max"] == setting.RangeMax,
            LegacySettingPart.Dropdown => (int?)imported == setting.DropdownDefault,
            LegacySettingPart.Keybind => (string)imported == setting.KeybindDefault,
            LegacySettingPart.Text => (string)imported == setting.TextDefault,
            _ => (float?)imported?["Value"] == setting.FloatValue,
        };
    }

    private static Dictionary<(string Group, string Key), DeclaredSetting> ReadDeclaredSettings()
    {
        Dictionary<(string Group, string Key), DeclaredSetting> declared = new();
        using var module = ModuleDefinition.ReadModule(GameEnvironment.ModDll);
        IEnumerable<MethodDefinition> initializers = module
            .GetTypes()
            .Where(t => t.IsNestedPublic && t.DeclaringType.FullName + "/" == ModSettingsPrefix)
            .SelectMany(t => t.Methods)
            .Where(m => m.IsConstructor && m.IsStatic && m.HasBody);
        foreach (MethodDefinition initializer in initializers)
        {
            ReadInitializer(initializer, declared);
        }
        return declared;
    }

    private static void ReadInitializer(
        MethodDefinition initializer,
        Dictionary<(string Group, string Key), DeclaredSetting> declared
    )
    {
        string group = null;
        FieldReference pending = null;
        List<object> args = new();
        foreach (Instruction instruction in initializer.Body.Instructions)
        {
            if (instruction.OpCode == OpCodes.Stsfld)
            {
                args.Clear();
                pending = null;
                continue;
            }
            if (TryReadConstant(instruction, out object constant))
            {
                args.Add(constant);
                continue;
            }
            if (instruction.Operand is FieldReference field)
            {
                pending = IsLimitsField(field) ? field : pending;
                continue;
            }
            if (instruction.OpCode == OpCodes.Initobj && IsNullableFloat(instruction.Operand))
            {
                args.Add(_nullFloat);
                continue;
            }
            if (instruction.Operand is not MethodReference call)
            {
                continue;
            }
            if (call.DeclaringType.Name == nameof(SliderRange))
            {
                args.Add(ResolveLimit(pending, call.Name));
                pending = null;
                continue;
            }
            if (call.DeclaringType.Name != SettingsGroupType)
            {
                continue;
            }
            if (call.Name == ".ctor")
            {
                group = args.OfType<string>().FirstOrDefault();
                args.Clear();
                continue;
            }
            if (!_kinds.Contains(call.Name))
            {
                continue;
            }
            string key = args.OfType<string>().FirstOrDefault();
            if (group != null && key != null)
            {
                var numbers = args.Where(a => a is not string).ToList();
                declared[(group, key)] = Declare(
                    call.Name,
                    numbers,
                    args.OfType<string>().ToList()
                );
            }
            args.Clear();
        }
    }

    private static DeclaredSetting Declare(string kind, List<object> numbers, List<string> strings)
    {
        if (kind == BoolKind)
        {
            return new DeclaredSetting(
                kind,
                ToInt(numbers, 0) == 1,
                0f,
                false,
                0f,
                0f,
                false,
                0f,
                0f
            );
        }
        if (kind == "Range")
        {
            return DeclareRange(numbers);
        }
        if (kind == "Dropdown")
        {
            return new DeclaredSetting(
                kind,
                false,
                0f,
                false,
                0f,
                0f,
                false,
                0f,
                0f,
                ToInt(numbers, 0)
            );
        }
        if (kind == "Keybind")
        {
            return new DeclaredSetting(
                kind,
                false,
                0f,
                false,
                0f,
                0f,
                false,
                0f,
                0f,
                0,
                strings.Count > 1 ? strings[1] : null
            );
        }
        if (kind == "Text")
        {
            return new DeclaredSetting(
                kind,
                false,
                0f,
                false,
                0f,
                0f,
                false,
                0f,
                0f,
                0,
                null,
                strings.Count > 1 ? strings[1] : null
            );
        }
        if (kind != FloatKind)
        {
            return new DeclaredSetting(kind, false, 0f, false, 0f, 0f, false, 0f, 0f);
        }
        return new DeclaredSetting(
            kind,
            false,
            ToFloat(numbers, 0),
            ToInt(numbers, 1) == 1,
            ToFloat(numbers, 3),
            ToFloat(numbers, 4),
            false,
            0f,
            0f
        );
    }

    private static DeclaredSetting DeclareRange(List<object> numbers)
    {
        float minLimit = ToFloat(numbers, 0);
        float maxLimit = ToFloat(numbers, 1);
        return new DeclaredSetting(
            "Range",
            false,
            0f,
            false,
            minLimit,
            maxLimit,
            ToInt(numbers, 4) == 1,
            ToFloatOr(numbers, 2, minLimit),
            ToFloatOr(numbers, 3, maxLimit)
        );
    }

    private static bool IsNullableFloat(object operand)
    {
        return operand is TypeReference type && type.FullName == "System.Nullable`1<System.Single>";
    }

    private static float ToFloatOr(List<object> numbers, int index, float fallback)
    {
        return index < numbers.Count && ReferenceEquals(numbers[index], _nullFloat)
            ? fallback
            : ToFloat(numbers, index);
    }

    private static int ToInt(List<object> numbers, int index)
    {
        return index < numbers.Count && numbers[index] is int value ? value : -1;
    }

    private static float ToFloat(List<object> numbers, int index)
    {
        return index < numbers.Count && numbers[index] is float value ? value : float.NaN;
    }

    private static bool IsLimitsField(FieldReference field)
    {
        return field.FieldType.Name == nameof(SliderRange)
            && field.DeclaringType.Name.EndsWith("SliderLimits", StringComparison.Ordinal);
    }

    private static float ResolveLimit(FieldReference field, string getter)
    {
        if (field == null)
        {
            return float.NaN;
        }
        Type limits = typeof(SliderRange).Assembly.GetType(field.DeclaringType.FullName);
        SliderRange? range =
            limits == null ? null : SliderLimitsReflection.Find(limits, field.Name);
        if (!range.HasValue)
        {
            return float.NaN;
        }
        return getter == "get_Min" ? range.Value.Min : range.Value.Max;
    }

    private static bool TryReadConstant(Instruction instruction, out object constant)
    {
        constant = instruction.OpCode.Code switch
        {
            Code.Ldstr => instruction.Operand,
            Code.Ldc_R4 => instruction.Operand,
            Code.Ldc_I4_M1 => -1,
            Code.Ldc_I4_0 => 0,
            Code.Ldc_I4_1 => 1,
            Code.Ldc_I4_2 => 2,
            Code.Ldc_I4_3 => 3,
            Code.Ldc_I4_4 => 4,
            Code.Ldc_I4_5 => 5,
            Code.Ldc_I4_6 => 6,
            Code.Ldc_I4_7 => 7,
            Code.Ldc_I4_8 => 8,
            Code.Ldc_I4_S => (int)(sbyte)instruction.Operand,
            Code.Ldc_I4 => instruction.Operand,
            _ => null,
        };
        return constant != null;
    }

    private static string KindOf(LegacySettingPart part)
    {
        return part switch
        {
            LegacySettingPart.FloatEnabled or LegacySettingPart.FloatValue => FloatKind,
            LegacySettingPart.RangeEnabled
            or LegacySettingPart.RangeMin
            or LegacySettingPart.RangeMax => "Range",
            _ => part.ToString(),
        };
    }
}
