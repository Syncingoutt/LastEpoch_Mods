using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items.Drop;

public class Items_Drop_UniqueMods
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
        return ModSettings.ItemsDrop.UniqueMods.Enabled;
    }

    [HarmonyPatch(typeof(ItemData), "randomiseUniqueRolls")]
    public class randomiseUniqueRolls
    {
        [HarmonyPostfix]
        static void Postfix(ItemData __instance)
        {
            if (CanRun() && !__instance.IsNullOrDestroyed())
            {
                for (int k = 0; k < __instance.uniqueRolls.Count; k++)
                {
                    byte roll = 0;
                    if (
                        ModSettings.ItemsDrop.UniqueMods.Min == ModSettings.ItemsDrop.UniqueMods.Max
                    )
                    {
                        roll = (byte)ModSettings.ItemsDrop.UniqueMods.Max;
                    }
                    else
                    {
                        roll = (byte)
                            Random.RandomRange(
                                ModSettings.ItemsDrop.UniqueMods.Min,
                                ModSettings.ItemsDrop.UniqueMods.Max
                            );
                    }
                    __instance.uniqueRolls[k] = roll;
                }
                // Only change the rolls here. Ascendance can call this before it has
                // finished assigning the unique identity; rebuilding or refreshing now
                // can serialize that intermediate base item. The caller owns finalization.
            }
        }
    }
}
