namespace LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

/// <summary>Maps a "0 = off" slider to a setting's Enabled and Value.</summary>
public static class OffAtZeroSlider
{
    public const float OffThreshold = 0.0001f;

    public static bool IsOn(float position)
    {
        return position > OffThreshold;
    }

    public static float Position(bool enabled, float value)
    {
        return enabled ? value : 0f;
    }
}
