using System.Collections.Generic;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Items.Mjolner fields mapped to the Mjolner group. The legacy BaseDrop is dropped: Mjolner never read it.</summary>
public static class MjolnerMap
{
    public const string Group = "Mjolner";

    private const string Prefix = "Items.Mjolner.";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } =
        new[]
        {
            Bool("enable", "EnableMod"),
            Bool("UniqueDrop", "UniqueDrop"),
            Bool("WeaverWill", "WeaverWill"),
            Bool("ProcAnyLightningSpell", "ProcAnyLightningSpell"),
            Number("MinTriggerChance"),
            Number("MaxTriggerChance"),
            Number("IntRequirement"),
            Number("StrRequirement"),
            Number("SocketedCooldown"),
            // The legacy field name has this typo.
            Text("SockectedSkill_0", "SocketedSkill0"),
            Text("SockectedSkill_1", "SocketedSkill1"),
            Text("SockectedSkill_2", "SocketedSkill2"),
        };

    private static LegacyFieldMapping Bool(string legacyName, string key)
    {
        return new LegacyFieldMapping(Prefix + legacyName, Group, key, LegacySettingPart.Bool);
    }

    private static LegacyFieldMapping Number(string name)
    {
        return new LegacyFieldMapping(Prefix + name, Group, name, LegacySettingPart.FloatValue);
    }

    private static LegacyFieldMapping Text(string legacyName, string key)
    {
        return new LegacyFieldMapping(Prefix + legacyName, Group, key, LegacySettingPart.Text);
    }
}
