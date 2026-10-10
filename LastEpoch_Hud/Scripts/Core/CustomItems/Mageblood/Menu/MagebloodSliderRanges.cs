using System;

namespace LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;

/// <summary>Slider bounds per editable value; the max stretches when the file holds more.</summary>
public static class MagebloodSliderRanges
{
    public const float MaxResistancesMax = 20f;

    public static MagebloodSliderRange ForMaxResistances(float value)
    {
        return new MagebloodSliderRange(0f, Stretch(MaxResistancesMax, value), IsWhole(value));
    }

    private static float Stretch(float max, float value)
    {
        return float.IsFinite(value) && value > max ? MathF.Ceiling(value) : max;
    }

    private static bool IsWhole(float value)
    {
        return !float.IsFinite(value) || MathF.Abs(value - MathF.Round(value)) < 0.001f;
    }
}
