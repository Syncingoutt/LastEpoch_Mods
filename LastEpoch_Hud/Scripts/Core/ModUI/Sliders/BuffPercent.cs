using System;

namespace LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

/// <summary>Converts a stored buff value to the page percent and back.</summary>
public static class BuffPercent
{
    public static float ToPercent(BuffPercentScale scale, float stored)
    {
        return scale switch
        {
            BuffPercentScale.Fraction => stored * 100f,
            BuffPercentScale.CriticalChance => stored > 0f ? (stored * 100f) + 1f : 0f,
            _ => stored,
        };
    }

    public static float ToStored(
        BuffPercentScale scale,
        float percent,
        float minLimit,
        float maxLimit
    )
    {
        float stored = scale switch
        {
            BuffPercentScale.Fraction => percent / 100f,
            BuffPercentScale.CriticalChance => (percent - 1f) / 100f,
            _ => percent,
        };
        return Math.Min(Math.Max(stored, minLimit), maxLimit);
    }
}
