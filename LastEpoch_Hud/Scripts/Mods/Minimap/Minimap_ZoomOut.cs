using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Minimap;

internal class Minimap_ZoomOut
{
    private const float GameMaxZoom = 37.5f;

    /// <summary>True in a game scene with Max Zoom Out on.</summary>
    public static bool CanRun() =>
        Scenes.IsGameScene() && ModSettings.ScenesMinimap.MaxZoomOut.Value;

    [HarmonyPatch(typeof(DMMapZoom), "ZoomOutMinimap")]
    public class DMMapZoom_ZoomOutMinimap
    {
        public static void Prefix(ref DMMapZoom __instance)
        {
            __instance.maxMinimapZoom = CanRun() ? float.MaxValue : GameMaxZoom;
        }
    }
}
