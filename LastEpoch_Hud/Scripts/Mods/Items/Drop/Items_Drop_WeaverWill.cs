using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items.Drop;

public class Items_Drop_WeaverWill
{
    public static bool CanRun()
    {
        if (Hud_Manager.IsPauseOpen() && ModUI.ForceDrop.ForceDropModel.Enabled)
        {
            return false;
        }
        if (!Scenes.IsGameScene())
        {
            return false;
        }
        return ModSettings.ItemsDrop.WeaverWill.Enabled;
    }

    [HarmonyPatch(typeof(ItemData), "RollWeaversWill")]
    public class RollWeaversWill
    {
        [HarmonyPrefix]
        static bool Prefix(
            ref ItemData __instance,
            ref int __result,
            UniqueList.Entry __0,
            int __1,
            int __2
        )
        {
            if (CanRun())
            {
                int roll = 0;
                if (ModSettings.ItemsDrop.WeaverWill.Min == ModSettings.ItemsDrop.WeaverWill.Max)
                {
                    roll = (int)ModSettings.ItemsDrop.WeaverWill.Max;
                }
                else
                {
                    roll = (int)
                        Random.RandomRange(
                            ModSettings.ItemsDrop.WeaverWill.Min,
                            ModSettings.ItemsDrop.WeaverWill.Max
                        );
                }
                __result = roll;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
