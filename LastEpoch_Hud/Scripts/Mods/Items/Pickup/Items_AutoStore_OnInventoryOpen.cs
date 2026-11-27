using HarmonyLib;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Items.Pickup;

public class Items_AutoStore_OnInventoryOpen
{
    public static bool CanRun()
    {
        return ModSettings.ItemsPickup.AutoStoreOnInventoryOpen.Value && Scenes.IsGameScene();
    }

    [HarmonyPatch(typeof(Il2CppLE.UI.PanelSystem.InventoryPanel), "OnOpen")]
    public class InventoryPanel_OnOpen
    {
        [HarmonyPostfix]
        static void Postfix()
        {
            if (CanRun())
            {
                Items_AutoStore_OnPickup.StoreNow();
            }
        }
    }
}
