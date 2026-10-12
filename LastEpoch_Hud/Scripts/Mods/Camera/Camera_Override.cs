using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Camera;

public class Camera_Override
{
    private const float GameZoomMinimum = -7f;
    private const float GameZoomPerScroll = 0.15f;
    private const float GameZoomSpeed = 2.5f;
    private const float GameDefaultRotation = 95f;
    private const float GameOffsetMinimum = -1f;
    private const float GameOffsetMaximum = 0.31f;
    private const float GameAngleMinimum = 35f;
    private const float GameAngleMaximum = 49f;

    /// <summary>Writes each enabled camera setting to the game camera.</summary>
    public static void Set()
    {
        if (!Scenes.IsGameScene())
        {
            return;
        }
        CameraManager camera = Refs_Manager.camera_manager;
        if (camera.IsNullOrDestroyed())
        {
            return;
        }
        if (ModSettings.ScenesCamera.ZoomMinimum.Enabled)
        {
            camera.zoomMin = ModSettings.ScenesCamera.ZoomMinimum.Value;
        }
        if (ModSettings.ScenesCamera.ZoomPerScroll.Enabled)
        {
            camera.zoomPerScroll = ModSettings.ScenesCamera.ZoomPerScroll.Value;
        }
        if (ModSettings.ScenesCamera.ZoomSpeed.Enabled)
        {
            camera.zoomSpeed = ModSettings.ScenesCamera.ZoomSpeed.Value;
        }
        if (ModSettings.ScenesCamera.DefaultRotation.Enabled)
        {
            camera.cameraRotationDefault = ModSettings.ScenesCamera.DefaultRotation.Value;
        }
        if (ModSettings.ScenesCamera.OffsetMinimum.Enabled)
        {
            camera.offsetMin = ModSettings.ScenesCamera.OffsetMinimum.Value;
        }
        if (ModSettings.ScenesCamera.OffsetMaximum.Enabled)
        {
            camera.offsetMax = ModSettings.ScenesCamera.OffsetMaximum.Value;
        }
        if (ModSettings.ScenesCamera.AngleMinimum.Enabled)
        {
            camera.cameraAngleMin = ModSettings.ScenesCamera.AngleMinimum.Value;
        }
        if (ModSettings.ScenesCamera.AngleMaximum.Enabled)
        {
            camera.cameraAngleMax = ModSettings.ScenesCamera.AngleMaximum.Value;
        }
    }

    /// <summary>Stores the game camera values and applies them.</summary>
    public static void ResetToDefault()
    {
        if (!Scenes.IsGameScene())
        {
            return;
        }
        CameraManager camera = Refs_Manager.camera_manager;
        if (camera.IsNullOrDestroyed())
        {
            return;
        }
        StoreGameValues();
        ApplyAll(camera);
    }

    private static void StoreGameValues()
    {
        ModSettings.ScenesCamera.ZoomMinimum.SetValue(GameZoomMinimum);
        ModSettings.ScenesCamera.ZoomPerScroll.SetValue(GameZoomPerScroll);
        ModSettings.ScenesCamera.ZoomSpeed.SetValue(GameZoomSpeed);
        ModSettings.ScenesCamera.DefaultRotation.SetValue(GameDefaultRotation);
        ModSettings.ScenesCamera.OffsetMinimum.SetValue(GameOffsetMinimum);
        ModSettings.ScenesCamera.OffsetMaximum.SetValue(GameOffsetMaximum);
        ModSettings.ScenesCamera.AngleMinimum.SetValue(GameAngleMinimum);
        ModSettings.ScenesCamera.AngleMaximum.SetValue(GameAngleMaximum);
    }

    private static void ApplyAll(CameraManager camera)
    {
        camera.zoomMin = ModSettings.ScenesCamera.ZoomMinimum.Value;
        camera.zoomPerScroll = ModSettings.ScenesCamera.ZoomPerScroll.Value;
        camera.zoomSpeed = ModSettings.ScenesCamera.ZoomSpeed.Value;
        camera.cameraRotationDefault = ModSettings.ScenesCamera.DefaultRotation.Value;
        camera.offsetMin = ModSettings.ScenesCamera.OffsetMinimum.Value;
        camera.offsetMax = ModSettings.ScenesCamera.OffsetMaximum.Value;
        camera.cameraAngleMin = ModSettings.ScenesCamera.AngleMinimum.Value;
        camera.cameraAngleMax = ModSettings.ScenesCamera.AngleMaximum.Value;
    }
}
