using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Skills;

public class Skills_Companions
{
    [HarmonyPatch(typeof(AbilityStatsMutatorManager), "OnStatsUpdate")]
    public class AbilityStatsMutatorManager_OnStatsUpdate
    {
        [HarmonyPostfix]
        static void Postfix(ref AbilityStatsMutatorManager __instance)
        {
            if (!Scenes.IsGameScene())
            {
                return;
            }
            if (ModSettings.SkillsCompanions.WolfSummonMaximum.Value)
            {
                __instance.canSummonWolvesUpToMaxCompanions = true;
            }
            __instance.wolfStunImmunity = ModSettings.SkillsCompanions.WolfStunImmunity.Value;
            ApplyBoneGolemsPerSkeletons(__instance);
        }

        private static void ApplyBoneGolemsPerSkeletons(AbilityStatsMutatorManager manager)
        {
            if (!ModSettings.SkillsMinions.BoneGolemPerSkeletons.Enabled)
            {
                return;
            }
            manager.addedGolemsPer4Skeletons = (int)
                ModSettings.SkillsMinions.BoneGolemPerSkeletons.Value;
        }
    }

    [HarmonyPatch(typeof(CharacterStats), "getMaximumCompanions")]
    public class CharacterStats_GetMaximumCompanions
    {
        [HarmonyPostfix]
        static void Postfix(CharacterStats __instance, ref int __result)
        {
            if (!Scenes.IsGameScene())
            {
                return;
            }
            if (!ModSettings.SkillsCompanions.MaximumCompanions.Enabled)
            {
                return;
            }
            __result = (int)ModSettings.SkillsCompanions.MaximumCompanions.Value;
        }
    }
}
