using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Items.CraftingSlot fields mapped to the ItemsCraftingSlot group.</summary>
public static class ItemsCraftingSlotMap
{
    public const string Group = "ItemsCraftingSlot";

    private const string Prefix = "Items.CraftingSlot.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } = Build();

    private static LegacyFieldMapping[] Build()
    {
        List<LegacyFieldMapping> all = new();
        all.Add(new(Prefix + "Enable_Mod", Group, "EnableMod", LegacySettingPart.Bool));
        AddOverride(
            all,
            "ForginPotencial",
            "ForgingPotential",
            ItemSliderLimits.CraftingSlotForgingPotential
        );
        for (int n = 0; n < 3; n++)
        {
            AddOverride(
                all,
                "Implicit_" + n,
                "Implicit" + n,
                ItemSliderLimits.CraftingSlotImplicit
            );
        }
        AddOverride(all, "Seal_Tier", "SealTier", ItemSliderLimits.CraftingSlotSealTier);
        AddOverride(all, "Seal_Value", "SealValue", ItemSliderLimits.CraftingSlotSealValue);
        for (int n = 0; n < 4; n++)
        {
            AddOverride(
                all,
                "Affix_" + n + "_Tier",
                "Affix" + n + "Tier",
                ItemSliderLimits.CraftingSlotAffixTier
            );
            AddOverride(
                all,
                "Affix_" + n + "_Value",
                "Affix" + n + "Value",
                ItemSliderLimits.CraftingSlotAffixValue
            );
        }
        for (int n = 0; n < 8; n++)
        {
            AddOverride(
                all,
                "UniqueMod_" + n,
                "UniqueMod" + n,
                ItemSliderLimits.CraftingSlotUniqueMod
            );
        }
        AddOverride(
            all,
            "LegendaryPotencial",
            "LegendaryPotential",
            ItemSliderLimits.CraftingSlotLegendaryPotential
        );
        AddOverride(all, "WeaverWill", "WeaverWill", ItemSliderLimits.CraftingSlotWeaverWill);
        return all.ToArray();
    }

    private static void AddOverride(
        List<LegacyFieldMapping> all,
        string legacyName,
        string key,
        SliderRange limits
    )
    {
        all.Add(new(Prefix + "Enable_" + legacyName, Group, key, LegacySettingPart.FloatEnabled));
        all.Add(
            new(
                Prefix + legacyName,
                Group,
                key,
                LegacySettingPart.FloatValue,
                limits.Min,
                limits.Max
            )
        );
    }
}
