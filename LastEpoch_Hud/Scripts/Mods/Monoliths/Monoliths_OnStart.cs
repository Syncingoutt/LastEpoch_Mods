using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Monoliths;

public class Monoliths_OnStart
{
    public static bool CanRun() =>
        Scenes.IsGameScene()
        && !Refs_Manager.player_actor.IsNullOrDestroyed()
        && AnyStartOptionOn();

    private static bool AnyStartOptionOn() =>
        ModSettings.ScenesMonoliths.MaxStability.Enabled
        || ModSettings.ScenesMonoliths.MaxStabilityOnStart.Value
        || ModSettings.ScenesMonoliths.MobsDefeatOnStart.Enabled
        || ModSettings.ScenesMonoliths.ObjectiveReveal.Value;

    //MonolithProgressManager

    [HarmonyPatch(typeof(MonolithZoneManager), "initialise")]
    public class MonolithZoneManager_initialise
    {
        [HarmonyPostfix]
        static void Postfix(ref MonolithZoneManager __instance, StatefulQuestList __0)
        {
            if (Refs_Manager.monolith_zone_manager.IsNullOrDestroyed())
            {
                Refs_Manager.monolith_zone_manager = __instance;
            }
            if (!CanRun())
            {
                return;
            }
            if (ModSettings.ScenesMonoliths.MaxStability.Enabled)
            {
                __instance.maxBonusStablity = (int)ModSettings.ScenesMonoliths.MaxStability.Value;
            }
            if (ModSettings.ScenesMonoliths.MaxStabilityOnStart.Value)
            {
                __instance.bonusStablity = __instance.maxBonusStablity;
            }
            if (ModSettings.ScenesMonoliths.MobsDefeatOnStart.Enabled)
            {
                __instance.enemiesDefeated = ModSettings.ScenesMonoliths.MobsDefeatOnStart.Value;
            }
            if (ModSettings.ScenesMonoliths.ObjectiveReveal.Value)
            {
                __instance.objectiveRevealThresholdModifier = float.MaxValue;
            }
            if (
                ModSettings.ScenesMonoliths.DropShadeGauntlet.Value
                && !Refs_Manager.player_actor.IsNotNullOrDestroyed()
            )
            {
                //Drop woven echoes on start
                __instance.DropShadeGauntletWovenEchoForAllPlayersInEcho(
                    Refs_Manager.player_actor.gameObject.transform.position,
                    0
                );
            }
        }
    }
}
