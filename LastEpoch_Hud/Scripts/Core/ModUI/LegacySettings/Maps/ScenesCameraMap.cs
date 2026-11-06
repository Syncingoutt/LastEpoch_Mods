using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Scenes.Camera fields mapped to the ScenesCamera group.</summary>
public static class ScenesCameraMap
{
    public const string Group = "ScenesCamera";

    private const string Prefix = "Scenes.Camera.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } = Build();

    private static LegacyFieldMapping[] Build()
    {
        List<LegacyFieldMapping> all = new();
        AddBool(all, "Enable_Mod", "EnableMod");
        AddBool(all, "Enable_LoadOnStart", "LoadOnStart");
        AddFloat(all, "ZoomMinimum", SceneSliderLimits.CameraZoomMinimum);
        AddFloat(all, "ZoomPerScroll", SceneSliderLimits.CameraZoomPerScroll);
        AddFloat(all, "ZoomSpeed", SceneSliderLimits.CameraZoomSpeed);
        AddFloat(all, "DefaultRotation", SceneSliderLimits.CameraDefaultRotation);
        AddFloat(all, "OffsetMinimum", SceneSliderLimits.CameraOffsetMinimum);
        AddFloat(all, "OffsetMaximum", SceneSliderLimits.CameraOffsetMaximum);
        AddFloat(all, "AngleMinimum", SceneSliderLimits.CameraAngleMinimum);
        AddFloat(all, "AngleMaximum", SceneSliderLimits.CameraAngleMaximum);
        return all.ToArray();
    }

    private static void AddBool(List<LegacyFieldMapping> all, string legacyName, string key)
    {
        all.Add(new LegacyFieldMapping(Prefix + legacyName, Group, key, LegacySettingPart.Bool));
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
