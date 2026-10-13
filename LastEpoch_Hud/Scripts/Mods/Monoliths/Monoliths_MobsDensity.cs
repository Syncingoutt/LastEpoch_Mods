using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Monoliths;

public class Monoliths_MobsDensity
{
    public static bool CanRun() =>
        Scenes.IsGameScene() && ModSettings.ScenesMonoliths.MobsDensity.Enabled;

    [HarmonyPatch(typeof(MonolithRun), "calculateIncreasedRarityAndExperienceFromMods")]
    public class MonolithRun_calculateIncreasedRarityAndExperienceFromMods
    {
        [HarmonyPostfix]
        static void Postfix(ref MonolithRun __instance)
        {
            if (!CanRun())
            {
                return;
            }
            __instance.timeline.enemyDensityModifier = ModSettings
                .ScenesMonoliths
                .MobsDensity
                .Value;
        }
    }
}
