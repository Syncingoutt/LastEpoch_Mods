using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Mobs;

public class Mobs_Drop_Item_Multiplier
{
    public static bool CanRun()
    {
        return Scenes.IsGameScene() && ModSettings.CharacterMultipliers.ItemDropMultiplier.Enabled;
    }

    [HarmonyPatch(typeof(DeathItemDrop), "Start")]
    public class DeathItemDrop_Start
    {
        [HarmonyPrefix]
        static void Prefix(ref DeathItemDrop __instance)
        {
            if (CanRun())
            {
                __instance.itemMultiplier = ModSettings
                    .CharacterMultipliers
                    .ItemDropMultiplier
                    .Value;
            }
        }
    }

    [HarmonyPatch(
        typeof(ItemDrop),
        "DropItem",
        new System.Type[]
        {
            typeof(int),
            typeof(UnityEngine.Vector3),
            typeof(float),
            typeof(bool),
            typeof(float),
            typeof(ItemDrop.BaseDropRates),
            typeof(bool),
            typeof(float),
            typeof(float),
            typeof(float),
            typeof(ItemDrop.DropFlags),
            typeof(UnityEngine.SceneManagement.Scene),
            typeof(bool),
            typeof(bool),
            typeof(bool),
            typeof(ItemDrop.GoldDropType),
            typeof(CorruptionOutcome),
            typeof(float),
        }
    )]
    public class ItemDrop_DropItem
    {
        [HarmonyPrefix]
        static void Prefix(ref float __4, ItemDrop.BaseDropRates __5)
        {
            if (
                (CanRun())
                && ((__5 == ItemDrop.BaseDropRates.Enemy) || (__5 == ItemDrop.BaseDropRates.Chest))
            )
            {
                __4 = ModSettings.CharacterMultipliers.ItemDropMultiplier.Value;
            }
        }
    }
}
