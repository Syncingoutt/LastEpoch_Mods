using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Mobs;

public class Mobs_Density
{
    public static bool CanRun()
    {
        return Scenes.IsGameScene() && ModSettings.CharacterMultipliers.Density.Enabled;
    }

    public static void Apply(SpawnerPlacementManager manager)
    {
        if ((!CanRun()) || (manager.IsNullOrDestroyed()))
        {
            return;
        }
        manager.defaultSpawnerDensity = ModSettings.CharacterMultipliers.Density.Value;
        manager.alwaysRollSpawnerDensity = false;
    }

    [HarmonyPatch(typeof(SpawnerPlacementManager), "RollSpawners", new System.Type[] { })]
    public class RollSpawners_NoArgs
    {
        [HarmonyPrefix]
        static void Prefix(SpawnerPlacementManager __instance)
        {
            Apply(__instance);
        }
    }

    [HarmonyPatch(
        typeof(SpawnerPlacementManager),
        "RollSpawners",
        new System.Type[] { typeof(SpawnerPlacementRoom.SpawnerRuntimeConfig) }
    )]
    public class RollSpawners_Config
    {
        [HarmonyPrefix]
        static void Prefix(SpawnerPlacementManager __instance)
        {
            Apply(__instance);
        }
    }

    [HarmonyPatch(typeof(SpawnerPlacementManager), "rollSpawnersNPerFrame")]
    public class RollSpawners_PerFrame
    {
        [HarmonyPrefix]
        static void Prefix(SpawnerPlacementManager __instance)
        {
            Apply(__instance);
        }
    }
}
