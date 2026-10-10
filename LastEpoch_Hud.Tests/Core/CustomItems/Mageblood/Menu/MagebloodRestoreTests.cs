using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu;

public sealed class MagebloodRestoreTests
{
    private const string Old = "OLD";
    private const string Defaults = "DEF";

    [Fact]
    public void Run_NoFile_WritesDefaultsNoBackup()
    {
        var file = new FakeConfigText(null);
        var backup = new FakeConfigText(null);

        MagebloodRestoreResult result = MagebloodRestore.Run(file, backup, Defaults);

        Assert.Equal(MagebloodRestoreResult.DefaultsWritten, result);
        Assert.Equal(Defaults, file.Text);
        Assert.Equal(0, backup.WriteCount);
    }

    [Fact]
    public void Run_ReadNull_AbortsFileUntouched()
    {
        var file = new FakeConfigText(Old, readFails: true);
        var backup = new FakeConfigText(null);

        MagebloodRestoreResult result = MagebloodRestore.Run(file, backup, Defaults);

        Assert.Equal(MagebloodRestoreResult.ReadFailed, result);
        Assert.Equal(Old, file.Text);
        Assert.Equal(0, file.WriteCount);
        Assert.Equal(0, backup.WriteCount);
    }

    [Fact]
    public void Run_BackupFails_AbortsFileUntouched()
    {
        var file = new FakeConfigText(Old);
        var backup = new FakeConfigText(null, writeFails: true);

        MagebloodRestoreResult result = MagebloodRestore.Run(file, backup, Defaults);

        Assert.Equal(MagebloodRestoreResult.BackupFailed, result);
        Assert.Equal(Old, file.Text);
        Assert.Equal(0, file.WriteCount);
    }

    [Fact]
    public void Run_BackupOk_BackupHoldsOldTextThenDefaults()
    {
        var file = new FakeConfigText(Old);
        var backup = new FakeConfigText(null);

        MagebloodRestoreResult result = MagebloodRestore.Run(file, backup, Defaults);

        Assert.Equal(MagebloodRestoreResult.BackedUpAndWritten, result);
        Assert.Equal(Old, backup.Text);
        Assert.Equal(Defaults, file.Text);
    }

    [Fact]
    public void Run_DefaultsWriteFails_WriteFailed()
    {
        var file = new FakeConfigText(Old, writeFails: true);
        var backup = new FakeConfigText(null);

        MagebloodRestoreResult result = MagebloodRestore.Run(file, backup, Defaults);

        Assert.Equal(MagebloodRestoreResult.WriteFailed, result);
    }

    [Fact]
    public void Run_NoFileAndWriteFails_WriteFailed()
    {
        var file = new FakeConfigText(null, writeFails: true);
        var backup = new FakeConfigText(null);

        MagebloodRestoreResult result = MagebloodRestore.Run(file, backup, Defaults);

        Assert.Equal(MagebloodRestoreResult.WriteFailed, result);
    }
}
