using HarmonyLib;
using LastEpoch_Hud.Scripts.Core.Character;
using LastEpoch_Hud.Scripts.ModUI;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character;

public class Character_MemoryAmber_Multiplier
{
    public static bool CanRun()
    {
        return Scenes.IsGameScene()
            && !Refs_Manager.player_actor.IsNullOrDestroyed()
            && ModSettings.CharacterMultipliers.MemoryAmber.Enabled;
    }

    [HarmonyPatch(
        typeof(Il2CppLE.Factions.PickupableObjectsManager),
        "CreatePickupableObjectForPlayer",
        new System.Type[]
        {
            typeof(Il2CppLE.Factions.PickupableObjectType),
            typeof(Il2CppLE.Factions.PickupableObjectSet),
            typeof(Vector3),
            typeof(uint),
        }
    )]
    public class PickupableObjectsManager_CreatePickupableObjectForPlayer2
    {
        [HarmonyPrefix]
        static void Prefix(
            Il2CppLE.Factions.PickupableObjectType __0,
            Il2CppLE.Factions.PickupableObjectSet __1,
            ref uint __3
        )
        {
            if (
                CanRun()
                && __0 == Il2CppLE.Factions.PickupableObjectType.MemoryAmber
                && !__1.IsNullOrDestroyed()
                && __1.PlayerActor == Refs_Manager.player_actor
            )
            {
                __3 = RewardMultiplier.ScaleSaturated(
                    __3,
                    ModSettings.CharacterMultipliers.MemoryAmber.Value
                );
            }
        }
    }
}
