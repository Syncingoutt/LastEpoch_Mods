using HarmonyLib;
using Il2Cpp;

namespace LastEpoch_Hud.Scripts.Mods.Items.CustomItems.Mageblood;

/// <summary>Raises the shared cap while the game resists a hit on the local player.</summary>
[HarmonyPatch(typeof(ProtectionClass), "ApplyResistance")]
public class MagebloodResistancePatch
{
    [HarmonyPrefix]
    private static void Prefix(ProtectionClass __instance, out bool __state)
    {
        __state = MagebloodCapSync.Enter(__instance);
    }

    [HarmonyFinalizer]
    private static void Finalizer(bool __state)
    {
        MagebloodCapSync.Exit(__state);
    }
}
