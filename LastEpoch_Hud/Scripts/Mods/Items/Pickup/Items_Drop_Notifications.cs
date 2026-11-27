using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items.Pickup;

public class Items_Drop_Notifications
{
    public static bool CanRun()
    {
        return ModSettings.ItemsPickup.HideMaterialsNotifications.Value && Scenes.IsGameScene();
    }

    [HarmonyPatch(typeof(Notifications), "MaterialAdded")]
    public class Notifications_MaterialAdded
    {
        [HarmonyPrefix]
        static bool Prefix(string __0)
        {
            if (CanRun())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Notifications), "RuneOrGlyphAddedNotification")]
    public class Notifications_RuneOrGlyphAddedNotification
    {
        [HarmonyPrefix]
        static bool Prefix(string __0, int __1)
        {
            if (CanRun())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Notifications), "ShardAdded")]
    public class Notifications_ShardAdded
    {
        [HarmonyPrefix]
        static bool Prefix(int __0, int __1)
        {
            if (CanRun())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Notifications), "MultiShardsAdded")]
    public class Notifications_MultiShardsAdded
    {
        [HarmonyPrefix]
        static bool Prefix(int __0)
        {
            if (CanRun())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Notifications), "showBufferedCraftingNotifications")]
    public class Notifications_showBufferedCraftingNotifications
    {
        [HarmonyPrefix]
        static bool Prefix(ref Notifications __instance)
        {
            if (CanRun())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
