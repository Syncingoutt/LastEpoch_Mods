using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.ModUI.Settings;

namespace LastEpoch_Hud.Scripts.ModUI;

public static partial class ModSettings
{
#pragma warning disable CA1034 // nested like every settings section
    /// <summary>Hotkeys of features without a menu page.</summary>
    public static class KeyBinds
    {
        public static readonly SettingsGroup Group = new(KeyBindsMap.Group);

        public static readonly KeybindSetting BankStashs = Group.Keybind(
            "BankStashs",
            defaultBinding: "kb:F3"
        );

        public static readonly KeybindSetting DamageMeter = Group.Keybind(
            "DamageMeter",
            defaultBinding: "kb:F7"
        );
    }

    /// <summary>Damage meter window options.</summary>
    public static class DamageMeter
    {
        public static readonly SettingsGroup Group = new(DamageMeterMap.Group);

        public static readonly DropdownSetting DamageType = Group.Dropdown("DamageType");

        public static readonly BoolSetting SeparateHitAndDot = Group.Bool("SeparateHitAndDot");
    }
#pragma warning restore CA1034
}
