using LastEpoch_Hud.Scripts.Core.ModUI.Settings;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.Settings;

public sealed class SettingsFileTests : IDisposable
{
    private readonly TempDirectory _dir = new();

    public void Dispose()
    {
        _dir.Dispose();
    }

    [Fact]
    public void TryRead_NoFile_IsMissing()
    {
        SettingsFileState state = SettingsFile.TryRead(
            _dir.File("s.json"),
            out JObject root,
            out string error
        );

        Assert.Equal(SettingsFileState.Missing, state);
        Assert.Null(root);
        Assert.Null(error);
    }

    [Theory]
    [InlineData("{bad")]
    [InlineData("[1]")]
    [InlineData("")]
    public void TryRead_NotAnObject_IsCorrupt(string content)
    {
        string path = _dir.File("s.json");
        File.WriteAllText(path, content);

        SettingsFileState state = SettingsFile.TryRead(path, out JObject root, out string error);

        Assert.Equal(SettingsFileState.Corrupt, state);
        Assert.Null(root);
        Assert.NotNull(error);
    }

    [Fact]
    public void TryRead_ValidObject_IsLoaded()
    {
        string path = _dir.File("s.json");
        File.WriteAllText(path, "{\"a\":1}");

        SettingsFileState state = SettingsFile.TryRead(path, out JObject root, out string error);

        Assert.Equal(SettingsFileState.Loaded, state);
        Assert.Equal(1, (int)root["a"]);
        Assert.Null(error);
    }

    [Fact]
    public void TryRead_FileLocked_IsUnreadable()
    {
        string path = _dir.File("s.json");
        File.WriteAllText(path, "{}");
        using var held = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);

        SettingsFileState state = SettingsFile.TryRead(path, out JObject root, out string error);

        Assert.Equal(SettingsFileState.Unreadable, state);
        Assert.Null(root);
        Assert.NotNull(error);
    }

    [Fact]
    public void WriteAtomic_NewFile_WritesAndLeavesNoTemp()
    {
        string path = _dir.File("s.json");

        bool ok = SettingsFile.WriteAtomic(path, "new", out string error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.Equal("new", File.ReadAllText(path));
        Assert.False(File.Exists(path + ".tmp"));
    }

    [Fact]
    public void WriteAtomic_ExistingFile_ReplacesAndLeavesNoTemp()
    {
        string path = _dir.File("s.json");
        File.WriteAllText(path, "old");

        bool ok = SettingsFile.WriteAtomic(path, "new", out string error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.Equal("new", File.ReadAllText(path));
        Assert.False(File.Exists(path + ".tmp"));
    }

    [Fact]
    public void WriteAtomic_StaleTempFromCrash_OverwritesAndLeavesNoTemp()
    {
        string path = _dir.File("s.json");
        File.WriteAllText(path, "old");
        File.WriteAllText(path + ".tmp", "partial");

        bool ok = SettingsFile.WriteAtomic(path, "new", out string error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.Equal("new", File.ReadAllText(path));
        Assert.False(File.Exists(path + ".tmp"));
    }

    [Fact]
    public void WriteAtomic_StaleTempWithoutTarget_OverwritesAndLeavesNoTemp()
    {
        string path = _dir.File("s.json");
        File.WriteAllText(path + ".tmp", "partial");

        bool ok = SettingsFile.WriteAtomic(path, "new", out string error);

        Assert.True(ok);
        Assert.Null(error);
        Assert.Equal("new", File.ReadAllText(path));
        Assert.False(File.Exists(path + ".tmp"));
    }

    [Fact]
    public void WriteAtomic_TargetLocked_FailsKeepsOriginalAndLeavesNoTemp()
    {
        Assert.SkipUnless(OperatingSystem.IsWindows(), "Rename ignores file locks on Unix.");
        string path = _dir.File("s.json");
        File.WriteAllText(path, "old");
        var held = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);

        bool ok = SettingsFile.WriteAtomic(path, "new", out string error);

        held.Dispose();
        Assert.False(ok);
        Assert.NotNull(error);
        Assert.Equal("old", File.ReadAllText(path));
        Assert.False(File.Exists(path + ".tmp"));
    }

    [Fact]
    public void WriteAtomic_MissingDirectory_FailsAndLeavesNoTemp()
    {
        string path = Path.Combine(_dir.File("nope"), "s.json");

        bool ok = SettingsFile.WriteAtomic(path, "new", out string error);

        Assert.False(ok);
        Assert.NotNull(error);
        Assert.False(File.Exists(path + ".tmp"));
    }
}
