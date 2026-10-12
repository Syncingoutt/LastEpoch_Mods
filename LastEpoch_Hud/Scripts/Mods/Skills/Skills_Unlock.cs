using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Skills;

public class Skills_UnlockPassives_SpendPoints
{
    public static bool CanRun()
    {
        return Scenes.IsGameScene() && ModSettings.SkillsGeneral.UnlockAllSkills.Value;
    }

    [HarmonyPatch(typeof(CharacterClass), "getLevelUnlockedBaseClassAbilities")]
    public class CharacterClass_getLevelUnlockedBaseClassAbilities
    {
        [HarmonyPrefix]
        static void Prefix(
            ref CharacterClass __instance,
            Il2CppSystem.Collections.Generic.List<Ability> __result
        )
        {
            if (CanRun())
            {
                foreach (var a in __instance.unlockableAbilities)
                {
                    a.level = 0;
                }
                foreach (Mastery m in __instance.masteries)
                {
                    foreach (var b in m.abilities)
                    {
                        b.level = 0;
                    }
                }
            }
        }
    }
}
