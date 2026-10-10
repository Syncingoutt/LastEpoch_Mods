using System.Globalization;

namespace LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;

/// <summary>Builds the Debug log lines written by the Mageblood sync.</summary>
public static class MagebloodSyncLog
{
    public static string Format(MagebloodSyncReason reason, string scene, int activeFlasks)
    {
        return $"Mageblood sync: reason={reason} scene={scene ?? string.Empty} n={activeFlasks}";
    }

    public static string Cap(float from, float to)
    {
        return $"Mageblood max res: cap {Number(from)} -> {Number(to)}";
    }

    private static string Number(float value)
    {
        return value.ToString("0.##", CultureInfo.InvariantCulture);
    }
}
