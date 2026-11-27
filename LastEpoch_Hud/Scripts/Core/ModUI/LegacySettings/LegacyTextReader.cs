using System;
using System.IO;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>Reads the legacy file text, telling an absent file from a failed read.</summary>
public static class LegacyTextReader
{
    /// <summary>Text is set only when the result is Read; error only when Failed.</summary>
    public static LegacyTextState TryRead(string path, out string text, out string error)
    {
        text = null;
        error = null;
        if (!File.Exists(path))
        {
            return LegacyTextState.Absent;
        }
        try
        {
            text = File.ReadAllText(path);
            return LegacyTextState.Read;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            error = ex.Message;
            return LegacyTextState.Failed;
        }
    }
}
