using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Mobs;

public class Mobs_Drop_Gold_Chance
{
    public static bool CanRun()
    {
        return Scenes.IsGameScene() && ModSettings.CharacterMultipliers.GoldDropChance.Enabled;
    }

    [HarmonyPatch(typeof(DeathItemDrop), "Start")]
    public class DeathItemDrop_Start
    {
        [HarmonyPrefix]
        static void Prefix(ref DeathItemDrop __instance)
        {
            if (!CanRun())
            {
                return;
            }
            __instance.goldDropChance = ModSettings.CharacterMultipliers.GoldDropChance.Value;
        }
    }
}
