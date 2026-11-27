namespace LastEpoch_Hud.Scripts.Core.CustomItems.Mjolner;

/// <summary>Socketed-skill cooldown Mjolner uses, with its floor.</summary>
public static class MjolnerSocketedCooldown
{
    public const double MinimumMs = 250;

    public static double Milliseconds(float configuredMs)
    {
        return configuredMs >= MinimumMs ? configuredMs : MinimumMs;
    }
}
