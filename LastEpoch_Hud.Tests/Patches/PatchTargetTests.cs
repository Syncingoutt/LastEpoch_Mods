using System.Text.RegularExpressions;
using LastEpoch_Hud.Tests.Support;
using Mono.Cecil;

namespace LastEpoch_Hud.Tests.Patches;

/// <summary>Every Harmony patch must hit a method the current game declares.</summary>
public sealed partial class PatchTargetTests
{
    private static readonly Lazy<IReadOnlyList<PatchDeclaration>> _cache = new(() =>
        PatchScanner.Scan(GameEnvironment.ReadGameModule(GameEnvironment.ModDll))
    );

    [Fact]
    public void Patches_ResolveToOneGameMethod()
    {
        var problems = Problems().Where(p => !KnownIssues.Contains(p.Patch.Id)).ToList();

        Assert.True(problems.Count == 0, Report("Broken patch targets", problems));
    }

    [Fact]
    public void KnownBrokenPatches_AreStillBroken()
    {
        var broken = Problems().Select(p => p.Patch.Id).ToHashSet();
        var fixedOnes = KnownIssues.WithPrefix("patch:").Where(id => !broken.Contains(id)).ToList();

        Assert.True(
            fixedOnes.Count == 0,
            "Fixed or gone; remove from KnownIssues.txt:\n" + string.Join("\n", fixedOnes)
        );
    }

    // Guards the scanner: a patch form it can't read would silently go unchecked.
    [Fact]
    public void Scanner_FindsEveryPatchInSource()
    {
        int inSource = Directory
            .EnumerateFiles(GameEnvironment.ModProjectDir, "*.*", SearchOption.AllDirectories)
            .Where(path => path.EndsWith(".cs") || path.EndsWith("Items_Temporalis.txt"))
            .Where(path =>
                !path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}")
            )
            .Select(path => BlockComment().Replace(File.ReadAllText(path), ""))
            .Sum(code => PatchAttribute().Count(code));

        Assert.Equal(inSource, Load().Count);
    }

    [GeneratedRegex(@"^[ \t]*\[HarmonyPatch\(\s*typeof", RegexOptions.Multiline)]
    private static partial Regex PatchAttribute();

    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    private static partial Regex BlockComment();

    private static List<(PatchDeclaration Patch, string Problem)> Problems() =>
        Load()
            .Select(p => (Patch: p, Problem: PatchTargetResolver.Problem(p)))
            .Where(p => p.Problem != null)
            .Select(p => (p.Patch, p.Problem))
            .ToList();

    private static IReadOnlyList<PatchDeclaration> Load()
    {
        GameEnvironment.SkipWithoutGame();
        GameEnvironment.SkipWithoutModBuild();
        return _cache.Value;
    }

    private static string Report(
        string title,
        IEnumerable<(PatchDeclaration Patch, string Problem)> problems
    ) =>
        $"{title} (fix, or add the id to KnownIssues.txt only for bugs you are not fixing now):\n"
        + string.Join("\n", problems.Select(p => $"{p.Patch.Id}: {p.Problem}"));
}
