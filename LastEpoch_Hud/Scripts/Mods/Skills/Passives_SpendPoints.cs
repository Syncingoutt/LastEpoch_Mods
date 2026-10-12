using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Skills;

public class Passives_SpendPoints
{
    public static bool CanRun()
    {
        return Scenes.IsGameScene();
    }

    [HarmonyPatch(typeof(LocalTreeData), "tryToSpendPassivePoint")]
    public class LocalTreeData_TryToSpendPassivePoint
    {
        private static bool Added_Level = false;
        private static Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<byte> backup_masteries_level =
            new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<byte>(4);

        [HarmonyPrefix]
        static void Prefix(
            LocalTreeData __instance,
            ref bool __result,
            CharacterClass __0,
            byte __1
        )
        {
            //Main.logger_instance?.Msg("LocalTreeData.tryToSpendPassivePoint() Prefix");
            Added_Level = false;
            if (CanRun())
            {
                backup_masteries_level = __instance.masteryLevels;
                if ((!__result) && (ModSettings.SkillsGeneral.RemoveNodeRequirements.Value))
                {
                    Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<byte> results =
                        new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<byte>(4);
                    for (int i = 0; i < results.Length; i++)
                    {
                        results[i] = 255;
                    }
                    __instance.masteryLevels = results;
                    Added_Level = true;
                }
            }
        }

        [HarmonyPostfix]
        static void Postfix(
            ref LocalTreeData __instance,
            ref bool __result,
            CharacterClass __0,
            byte __1
        )
        {
            //Main.logger_instance?.Msg("LocalTreeData.tryToSpendPassivePoint() Postfix");
            if (CanRun())
            {
                if (Added_Level)
                {
                    __instance.masteryLevels = backup_masteries_level;
                }
            }
        }
    }
}
