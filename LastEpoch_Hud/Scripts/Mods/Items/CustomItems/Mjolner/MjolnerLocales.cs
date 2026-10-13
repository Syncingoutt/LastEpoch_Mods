using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mjolner;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Items.CustomItems.Mjolner;

internal static class MjolnerLocales
{
    internal static string CurrentDescription(IReadOnlyDictionary<string, string> texts)
    {
        if (ModSettings.Mjolner.ProcAnyLightningSpell.Value)
        {
            return MjolnerDescription.LightningProc(
                texts,
                (int)ModSettings.Mjolner.StrRequirement.Value,
                (int)ModSettings.Mjolner.IntRequirement.Value,
                ModSettings.Mjolner.MinTriggerChance.Value,
                ModSettings.Mjolner.MaxTriggerChance.Value
            );
        }

        return MjolnerDescription.SocketedSkills(
            texts,
            (int)ModSettings.Mjolner.StrRequirement.Value,
            (int)ModSettings.Mjolner.IntRequirement.Value,
            ModSettings.Mjolner.SocketedCooldown.Value,
            ModSettings.Mjolner.SocketedSkill0.Value,
            ModSettings.Mjolner.SocketedSkill1.Value,
            ModSettings.Mjolner.SocketedSkill2.Value
        );
    }
}
