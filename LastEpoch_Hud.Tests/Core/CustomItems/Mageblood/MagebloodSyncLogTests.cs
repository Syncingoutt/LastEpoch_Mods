using System.Globalization;
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
    public void Cap_HasFromAndTo()
    {
        Assert.Equal("Mageblood max res: cap 75 -> 80", MagebloodSyncLog.Cap(75f, 80f));
    }

    [Fact]
    public void Cap_CommaCulture_UsesDot()
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("de-DE");
        try
        {
            Assert.Equal("Mageblood max res: cap 0.75 -> 0.8", MagebloodSyncLog.Cap(0.75f, 0.8f));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void Format_NullScene_EmptyScene()
    {
        string line = MagebloodSyncLog.Format(MagebloodSyncReason.Dirty, null, 2);

        Assert.Equal("Mageblood sync: reason=Dirty scene= n=2", line);
    }
}
