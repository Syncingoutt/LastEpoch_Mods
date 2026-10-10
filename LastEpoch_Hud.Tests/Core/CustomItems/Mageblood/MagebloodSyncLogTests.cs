using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood;

public sealed class MagebloodSyncLogTests
{
    [Theory]
    [InlineData(MagebloodSyncReason.None)]
    [InlineData(MagebloodSyncReason.Dirty)]
    [InlineData(MagebloodSyncReason.WornChanged)]
    [InlineData(MagebloodSyncReason.BuffsLost)]
    public void Format_HasReasonSceneAndCount(MagebloodSyncReason reason)
    {
        string line = MagebloodSyncLog.Format(reason, "FakeScene", 3);

        Assert.Equal($"Mageblood sync: reason={reason} scene=FakeScene n=3", line);
    }

    [Fact]
    public void Format_NullScene_EmptyScene()
    {
        string line = MagebloodSyncLog.Format(MagebloodSyncReason.Dirty, null, 2);

        Assert.Equal("Mageblood sync: reason=Dirty scene= n=2", line);
    }
}
