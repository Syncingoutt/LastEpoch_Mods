using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Factions.TheWoven fields mapped to the FactionsWoven group.</summary>
public static class FactionsWovenMap
{
    public const string Group = "FactionsWoven";

    private const string Prefix = "Factions.TheWoven.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } = Build();

    private static LegacyFieldMapping[] Build()
    {
        List<LegacyFieldMapping> all = new();
        AddFloat(all, "TreePoints", CharacterSliderLimits.WeaverTreePoints);
        AddFloat(all, "PointMultiplier", CharacterSliderLimits.WeaverPointMultiplier);
        AddBool(all, "Enable_FreeRespe", "FreeRespec");
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
