using Mono.Cecil;

namespace LastEpoch_Hud.Tests.Support;

/// <summary>Paths to the repo, the built mod and the installed game.</summary>
internal static class GameEnvironment
{
    private const string DefaultGameDir =
        @"C:\Program Files (x86)\Steam\steamapps\common\Last Epoch";

    public static string RepoRoot { get; } = FindRepoRoot();

    public static string GameDir =>
        Environment.GetEnvironmentVariable("LAST_EPOCH_PATH") ?? DefaultGameDir;

    public static string Il2CppDir => Path.Combine(GameDir, "MelonLoader", "Il2CppAssemblies");

    public static string Cpp2IlOutDir =>
        Path.Combine(
            GameDir,
            "MelonLoader",
            "Dependencies",
            "Il2CppAssemblyGenerator",
            "Cpp2IL",
            "cpp2il_out"
        );

    public static string ModProjectDir => Path.Combine(RepoRoot, "LastEpoch_Hud");

    public static string ModDll =>
        Environment.GetEnvironmentVariable("LAST_EPOCH_MOD_DLL")
        ?? Path.Combine(RepoRoot, "Build", "Keyboard", "net6.0", "LastEpoch_Hud.dll");

    public static void SkipWithoutGame() =>
        Assert.SkipUnless(
            Directory.Exists(Il2CppDir),
            $"No Il2Cpp assemblies at {Il2CppDir}; set LAST_EPOCH_PATH"
        );

    public static void SkipWithoutModBuild() =>
        Assert.SkipUnless(
            File.Exists(ModDll),
            $"No mod build at {ModDll}; build the mod or set LAST_EPOCH_MOD_DLL"
        );

    public static void SkipWithoutCpp2IlOut() =>
        Assert.SkipUnless(
            Directory.Exists(Cpp2IlOutDir),
            $"No Cpp2IL output at {Cpp2IlOutDir}; set LAST_EPOCH_PATH"
        );

    public static ModuleDefinition ReadGameModule(string path)
    {
        var resolver = new DefaultAssemblyResolver();
        resolver.AddSearchDirectory(Il2CppDir);
        resolver.AddSearchDirectory(Path.Combine(GameDir, "MelonLoader", "net6"));
        return ModuleDefinition.ReadModule(
            path,
            new ReaderParameters { AssemblyResolver = resolver }
        );
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "LastEpoch_Hud.sln")))
        {
            dir = dir.Parent;
        }
        return dir?.FullName
            ?? throw new InvalidOperationException(
                "LastEpoch_Hud.sln not found above the test output"
            );
    }
}
