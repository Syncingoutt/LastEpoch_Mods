using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Scenes.Monoliths fields mapped to the ScenesMonoliths group.</summary>
public static class ScenesMonolithsMap
{
    public const string Group = "ScenesMonoliths";

    private const string Prefix = "Scenes.Monoliths.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } = Build();

    private static LegacyFieldMapping[] Build()
    {
        List<LegacyFieldMapping> all = new();
        AddFloat(all, "MaxStability", SceneSliderLimits.MonolithMaxStability);
        AddBool(all, "MaxStabilityOnStart");
        AddFloat(all, "MobsDensity", SceneSliderLimits.MonolithMobsDensity);
        AddFloat(all, "MobsDefeatOnStart", SceneSliderLimits.MonolithMobsDefeat);
        AddBool(all, "ObjectiveReveal");
        AddBool(all, "CompleteObjective");
        AddFloat(all, "BlessingSlots", SceneSliderLimits.MonolithBlessingSlots);
        AddBool(all, "NoLostWhenDie");
        AddBool(all, "MaxStabilityOnStabilityChanged");
        AddBool(all, "DropShadeGauntlet");
        AddBool(all, "Islands");
        return all.ToArray();
    }

    private static void AddBool(List<LegacyFieldMapping> all, string key)
    {
        all.Add(
            new LegacyFieldMapping(Prefix + "Enable_" + key, Group, key, LegacySettingPart.Bool)
        );
    }

    private static void AddFloat(List<LegacyFieldMapping> all, string name, SliderRange limits)
    {
        all.Add(
            new LegacyFieldMapping(
                Prefix + "Enable_" + name,
                Group,
                name,
                LegacySettingPart.FloatEnabled
            )
        );
        all.Add(
            new LegacyFieldMapping(
                Prefix + name,
                Group,
                name,
                LegacySettingPart.FloatValue,
                limits.Min,
                limits.Max
            )
        );
    }
}
