using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Skills root fields mapped to the SkillsGeneral group.</summary>
public static class SkillsGeneralMap
{
    public const string Group = "SkillsGeneral";

    private const string Prefix = "Skills.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } = Build();

    private static LegacyFieldMapping[] Build()
    {
        List<LegacyFieldMapping> all = new();
        AddBool(all, "Enable_RemoveManaCost", "RemoveManaCost");
        AddBool(all, "Enable_RemoveChannelCost", "RemoveChannelCost");
        AddBool(all, "Enable_NoManaRegenWhileChanneling", "ManaRegenWhileChanneling");
        AddBool(all, "Enable_StopWhenOutOfMana", "ContinueWhenOutOfMana");
        AddBool(all, "Enable_RemoveCooldown", "RemoveCooldown");
        AddBool(all, "Enable_AllSkills", "UnlockAllSkills");
        AddBool(all, "Disable_NodeRequirement", "RemoveNodeRequirements");
        AddFloat(all, "SpecializationSlots", SkillSliderLimits.SpecializationSlots);
        AddFloat(all, "SkillLevel", SkillSliderLimits.SkillLevel);
        AddFloat(all, "SkillLevelMultiplier", SkillSliderLimits.PointMultiplier);
        AddFloat(all, "PassivePoints", SkillSliderLimits.PassivePoints);
        AddFloat(all, "PassivePointMultiplier", SkillSliderLimits.PointMultiplier);
        return all.ToArray();
    }

    private static void AddBool(List<LegacyFieldMapping> all, string legacyName, string key)
    {
        all.Add(new LegacyFieldMapping(Prefix + legacyName, Group, key, LegacySettingPart.Bool));
    }

    private static void AddFloat(List<LegacyFieldMapping> all, string name, SliderRange limits)
    {
        all.Add(
            new LegacyFieldMapping(
                Prefix + "Enable_" + name,
                Group,
                name,
                LegacySettingPart.FloatEnabled
            )
        );
        all.Add(
            new LegacyFieldMapping(
                Prefix + name,
                Group,
                name,
                LegacySettingPart.FloatValue,
                limits.Min,
                limits.Max
            )
        );
    }
}
