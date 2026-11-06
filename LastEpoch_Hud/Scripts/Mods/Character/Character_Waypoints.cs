using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Character;

public class Character_Waypoints
{
    public static bool CanRun()
    {
        return Scenes.IsGameScene() && ModSettings.CharacterCheats.WaypointsUnlock.Value;
    }

    [HarmonyPatch(typeof(UIWaypointStandard), "OnPointerEnter")]
    public class UIWaypointStandard_OnPointerEnter
    {
        [HarmonyPrefix]
        static void Prefix(ref UIWaypointStandard __instance)
        {
            if (CanRun())
            {
                __instance.isActive = true;
            }
        }
    }
}
