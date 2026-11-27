using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Skills.Companion fields mapped to the SkillsCompanions group.</summary>
public static class SkillsCompanionsMap
{
    public const string Group = "SkillsCompanions";

    private const string Prefix = "Skills.Companion.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } = Build();

    private static LegacyFieldMapping[] Build()
    {
        List<LegacyFieldMapping> all = new();
        AddBool(all, "Wolf.Enable_SummonMax", "WolfSummonMaximum");
        AddBool(all, "Wolf.Enable_StunImmunity", "WolfStunImmunity");
        AddFloat(
            all,
            "Enable_Limit",
            "Limit",
            "MaximumCompanions",
            SkillSliderLimits.CompanionsMaximum
        );
        AddFloat(
            all,
            "Wolf.Enable_SummonLimit",
            "Wolf.SummonLimit",
            "WolfSummonLimit",
            SkillSliderLimits.WolfSummonLimit
        );
        AddFloat(
            all,
            "Scorpion.Enable_BabyQuantity",
            "Scorpion.BabyQuantity",
            "ScorpionBabyQuantity",
            SkillSliderLimits.ScorpionBabyQuantity
        );
        return all.ToArray();
    }

    private static void AddBool(List<LegacyFieldMapping> all, string legacyPath, string key)
    {
        all.Add(new LegacyFieldMapping(Prefix + legacyPath, Group, key, LegacySettingPart.Bool));
    }

    private static void AddFloat(
        List<LegacyFieldMapping> all,
        string enabledPath,
        string valuePath,
        string key,
        SliderRange limits
    )
    {
        all.Add(
            new LegacyFieldMapping(Prefix + enabledPath, Group, key, LegacySettingPart.FloatEnabled)
        );
        all.Add(
            new LegacyFieldMapping(
                Prefix + valuePath,
                Group,
                key,
                LegacySettingPart.FloatValue,
                limits.Min,
                limits.Max
            )
        );
    }
}
