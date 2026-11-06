using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Items.AutoLoot;

/// <summary>Shared cheap gate for the pickup patches: toggle on, game scene.</summary>
internal static class AutoPickupGate
{
    public static bool IsOn(AutoPickupToggle toggle)
    {
        return IsToggleOn(toggle) && Scenes.IsGameScene();
    }

    private static bool IsToggleOn(AutoPickupToggle toggle)
    {
        return toggle switch
        {
            AutoPickupToggle.Gold => ModSettings.ItemsPickup.AutoPickupGold.Value,
            AutoPickupToggle.Potions => ModSettings.ItemsPickup.AutoPickupPotions.Value,
            AutoPickupToggle.XpTome => ModSettings.ItemsPickup.AutoPickupXpTome.Value,
            AutoPickupToggle.FavorTome => ModSettings.ItemsPickup.AutoPickupFavorTome.Value,
            AutoPickupToggle.MemoryAmber => ModSettings.ItemsPickup.AutoPickupMemoryAmber.Value,
            AutoPickupToggle.Materials => ModSettings.ItemsPickup.AutoPickupMaterials.Value,
            _ => false,
        };
    }
}
