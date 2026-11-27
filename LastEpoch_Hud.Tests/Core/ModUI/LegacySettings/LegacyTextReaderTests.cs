using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Tests.Core.ModUI.Settings;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings;

public sealed class LegacyTextReaderTests : IDisposable
{
    private readonly TempDirectory _dir = new();

    public void Dispose()
    {
        _dir.Dispose();
    }

    [Fact]
    public void TryRead_NoFile_IsAbsent()
    {
        LegacyTextState state = LegacyTextReader.TryRead(
            _dir.File("Save.json"),
            out string text,
            out string error
        );

        Assert.Equal(LegacyTextState.Absent, state);
        Assert.Null(text);
        Assert.Null(error);
    }

    [Fact]
    public void TryRead_ExistingFile_ReturnsText()
    {
        string path = _dir.File("Save.json");
        System.IO.File.WriteAllText(path, "x");

        LegacyTextState state = LegacyTextReader.TryRead(path, out string text, out string error);

        Assert.Equal(LegacyTextState.Read, state);
        Assert.Equal("x", text);
        Assert.Null(error);
    }

    [Fact]
    public void TryRead_FileLocked_Fails()
    {
        string path = _dir.File("Save.json");
        System.IO.File.WriteAllText(path, "x");
        using var held = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);

        LegacyTextState state = LegacyTextReader.TryRead(path, out string text, out string error);

        Assert.Equal(LegacyTextState.Failed, state);
        Assert.Null(text);
        Assert.NotNull(error);
    }
}
