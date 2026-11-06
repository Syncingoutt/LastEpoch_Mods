using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Monoliths;

public class Monoliths_Islands
{
    public static bool CanRun() =>
        Scenes.IsGameScene() && ModSettings.ScenesMonoliths.Islands.Value;

    [HarmonyPatch(typeof(EchoWeb), "islandCanBeRun")]
    public class EchoWeb_islandCanBeRun
    {
        [HarmonyPostfix]
        static void Postfix(EchoWeb __instance, ref bool __result, EchoWebIsland __0)
        {
            if (!CanRun())
            {
                return;
            }
            __result = true;
        }
    }
}
