using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Mobs;

public class Mobs_Drop_Gold_Multiplier
{
    public static bool CanRun()
    {
        return Scenes.IsGameScene() && ModSettings.CharacterMultipliers.GoldDropMultiplier.Enabled;
    }

    [HarmonyPatch(typeof(DeathItemDrop), "Start")]
    public class DeathItemDrop_Start
    {
        [HarmonyPrefix]
        static void Prefix(ref DeathItemDrop __instance)
        {
            if (CanRun())
            {
                __instance.goldMultiplier = ModSettings
                    .CharacterMultipliers
                    .GoldDropMultiplier
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
        static void Prefix(ref float __7, ItemDrop.BaseDropRates __5)
        {
            if (
                (CanRun())
                && ((__5 == ItemDrop.BaseDropRates.Enemy) || (__5 == ItemDrop.BaseDropRates.Chest))
            )
            {
                __7 = ModSettings.CharacterMultipliers.GoldDropMultiplier.Value;
            }
        }
    }
}
