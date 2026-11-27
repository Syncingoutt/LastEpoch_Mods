using System.IO;

namespace LastEpoch_Hud.Scripts;

/// <summary>The mod's data folder under the game's Mods directory.</summary>
public static class ModPaths
{
    public static string Folder { get; } =
        Path.Combine(Directory.GetCurrentDirectory(), "Mods", Main.mod_name);

    public static string FilePath(string fileName)
    {
        return Path.Combine(Folder, fileName);
    }
}
