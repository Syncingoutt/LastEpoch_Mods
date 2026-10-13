using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.Core.Character;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Character;

public class Character_Experience_Multiplier
{
    public static bool CanRun()
    {
        return Scenes.IsGameScene() && ModSettings.CharacterMultipliers.Experience.Enabled;
    }

    [HarmonyPatch(typeof(ExperienceTracker), "GainExp")]
    public class ExperienceTracker_GainExp
    {
        [HarmonyPrefix]
        static void Prefix(ref long __0)
        {
            if (!CanRun())
            {
                return;
            }
            __0 = RewardMultiplier.Scale(__0, ModSettings.CharacterMultipliers.Experience.Value);
        }
    }
}
