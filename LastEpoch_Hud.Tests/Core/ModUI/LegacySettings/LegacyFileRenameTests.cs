using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Tests.Core.ModUI.Settings;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings;

public sealed class LegacyFileRenameTests : IDisposable
{
    private const string Path = "dir/Save.json";

    private readonly TempDirectory _dir = new();

    public void Dispose()
    {
        _dir.Dispose();
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"Items\":{}}")]
    public void ShouldRename_GroupsAlreadyOnDisk_True(string text)
    {
        Assert.True(
            LegacyFileRename.ShouldRename(
                text,
                writesBlocked: false,
                groupsOnDiskBefore: true,
                savedThisRun: false
            )
        );
    }

    [Fact]
    public void ShouldRename_SavedThisRun_True()
    {
        Assert.True(
            LegacyFileRename.ShouldRename(
                "{}",
                writesBlocked: false,
                groupsOnDiskBefore: false,
                savedThisRun: true
            )
        );
    }

    [Fact]
    public void ShouldRename_MissingAndNotSaved_False()
    {
        Assert.False(
            LegacyFileRename.ShouldRename(
                "{}",
                writesBlocked: false,
                groupsOnDiskBefore: false,
                savedThisRun: false
            )
        );
    }

    [Fact]
    public void ShouldRename_WritesBlocked_False()
    {
        Assert.False(
            LegacyFileRename.ShouldRename(
                "{}",
                writesBlocked: true,
                groupsOnDiskBefore: true,
                savedThisRun: true
            )
        );
    }

    [Fact]
    public void ShouldRename_NoText_False()
    {
        Assert.False(
            LegacyFileRename.ShouldRename(
                null,
                writesBlocked: false,
                groupsOnDiskBefore: true,
                savedThisRun: true
            )
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("{")]
    [InlineData("[1]")]
    [InlineData("5")]
    [InlineData("{} {}")]
    public void ShouldRename_WholeFileFailure_False(string text)
    {
        Assert.False(
            LegacyFileRename.ShouldRename(
                text,
                writesBlocked: false,
                groupsOnDiskBefore: true,
                savedThisRun: true
            )
        );
    }

    [Fact]
    public void TryPickTarget_NoneTaken_UsesMigrated()
    {
        bool found = LegacyFileRename.TryPickTarget(Path, _ => false, out string target);

        Assert.True(found);
        Assert.Equal(Path + LegacyFileRename.Suffix, target);
    }

    [Fact]
    public void TryPickTarget_FirstTaken_UsesTwo()
    {
        string first = Path + LegacyFileRename.Suffix;

        bool found = LegacyFileRename.TryPickTarget(Path, name => name == first, out string target);

        Assert.True(found);
        Assert.Equal(first + ".2", target);
    }

    [Fact]
    public void TryPickTarget_SeveralTaken_UsesFirstFree()
    {
        string first = Path + LegacyFileRename.Suffix;
        string[] taken = { first, first + ".2", first + ".3" };

        bool found = LegacyFileRename.TryPickTarget(
            Path,
            name => taken.Contains(name),
            out string target
        );

        Assert.True(found);
        Assert.Equal(first + ".4", target);
    }

    [Fact]
    public void TryPickTarget_Gap_UsesFirstFree()
    {
        string first = Path + LegacyFileRename.Suffix;

        bool found = LegacyFileRename.TryPickTarget(
            Path,
            name => name == first + ".2",
            out string target
        );

        Assert.True(found);
        Assert.Equal(first, target);
    }

    [Fact]
    public void TryPickTarget_AllTaken_Fails()
    {
        int calls = 0;

        bool found = LegacyFileRename.TryPickTarget(
            Path,
            _ =>
            {
                calls++;
                return true;
            },
            out string target
        );

        Assert.False(found);
        Assert.Null(target);
        Assert.Equal(LegacyFileRename.MaxTargets, calls);
    }

    [Fact]
    public void TryMoveAside_NoTarget_MovesToMigrated()
    {
        string path = _dir.File("Save.json");
        System.IO.File.WriteAllText(path, "content");

        bool moved = LegacyFileRename.TryMoveAside(path, out string target, out string error);

        Assert.True(moved);
        Assert.Null(error);
        Assert.Equal(path + LegacyFileRename.Suffix, target);
        Assert.False(System.IO.File.Exists(path));
        Assert.Equal("content", System.IO.File.ReadAllText(target));
    }

    [Fact]
    public void TryMoveAside_MigratedExists_UsesTwoKeepsOld()
    {
        string path = _dir.File("Save.json");
        string old = path + LegacyFileRename.Suffix;
        System.IO.File.WriteAllText(path, "new");
        System.IO.File.WriteAllText(old, "old");

        bool moved = LegacyFileRename.TryMoveAside(path, out string target, out string error);

        Assert.True(moved);
        Assert.Null(error);
        Assert.Equal(old + ".2", target);
        Assert.Equal("new", System.IO.File.ReadAllText(target));
        Assert.Equal("old", System.IO.File.ReadAllText(old));
    }

    [Fact]
    public void TryMoveAside_SourceLocked_FailsKeepsSource()
    {
        Assert.SkipUnless(OperatingSystem.IsWindows(), "File locks block a move only on Windows.");
        string path = _dir.File("Save.json");
        System.IO.File.WriteAllText(path, "content");
        using var held = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);

        bool moved = LegacyFileRename.TryMoveAside(path, out string target, out string error);

        Assert.False(moved);
        Assert.NotNull(error);
        Assert.Null(target);
        Assert.False(System.IO.File.Exists(path + LegacyFileRename.Suffix));
        held.Dispose();
        Assert.Equal("content", System.IO.File.ReadAllText(path));
    }

    [Fact]
    public void TryMoveAside_SourceMissing_Fails()
    {
        bool moved = LegacyFileRename.TryMoveAside(
            _dir.File("Save.json"),
            out string target,
            out string error
        );

        Assert.False(moved);
        Assert.NotNull(error);
        Assert.Null(target);
    }

    [Fact]
    public void TryMoveAside_DirectoryNamedMigrated_UsesNextName()
    {
        string path = _dir.File("Save.json");
        string taken = path + LegacyFileRename.Suffix;
        System.IO.File.WriteAllText(path, "content");
        Directory.CreateDirectory(taken);

        bool moved = LegacyFileRename.TryMoveAside(path, out string target, out string error);

        Assert.True(moved);
        Assert.Null(error);
        Assert.Equal(taken + ".2", target);
        Assert.Equal("content", System.IO.File.ReadAllText(target));
    }
}
