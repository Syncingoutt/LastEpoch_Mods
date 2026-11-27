using System;

namespace LastEpoch_Hud.Scripts.Core.Character;

/// <summary>Scales reward amounts by a whole-number multiplier.</summary>
public static class RewardMultiplier
{
    public static long Scale(long amount, float multiplier)
    {
        return amount * (long)multiplier;
    }

    public static uint ScaleSaturated(uint amount, float multiplier)
    {
        return (uint)Math.Min(uint.MaxValue, (ulong)(uint)multiplier * amount);
    }
}
