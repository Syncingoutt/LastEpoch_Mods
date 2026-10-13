using System;

namespace LastEpoch_Hud.Scripts.Core.ModUI;

/// <summary>Key name of a single-key keyboard binding.</summary>
public static class KeybindKeyName
{
    private const string KeyboardPrefix = "kb:";

    public static string Single(string binding)
    {
        if (string.IsNullOrEmpty(binding))
        {
            return null;
        }
        if (!binding.StartsWith(KeyboardPrefix, StringComparison.Ordinal))
        {
            return null;
        }
        if (binding.Length == KeyboardPrefix.Length)
        {
            return null;
        }
        if (binding.IndexOf('+') >= 0)
        {
            return null;
        }
        return binding.Substring(KeyboardPrefix.Length);
    }
}
