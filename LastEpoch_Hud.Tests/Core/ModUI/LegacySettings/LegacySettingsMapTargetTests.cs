using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Tests.Support;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings;

public sealed class LegacySettingsMapTargetTests
{
    private const string ModSettingsPrefix = "LastEpoch_Hud.Scripts.ModUI.ModSettings/";
    private const string SettingsGroupType = "SettingsGroup";

    private static readonly string[] _kinds = { "Bool", "Float", "Range", "Dropdown", "Keybind" };

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
        LegacyFieldMapping[] bools = LegacySettingsMap
            .All.Where(m => m.Part == LegacySettingPart.Bool)
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

        IEnumerable<string> bad = bools
            .Where(m => !HasDefault(sections, declared, m))
            .Select(m => m.Group + "." + m.Key);

        Assert.Empty(bad);
    }

    private static bool HasDefault(
        JObject sections,
        Dictionary<(string Group, string Key), DeclaredSetting> declared,
        LegacyFieldMapping mapping
    )
    {
        JToken imported = sections[mapping.Group]?[mapping.Key];
        return imported != null
            && declared.TryGetValue((mapping.Group, mapping.Key), out DeclaredSetting setting)
            && (bool)imported == setting.BoolDefault;
    }

    private static Dictionary<(string Group, string Key), DeclaredSetting> ReadDeclaredSettings()
    {
        Dictionary<(string Group, string Key), DeclaredSetting> declared = new();
        using var module = ModuleDefinition.ReadModule(GameEnvironment.ModDll);
        IEnumerable<MethodDefinition> initializers = module
            .GetTypes()
            .Where(t => t.FullName.StartsWith(ModSettingsPrefix, StringComparison.Ordinal))
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
        bool? flag = null;
        List<string> strings = new();
        foreach (Instruction instruction in initializer.Body.Instructions)
        {
            if (instruction.OpCode == OpCodes.Ldstr)
            {
                strings.Add((string)instruction.Operand);
                continue;
            }
            if (strings.Count > 0 && flag == null && IsBoolConstant(instruction))
            {
                flag = instruction.OpCode == OpCodes.Ldc_I4_1;
                continue;
            }
            if (instruction.Operand is not MethodReference call)
            {
                continue;
            }
            string first = strings.FirstOrDefault();
            strings.Clear();
            bool defaultValue = flag == true;
            flag = null;
            if (call.DeclaringType.Name != SettingsGroupType)
            {
                continue;
            }
            if (call.Name == ".ctor")
            {
                group = first;
                continue;
            }
            if (group == null || first == null || !_kinds.Contains(call.Name))
            {
                continue;
            }
            declared[(group, first)] = new DeclaredSetting(call.Name, defaultValue);
        }
    }

    private static bool IsBoolConstant(Instruction instruction)
    {
        return instruction.OpCode == OpCodes.Ldc_I4_0 || instruction.OpCode == OpCodes.Ldc_I4_1;
    }

    private static string KindOf(LegacySettingPart part)
    {
        return part switch
        {
            LegacySettingPart.FloatEnabled or LegacySettingPart.FloatValue => "Float",
            LegacySettingPart.RangeEnabled
            or LegacySettingPart.RangeMin
            or LegacySettingPart.RangeMax => "Range",
            _ => part.ToString(),
        };
    }
}
