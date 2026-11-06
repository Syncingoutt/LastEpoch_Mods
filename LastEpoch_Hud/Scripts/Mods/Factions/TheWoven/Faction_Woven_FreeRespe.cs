using HarmonyLib;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Factions.TheWoven;

public class Faction_Woven_FreeRespe
{
    public static bool CanRun() =>
        Scenes.IsGameScene() && ModSettings.FactionsWoven.FreeRespec.Value;

    [HarmonyPatch(typeof(Il2CppLE.Factions.TheWeaver), "GetMemoryAmberRespecCostForWeaverTree")]
    public class TheWeaver_GetMemoryAmberRespecCostForWeaverTree
    {
        [HarmonyPrefix]
        static bool Prefix(ref int __result)
        {
            if (!CanRun())
            {
                return true;
            }
            __result = 0;
            return false;
        }
    }
}
