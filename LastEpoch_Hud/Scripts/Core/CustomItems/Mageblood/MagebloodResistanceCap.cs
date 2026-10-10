using System;

namespace LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;

/// <summary>Max resistance cap math, in the cap's own units.</summary>
public static class MagebloodResistanceCap
{
    public const float PercentCeiling = 100f;
    public const float FractionCeiling = 1f;

    /// <summary>The cap raised by the bonus percent points, in percent or fraction units by the base.</summary>
    public static float Raise(float baseCap, float bonusPercent)
    {
        if (!float.IsFinite(baseCap) || baseCap <= 0f)
        {
            return baseCap;
        }
        if (!float.IsFinite(bonusPercent) || bonusPercent <= 0f)
        {
            return baseCap;
        }
        if (baseCap > 1f)
        {
            return Math.Max(baseCap, Math.Min(baseCap + bonusPercent, PercentCeiling));
        }
        return Math.Max(baseCap, Math.Min(baseCap + (bonusPercent / 100f), FractionCeiling));
    }
}
