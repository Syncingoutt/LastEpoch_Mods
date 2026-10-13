using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Items.Pickup;

public class Items_RangePickup
{
    public static bool CanRun()
    {
        return ModSettings.ItemsPickup.RangePickup.Value
            && Scenes.IsGameScene()
            && !Refs_Manager.ground_item_manager.IsNullOrDestroyed()
            && !Refs_Manager.player_actor.IsNullOrDestroyed();
    }

    [HarmonyPatch(typeof(GroundItemLabel), "ClickedItem")]
    public class ClickedItem
    {
        [HarmonyPrefix]
        static bool Prefix(ref GroundItemLabel __instance)
        {
            if (CanRun())
            {
                bool found = false;
                ItemDataUnpacked item_data = __instance.getItemData();
                foreach (
                    GroundItemManager.GroundItem ground_item in Refs_Manager
                        .ground_item_manager
                        .offlineItems
                        .items
                )
                {
                    if (item_data.Pointer == ground_item.itemData.Pointer)
                    {
                        found = true;
                        ground_item.location = Refs_Manager.player_actor.position();
                        GroundItemManager.instance.pickupItem(
                            Refs_Manager.player_actor,
                            ground_item.id,
                            // Same flags as the game's click (GroundItemLabel.requestPickup): stackables go to storage.
                            StackableItemFlags.All
                        );
                        break;
                    }
                }
                if (found)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
