using System;

namespace LastEpoch_Hud.Scripts.Core.Items;

/// <summary>Turns a saved affix-count float into the whole number the slider shows.</summary>
public static class DropAffixCount
{
    private const int GameMin = 0;
    private const int GameMax = 4;
    private const int HardMax = 6;

    public static int ToWhole(float value) => (int)Math.Round(value);

    /// <summary>Rounds and clamps the range to 0..6; Overrides is decided before clamping.</summary>
    public static AffixCountBounds Bounds(float min, float max)
    {
        int minWhole = ToWhole(min);
        int maxWhole = ToWhole(max);
        bool overrides = !(minWhole == GameMin && maxWhole == GameMax);
        return new AffixCountBounds(
            Math.Clamp(minWhole, GameMin, HardMax),
            Math.Clamp(maxWhole, GameMin, HardMax),
            overrides
        );
    }
}
