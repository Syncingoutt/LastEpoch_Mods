using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Character.Cheats cheat toggles mapped to the CharacterCheats group.</summary>
public static class CharacterCheatsMap
{
    public const string Group = "CharacterCheats";

    private const string Prefix = "Character.Cheats.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } =
        new LegacyFieldMapping[]
        {
            new(Prefix + "Enable_GodMode", Group, "GodMode", LegacySettingPart.Bool),
            new(Prefix + "Enable_LowLife", Group, "LowLife", LegacySettingPart.Bool),
            new(Prefix + "Enable_AutoPot", Group, "AutoPot", LegacySettingPart.FloatEnabled),
            new(
                Prefix + "autoPot",
                Group,
                "AutoPot",
                LegacySettingPart.FloatValue,
                CharacterSliderLimits.AutoPotion.Min,
                CharacterSliderLimits.AutoPotion.Max
            ),
            new(
                Prefix + "Enable_UnlockAllIdolsSlots",
                Group,
                "UnlockAllIdolsSlots",
                LegacySettingPart.Bool
            ),
            new(
                Prefix + "Enable_WaypointsUnlock",
                Group,
                "WaypointsUnlock",
                LegacySettingPart.Bool
            ),
            new(
                Prefix + "Enable_TwoHandedWithShield",
                Group,
                "TwoHandedWithShield",
                LegacySettingPart.Bool
            ),
        };
}
