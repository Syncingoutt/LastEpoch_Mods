using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Monoliths;

public class Monoliths_StabilityChanged
{
    public static bool CanRun() =>
        Scenes.IsGameScene() && ModSettings.ScenesMonoliths.MaxStabilityOnStabilityChanged.Value;

    [HarmonyPatch(typeof(MonolithZoneManager), "OnBonusStabilityChanged")]
    public class MonolithZoneManager_OnBonusStabilityChanged
    {
        [HarmonyPrefix]
        static void Prefix(ref MonolithZoneManager __instance)
        {
            if (!CanRun())
            {
                return;
            }
            __instance.bonusStablity = __instance.maxBonusStablity;
        }
    }
}
