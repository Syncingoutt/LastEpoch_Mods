using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Skills;

public class Skills_SpecializationsSlots
{
    public static bool CanRun()
    {
        return Scenes.IsGameScene() && ModSettings.SkillsGeneral.SpecializationSlots.Enabled;
    }

    [HarmonyPatch(typeof(SkillsPanelManager), "OnEnable")]
    public class SkillsPanelManager_OnEnable
    {
        [HarmonyPrefix]
        static void Prefix(ref SkillsPanelManager __instance)
        {
            if (CanRun())
            {
                SpecialisedAbilityManager.getNumberOfSpecialisationSlots(
                    (int)PlayerFinder.localPlayerLevel()
                );
            }
        }
    }

    [HarmonyPatch(typeof(SpecialisedAbilityManager), "getNumberOfSpecialisationSlots")]
    public class SpecialisedAbilityManager_getNumberOfSpecialisationSlots
    {
        [HarmonyPrefix]
        static bool Prefix(ref byte __result, int __0)
        {
            if (CanRun())
            {
                __result = (byte)ModSettings.SkillsGeneral.SpecializationSlots.Value;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
