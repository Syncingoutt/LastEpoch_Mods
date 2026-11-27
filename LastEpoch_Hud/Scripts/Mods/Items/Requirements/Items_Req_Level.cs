using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Items.Requirements;

public class Items_Req_Level
{
    public static bool need_update = false;

    public static void Enable()
    {
        if (CanRun())
        {
            need_update = true;
        }
    }

    private static bool CanRun()
    {
        return ModSettings.ItemsRequirements.Level.Value
            && !Refs_Manager.item_list.IsNullOrDestroyed();
    }

    [HarmonyPatch(typeof(ItemData), "CalculateLevelAndClassRequirement")]
    public class CalculateLevelAndClassRequirement
    {
        [HarmonyPostfix]
        static void Postfix(ref int __result)
        {
            if (CanRun())
            {
                __result = 0;
            }
        }
    }
}
