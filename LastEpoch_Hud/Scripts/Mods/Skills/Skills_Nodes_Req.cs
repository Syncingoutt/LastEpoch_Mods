using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Skills;

internal class Skills_Nodes_Req
{
    public static bool CanRun()
    {
        return Scenes.IsGameScene() && ModSettings.SkillsGeneral.RemoveNodeRequirements.Value;
    }

    [HarmonyPatch(
        typeof(LocalTreeData),
        "fulfilledRequirementExists",
        new System.Type[]
        {
            typeof(GlobalTreeData.TreeData),
            typeof(GlobalTreeData.NodeData),
            typeof(LocalTreeData.TreeData),
            typeof(LocalTreeData.NodeData),
        }
    )]
    public class LocalTreeData_FulfilledRequirementExists
    {
        [HarmonyPrefix]
        static bool Prefix(ref bool __result)
        {
            // Bypass only the prerequisite predicate. Native spending still owns
            // the point budget and node cap; do not inflate skill point capacity.
            if (CanRun())
            {
                __result = true;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
