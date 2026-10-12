using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>All legacy field mappings, added one area at a time.</summary>
public static class LegacySettingsMap
{
    public static IReadOnlyList<LegacyFieldMapping> All { get; } =
        Concat(
            ItemsRequirementsMap.All,
            PermanentBuffsMap.All,
            ItemsDropMap.All,
            ItemsPickupMap.All,
            ItemsCraftingSlotMap.All,
            CharacterCheatsMap.All,
            CharacterMultipliersMap.All,
            SkillsGeneralMap.All,
            SkillsMovementMap.All,
            SkillsCompanionsMap.All,
            SkillsMinionsMap.All,
            SummonMap.All
        );

    private static LegacyFieldMapping[] Concat(params IReadOnlyList<LegacyFieldMapping>[] maps)
    {
        List<LegacyFieldMapping> joined = new();
        foreach (IReadOnlyList<LegacyFieldMapping> map in maps)
        {
            joined.AddRange(map);
        }
        return joined.ToArray();
    }
}
