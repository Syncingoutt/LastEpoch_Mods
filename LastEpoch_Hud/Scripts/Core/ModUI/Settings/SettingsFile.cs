using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Scripts.Core.ModUI.Settings;

/// <summary>Reads and writes the settings file without throwing.</summary>
public static class SettingsFile
{
    /// <summary>Root is set only when the result is Loaded; error only when Unreadable or Corrupt.</summary>
    public static SettingsFileState TryRead(string path, out JObject root, out string error)
    {
        root = null;
        error = null;
        if (!File.Exists(path))
        {
            return SettingsFileState.Missing;
        }
        string text;
        try
        {
            text = File.ReadAllText(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            error = ex.Message;
            return SettingsFileState.Unreadable;
        }
        return TryParse(text, out root, out error)
            ? SettingsFileState.Loaded
            : SettingsFileState.Corrupt;
    }

    /// <summary>Writes a temp file next to the target, then swaps it in. False on IO failure, with the reason in error.</summary>
    public static bool WriteAtomic(string path, string text, out string error)
    {
        error = null;
        string temp = path + ".tmp";
        try
        {
            File.WriteAllText(temp, text);
            Swap(temp, path);
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            error = ex.Message;
            DeleteQuietly(temp);
            return false;
        }
    }

    private static bool TryParse(string text, out JObject root, out string error)
    {
        try
        {
            root = JObject.Parse(text);
            error = null;
            return true;
        }
        catch (JsonReaderException ex)
        {
            root = null;
            error = ex.Message;
            return false;
        }
    }

    private static void Swap(string temp, string path)
    {
        if (File.Exists(path))
        {
            File.Replace(temp, path, null);
            return;
        }
        File.Move(temp, path);
    }

    private static void DeleteQuietly(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Safe to ignore: the write failure is already reported, and the next write overwrites the temp file.
        }
    }
}
