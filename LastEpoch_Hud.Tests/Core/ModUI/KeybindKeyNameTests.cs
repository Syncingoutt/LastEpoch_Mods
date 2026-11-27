using LastEpoch_Hud.Scripts.Core.ModUI;

namespace LastEpoch_Hud.Tests.Core.ModUI;

public sealed class KeybindKeyNameTests
{
    [Theory]
    [InlineData("kb:F7", "F7")]
    [InlineData("kb:LeftControl", "LeftControl")]
    public void Single_ReturnsKeyName(string binding, string expected)
    {
        Assert.Equal(expected, KeybindKeyName.Single(binding));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("kb:")]
    [InlineData("gp:A")]
    [InlineData("F7")]
    [InlineData("kb:LeftControl+F7")]
    public void Single_ReturnsNull(string binding)
    {
        Assert.Null(KeybindKeyName.Single(binding));
    }
}
