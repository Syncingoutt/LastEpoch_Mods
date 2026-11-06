using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.Core.Items;
using LastEpoch_Hud.Scripts.ModUI;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items.Drop;

public class Items_Drop_LegendaryPotencial
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
        return ModSettings.ItemsDrop.LegendaryPotential.Enabled;
    }

    [HarmonyPatch(typeof(ItemData), "RollLegendaryPotential")]
    public class rollLegendaryPotential
    {
        [HarmonyPrefix]
        static bool Prefix(ref int __result)
        {
            if (!CanRun())
            {
                return true;
            }

            RangeSetting legendary = ModSettings.ItemsDrop.LegendaryPotential;
            __result = LegendaryPotentialRoll.Pick(legendary.Min, legendary.Max, Random.value);
            return false;
        }
    }
}
