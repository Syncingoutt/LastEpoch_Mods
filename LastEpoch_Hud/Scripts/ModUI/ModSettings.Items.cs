using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using LastEpoch_Hud.Scripts.Mods.Items.Requirements;
using LastEpoch_Hud.Scripts.ModUI.Settings;

namespace LastEpoch_Hud.Scripts.ModUI;

public static partial class ModSettings
{
#pragma warning disable CA1034 // nested like every settings section
    /// <summary>Remove item requirements (class, level, set). Any change re-applies them.</summary>
    public static class ItemsRequirements
    {
        public static readonly SettingsGroup Group = new(ItemsRequirementsMap.Group);

        public static readonly BoolSetting Level = Group.Bool("Level");
        public static readonly BoolSetting Class = Group.Bool("Class");
        public static readonly BoolSetting Set = Group.Bool("Set");

        static ItemsRequirements()
        {
            Level.Changed += OnChanged;
            Class.Changed += OnChanged;
            Set.Changed += OnChanged;
        }

        private static void OnChanged(bool _)
        {
            Items_Update.Reqs();
        }
    }
#pragma warning restore CA1034
}
