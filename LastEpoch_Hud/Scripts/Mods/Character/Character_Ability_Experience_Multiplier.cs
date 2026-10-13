using HarmonyLib;
using LastEpoch_Hud.Scripts.Core.Character;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Character;

public class Character_Ability_Experience_Multiplier
{
    [HarmonyPatch(typeof(Il2Cpp.ExperienceTracker), "GainExp")]
    public class ExperienceTracker_GainExp
    {
        [HarmonyPrefix]
        static void Prefix(ref long __1)
        {
            if (!Scenes.IsGameScene() || !ModSettings.CharacterMultipliers.Ability.Enabled)
            {
                return;
            }
            __1 = RewardMultiplier.Scale(__1, ModSettings.CharacterMultipliers.Ability.Value);
        }
    }
}
