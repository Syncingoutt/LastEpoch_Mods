using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Scripts.ModUI.Settings;

namespace LastEpoch_Hud.Scripts.ModUI;

public static partial class ModSettings
{
#pragma warning disable CA1034 // nested like every settings section
    /// <summary>World > Camera override values.</summary>
    public static class ScenesCamera
    {
        public static readonly SettingsGroup Group = new(ScenesCameraMap.Group);

        public static readonly BoolSetting EnableMod = Group.Bool("EnableMod");

        public static readonly FloatSetting ZoomMinimum = Group.Float(
            "ZoomMinimum",
            defaultValue: -7f,
            min: SceneSliderLimits.CameraZoomMinimum.Min,
            max: SceneSliderLimits.CameraZoomMinimum.Max
        );

        public static readonly FloatSetting ZoomPerScroll = Group.Float(
            "ZoomPerScroll",
            min: SceneSliderLimits.CameraZoomPerScroll.Min,
            max: SceneSliderLimits.CameraZoomPerScroll.Max
        );

        public static readonly FloatSetting ZoomSpeed = Group.Float(
            "ZoomSpeed",
            defaultValue: 2f,
            min: SceneSliderLimits.CameraZoomSpeed.Min,
            max: SceneSliderLimits.CameraZoomSpeed.Max
        );

        public static readonly FloatSetting DefaultRotation = Group.Float(
            "DefaultRotation",
            defaultValue: 95f,
            min: SceneSliderLimits.CameraDefaultRotation.Min,
            max: SceneSliderLimits.CameraDefaultRotation.Max
        );

        public static readonly FloatSetting OffsetMinimum = Group.Float(
            "OffsetMinimum",
            defaultValue: -1f,
            min: SceneSliderLimits.CameraOffsetMinimum.Min,
            max: SceneSliderLimits.CameraOffsetMinimum.Max
        );

        public static readonly FloatSetting OffsetMaximum = Group.Float(
            "OffsetMaximum",
            min: SceneSliderLimits.CameraOffsetMaximum.Min,
            max: SceneSliderLimits.CameraOffsetMaximum.Max
        );

        public static readonly FloatSetting AngleMinimum = Group.Float(
            "AngleMinimum",
            defaultValue: 35f,
            min: SceneSliderLimits.CameraAngleMinimum.Min,
            max: SceneSliderLimits.CameraAngleMinimum.Max
        );

        public static readonly FloatSetting AngleMaximum = Group.Float(
            "AngleMaximum",
            defaultValue: 49f,
            min: SceneSliderLimits.CameraAngleMaximum.Min,
            max: SceneSliderLimits.CameraAngleMaximum.Max
        );

        public static readonly BoolSetting LoadOnStart = Group.Bool("LoadOnStart");
    }

    /// <summary>World > Misc minimap toggles.</summary>
    public static class ScenesMinimap
    {
        public static readonly SettingsGroup Group = new(ScenesMinimapMap.Group);

        public static readonly BoolSetting MaxZoomOut = Group.Bool("MaxZoomOut");

        public static readonly BoolSetting RemoveFogOfWar = Group.Bool("RemoveFogOfWar");
    }

    /// <summary>World > Misc dungeon entry toggle.</summary>
    public static class ScenesDungeons
    {
        public static readonly SettingsGroup Group = new(ScenesDungeonsMap.Group);

        public static readonly BoolSetting EnterWithoutKey = Group.Bool("EnterWithoutKey");
    }
#pragma warning restore CA1034
}
