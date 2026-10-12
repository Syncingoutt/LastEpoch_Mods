using LastEpoch_Hud.Tests.Support;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LastEpoch_Hud.Tests.Patches;

/// <summary>Weaver tree settings have one write path; no legacy Slider hooks.</summary>
public sealed class WeaverWritePathTests
{
    private const string PageType = "LastEpoch_Hud.Scripts.ModUI.Pages.UtilitiesCharacterPage";
    private const string WovenSettingsType =
        "LastEpoch_Hud.Scripts.ModUI.ModSettings/FactionsWoven";
    private const string WovenModsNamespace = "LastEpoch_Hud.Scripts.Mods.Factions.TheWoven";
    private const string SettingsNamespace = "LastEpoch_Hud.Scripts.ModUI.Settings";

    private static readonly string[] _wovenModTypes =
    {
        WovenModsNamespace + ".Faction_Woven_TreePoints",
        WovenModsNamespace + ".Faction_Woven_FreeRespe",
    };

    private static readonly string[] _setterNames =
    {
        "Set",
        "SetEnabled",
        "SetValue",
        "set_Value",
        "set_Enabled",
    };

    [Fact]
    public void Slider_IsPatchedOnlyBySliderHook()
    {
        GameEnvironment.SkipWithoutModBuild();
        using var module = ModuleDefinition.ReadModule(GameEnvironment.ModDll);

        PatchDeclaration sliderPatch = Assert.Single(
            PatchScanner.Scan(module),
            patch => patch.Target.FullName == "UnityEngine.UI.Slider"
        );

        Assert.Equal(
            "LastEpoch_Hud.Scripts.ModUI.Settings.SliderHook+SliderSetValuePatch",
            sliderPatch.PatchClass
        );
    }

    [Fact]
    public void WovenSettings_AreUsedOnlyByPageAndWovenMods()
    {
        GameEnvironment.SkipWithoutModBuild();
        using var module = ModuleDefinition.ReadModule(GameEnvironment.ModDll);

        var users = module
            .GetTypes()
            .Where(type => type.FullName != WovenSettingsType)
            .SelectMany(type => type.Methods)
            .Where(method => method.HasBody && LoadsWovenSetting(method))
            .Select(method => OutermostType(method.DeclaringType).FullName)
            .ToHashSet();

        Assert.Equal(new HashSet<string>(_wovenModTypes) { PageType }, users);
    }

    [Fact]
    public void WovenMods_NeverWriteSettings()
    {
        GameEnvironment.SkipWithoutModBuild();
        using var module = ModuleDefinition.ReadModule(GameEnvironment.ModDll);

        var writers = module
            .GetTypes()
            .Where(type => OutermostType(type).Namespace == WovenModsNamespace)
            .SelectMany(type => type.Methods)
            .Where(method => method.HasBody && CallsSettingSetter(method))
            .Select(method => method.FullName)
            .ToList();

        Assert.Empty(writers);
    }

    private static bool LoadsWovenSetting(MethodDefinition method)
    {
        return method.Body.Instructions.Any(instruction =>
            instruction.OpCode == OpCodes.Ldsfld
            && instruction.Operand is FieldReference field
            && field.DeclaringType.FullName == WovenSettingsType
        );
    }

    private static bool CallsSettingSetter(MethodDefinition method)
    {
        return method.Body.Instructions.Any(instruction =>
            IsCallLike(instruction.OpCode)
            && instruction.Operand is MethodReference called
            && _setterNames.Contains(called.Name)
            && OutermostType(called.DeclaringType).Namespace == SettingsNamespace
        );
    }

    private static bool IsCallLike(OpCode code)
    {
        return code == OpCodes.Call || code == OpCodes.Callvirt || code == OpCodes.Ldftn;
    }

    private static TypeReference OutermostType(TypeReference type)
    {
        while (type.DeclaringType != null)
        {
            type = type.DeclaringType;
        }
        return type;
    }
}
