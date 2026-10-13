using System.Collections.Generic;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy KeyBinds fields mapped to the KeyBinds group.</summary>
public static class KeyBindsMap
{
    public const string Group = "KeyBinds";

    private const string Prefix = "KeyBinds.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } =
        new[] { Keybind("BankStashs"), Keybind("DamageMeter") };

    private static LegacyFieldMapping Keybind(string name)
    {
        return new LegacyFieldMapping(Prefix + name, Group, name, LegacySettingPart.Keybind);
    }
}
