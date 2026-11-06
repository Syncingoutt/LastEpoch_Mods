using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>World &gt; Camera HUD page. State resets on HUD re-bind (Build).</summary>
internal static class WorldCameraPage
{
    private static HudFormPage page;

    public static void Build(HudPageId pageId, GameObject parent, GameObject hud, Font font)
    {
        page = HudFormPage.Build(parent, hud, font, pageId);
        if (page == null)
            return;
        var card = page.AddCard("Camera", "Camera");
        page.AddButton(card, "Set", "Set", Mods.Camera.Camera_Override.Set);
        page.AddButton(card, "Reset", "Reset", Mods.Camera.Camera_Override.ResetToDefault);
        page.AddToggle(
            card,
            "Enable",
            "Enable Camera Override",
            () => ModSettings.ScenesCamera.EnableMod.Value,
            ModSettings.ScenesCamera.EnableMod.Set
        );
        AddSlider(
            card,
            "ZoomMinimum",
            "Zoom Minimum",
            SceneSliderLimits.CameraZoomMinimum,
            ModSettings.ScenesCamera.ZoomMinimum
        );
        AddSlider(
            card,
            "ZoomPerScroll",
            "Zoom Per Scroll",
            SceneSliderLimits.CameraZoomPerScroll,
            ModSettings.ScenesCamera.ZoomPerScroll
        );
        AddSlider(
            card,
            "ZoomSpeed",
            "Zoom Speed",
            SceneSliderLimits.CameraZoomSpeed,
            ModSettings.ScenesCamera.ZoomSpeed
        );
        AddSlider(
            card,
            "DefaultRotation",
            "Default Rotation",
            SceneSliderLimits.CameraDefaultRotation,
            ModSettings.ScenesCamera.DefaultRotation
        );
        AddSlider(
            card,
            "OffsetMinimum",
            "Offset Minimum",
            SceneSliderLimits.CameraOffsetMinimum,
            ModSettings.ScenesCamera.OffsetMinimum
        );
        AddSlider(
            card,
            "OffsetMaximum",
            "Offset Maximum",
            SceneSliderLimits.CameraOffsetMaximum,
            ModSettings.ScenesCamera.OffsetMaximum
        );
        AddSlider(
            card,
            "AngleMinimum",
            "Angle Minimum",
            SceneSliderLimits.CameraAngleMinimum,
            ModSettings.ScenesCamera.AngleMinimum
        );
        AddSlider(
            card,
            "AngleMaximum",
            "Angle Maximum",
            SceneSliderLimits.CameraAngleMaximum,
            ModSettings.ScenesCamera.AngleMaximum
        );
        page.AddToggle(
            card,
            "LoadOnStart",
            "Load On Start",
            () => ModSettings.ScenesCamera.LoadOnStart.Value,
            ModSettings.ScenesCamera.LoadOnStart.Set
        );
    }

    public static void Show() => page?.Show();

    public static void Hide() => page?.Hide();

    public static void Refresh() => page?.RefreshValues();

    private static void AddSlider(
        HudFormPage.Card card,
        string id,
        string label,
        SliderRange range,
        FloatSetting setting
    )
    {
        page.AddToggleSlider(
            card,
            id,
            label,
            string.Empty,
            range.Min,
            range.Max,
            range.WholeNumbers,
            () => setting.Enabled,
            setting.SetEnabled,
            () => setting.Value,
            setting.SetValue
        );
    }
}
