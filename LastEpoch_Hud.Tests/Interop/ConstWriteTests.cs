using LastEpoch_Hud.Tests.Support;
using Mono.Cecil;

namespace LastEpoch_Hud.Tests.Interop;

/// <summary>Writing a game const through Il2CppInterop corrupts memory and crashes the game.</summary>
public sealed class ConstWriteTests
{
    private static readonly Lazy<ConstWriteScanner> _scanner = new(() =>
        new ConstWriteScanner(GameEnvironment.Il2CppDir, GameEnvironment.Cpp2IlOutDir)
    );

    [Fact]
    public void Mod_NeverWritesGameConstants()
    {
        ConstWriteScanner scanner = Scanner();
        ModuleDefinition mod = GameEnvironment.ReadGameModule(GameEnvironment.ModDll);

        IReadOnlyList<ConstWrite> writes = scanner.Scan(mod);

        Assert.True(
            writes.Count == 0,
            "Mod writes game constants:\n"
                + string.Join("\n", writes.Select(w => $"{w.Caller} -> {w.Setter}"))
        );
    }

    // Guards the name mapping: the test above can't pass by matching nothing.
    [Theory]
    [InlineData("Il2Cpp.ProtectionClass", "set_resistanceCap")]
    [InlineData("Il2Cpp.AutoPool/PrefabInstancePool", "set_HARD_MAX")]
    public void Scanner_FlagsConstSetter(string type, string setter)
    {
        Assert.True(Scanner().IsConstSetter(LoadSetter("Il2CppLE.dll", type, setter)));
    }

    [Theory]
    [InlineData("Il2CppLE.dll", "Il2Cpp.CharacterMutator", "set_global2hWeaponWithShieldBaseTypes")]
    [InlineData("UnityEngine.CoreModule.dll", "UnityEngine.Application", "set_targetFrameRate")]
    public void Scanner_IgnoresNonConstStaticSetter(string assemblyFile, string type, string setter)
    {
        Assert.False(Scanner().IsConstSetter(LoadSetter(assemblyFile, type, setter)));
    }

    private static MethodDefinition LoadSetter(string assemblyFile, string type, string setter)
    {
        Scanner();
        ModuleDefinition game = GameEnvironment.ReadGameModule(
            Path.Combine(GameEnvironment.Il2CppDir, assemblyFile)
        );
        return game.GetType(type).Methods.Single(m => m.Name == setter);
    }

    private static ConstWriteScanner Scanner()
    {
        GameEnvironment.SkipWithoutGame();
        GameEnvironment.SkipWithoutModBuild();
        GameEnvironment.SkipWithoutCpp2IlOut();
        return _scanner.Value;
    }
}
