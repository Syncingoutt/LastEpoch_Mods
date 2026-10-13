using System;

namespace LastEpoch_Hud.Scripts.Core.Items;

/// <summary>Tier rules of the crafting-slot override.</summary>
public static class CraftingSlotTier
{
    public const int MaxTier = 6;

    private const int PrimordialFrom = 7;

    public static byte ToAffixTier(float value)
    {
        double rounded = Math.Round(value, MidpointRounding.ToEven);
        return (byte)Math.Clamp(rounded, 0d, MaxTier);
    }

    public static bool IsPrimordial(int tier) => tier >= PrimordialFrom;
}
