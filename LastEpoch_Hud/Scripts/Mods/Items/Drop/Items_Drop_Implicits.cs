using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items.Drop;

public class Items_Drop_Implicits
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
        return ModSettings.ItemsDrop.Implicits.Enabled;
    }

    [HarmonyPatch(typeof(ItemData), "randomiseImplicitRolls")]
    public class ItemData_randomiseImplicitRolls
    {
        [HarmonyPostfix]
        static void Postfix(ItemData __instance)
        {
            if (CanRun() && !__instance.IsNullOrDestroyed())
            {
                for (int z = 0; z < __instance.implicitRolls.Count; z++)
                {
                    byte roll = 0;
                    if (ModSettings.ItemsDrop.Implicits.Min == ModSettings.ItemsDrop.Implicits.Max)
                    {
                        roll = (byte)ModSettings.ItemsDrop.Implicits.Max;
                    }
                    else
                    {
                        roll = (byte)
                            Random.RandomRange(
                                ModSettings.ItemsDrop.Implicits.Min,
                                ModSettings.ItemsDrop.Implicits.Max
                            );
                    }
                    __instance.implicitRolls[z] = roll;
                }
                // Only change the rolls here. Ascendance can call this before it has
                // finished assigning the unique identity; rebuilding or refreshing now
                // can serialize that intermediate base item. The caller owns finalization.
            }
        }
    }
}
