using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Items.Drop fields mapped to the ItemsDrop group.</summary>
public static class ItemsDropMap
{
    public const string Group = "ItemsDrop";

    private const string Prefix = "Items.Drop.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } = Build();

    private static LegacyFieldMapping[] Build()
    {
        List<LegacyFieldMapping> all = new();
        AddBool(all, "ForceUnique");
        AddBool(all, "ForceSet");
        AddBool(all, "ForceLegendary");
        AddBool(all, "ForceSeal");
        AddRange(all, "Implicits", "Implicits", ItemSliderLimits.DropImplicits);
        AddRange(all, "ForginPotencial", "ForgingPotential", ItemSliderLimits.DropForgingPotential);
        AddRange(all, "SealTier", "SealTier", ItemSliderLimits.DropSealTier);
        AddRange(all, "SealValue", "SealValue", ItemSliderLimits.DropSealValue);
        AddRange(all, "AffixCount", "AffixCount", ItemSliderLimits.DropAffixCount);
        AddRange(all, "IdolAffixCount", "IdolAffixCount", ItemSliderLimits.DropIdolAffixCount);
        AddRange(all, "AffixTiers", "AffixTiers", ItemSliderLimits.DropAffixTiers);
        AddRange(all, "AffixValues", "AffixValues", ItemSliderLimits.DropAffixValues);
        AddRange(all, "UniqueMods", "UniqueMods", ItemSliderLimits.DropUniqueMods);
        AddRange(
            all,
            "LegendaryPotencial",
            "LegendaryPotential",
            ItemSliderLimits.DropLegendaryPotential
        );
        AddRange(all, "WeaverWill", "WeaverWill", ItemSliderLimits.DropWeaverWill);
        return all.ToArray();
    }

    private static void AddBool(List<LegacyFieldMapping> all, string name)
    {
        all.Add(new(Prefix + "Enable_" + name, Group, name, LegacySettingPart.Bool));
    }

    private static void AddRange(
        List<LegacyFieldMapping> all,
        string legacyName,
        string key,
        SliderRange limits
    )
    {
        all.Add(new(Prefix + "Enable_" + legacyName, Group, key, LegacySettingPart.RangeEnabled));
        all.Add(
            new(
                Prefix + legacyName + "_Min",
                Group,
                key,
                LegacySettingPart.RangeMin,
                limits.Min,
                limits.Max
            )
        );
        all.Add(
            new(
                Prefix + legacyName + "_Max",
                Group,
                key,
                LegacySettingPart.RangeMax,
                limits.Min,
                limits.Max
            )
        );
    }
}
