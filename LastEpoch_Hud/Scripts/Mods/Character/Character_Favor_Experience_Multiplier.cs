using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.Core.Character;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Character;

public class Character_Favor_Experience_Multiplier
{
    public static bool CanRun()
    {
        return Scenes.IsGameScene() && ModSettings.CharacterMultipliers.Favor.Enabled;
    }

    [HarmonyPatch(typeof(ExperienceTracker), "GainExp")]
    public class ExperienceTracker_GainExp
    {
        [HarmonyPrefix]
        static void Prefix(ref long __2)
        {
            if (!CanRun())
            {
                return;
            }
            __2 = RewardMultiplier.Scale(__2, ModSettings.CharacterMultipliers.Favor.Value);
        }
    }
}
