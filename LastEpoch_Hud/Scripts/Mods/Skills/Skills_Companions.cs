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
            ApplyLegacyBoneGolems(__instance);
        }

        // Removed with the minions settings migration.
        private static void ApplyLegacyBoneGolems(AbilityStatsMutatorManager manager)
        {
            if (Save_Manager.instance.IsNullOrDestroyed())
            {
                return;
            }
            if (Save_Manager.instance.data.IsNullOrDestroyed())
            {
                return;
            }
            Save_Manager.Data.BoneGolems golems = Save_Manager
                .instance
                .data
                .Skills
                .Minions
                .BoneGolems;
            if (!golems.Enable_addedGolemsPer4Skeletons)
            {
                return;
            }
            manager.addedGolemsPer4Skeletons = golems.addedGolemsPer4Skeletons;
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
