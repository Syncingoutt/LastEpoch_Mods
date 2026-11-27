using System;
using System.IO;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

/// <summary>Decides when the legacy Save.json is done with and which name it moves to.</summary>
public static class LegacyFileRename
{
    public const string Suffix = ".migrated";
    public const int MaxTargets = 100;

    public static bool ShouldRename(
        string legacyText,
        bool writesBlocked,
        bool groupsOnDiskBefore,
        bool savedThisRun
    )
    {
        if (legacyText == null)
        {
            return false;
        }
        if (writesBlocked)
        {
            return false;
        }
        if (!groupsOnDiskBefore && !savedThisRun)
        {
            return false;
        }
        return LegacySettingsImporter.IsJsonObject(legacyText);
    }

    public static bool TryPickTarget(
        string legacyPath,
        Func<string, bool> exists,
        out string target
    )
    {
        for (int i = 1; i <= MaxTargets; i++)
        {
            string candidate = i == 1 ? legacyPath + Suffix : legacyPath + Suffix + "." + i;
            if (!exists(candidate))
            {
                target = candidate;
                return true;
            }
        }
        target = null;
        return false;
    }

    /// <summary>Moves the file to the first free migrated name without overwriting. Never throws; the source stays on failure.</summary>
    public static bool TryMoveAside(string path, out string target, out string error)
    {
        if (!TryPickTarget(path, NameTaken, out string picked))
        {
            return Fail("No free name to move Save.json aside.", out target, out error);
        }
        try
        {
            File.Move(path, picked);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return Fail(ex.Message, out target, out error);
        }
        target = picked;
        error = null;
        return true;
    }

    private static bool NameTaken(string name)
    {
        return File.Exists(name) || Directory.Exists(name);
    }

    private static bool Fail(string message, out string target, out string error)
    {
        target = null;
        error = message;
        return false;
    }
}
