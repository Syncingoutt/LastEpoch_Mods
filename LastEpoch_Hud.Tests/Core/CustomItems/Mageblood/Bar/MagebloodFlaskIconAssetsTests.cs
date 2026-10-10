using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Bar;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Bar;

public sealed class MagebloodFlaskIconAssetsTests
{
    [Theory]
    [InlineData("assets/mageblood/texture2d/flask_icons/fakeicon.png")]
    [InlineData("Assets/Mageblood/Texture2D/Flask_Icons/FakeIcon.PNG")]
    [InlineData("assets\\mageblood\\texture2d\\flask_icons\\FakeIcon.png")]
    public void TryIconName_Valid(string asset)
    {
        bool ok = MagebloodFlaskIconAssets.TryIconName(asset, out string name);

        Assert.True(ok);
        Assert.Equal("fakeicon", name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("assets/mageblood/texture2d/other/fakeicon.png")]
    [InlineData("assets/mageblood/texture2d/flask_icons/sub/fakeicon.png")]
    [InlineData("assets/mageblood/texture2d/flask_icons/.png")]
    [InlineData("assets/mageblood/texture2d/flask_icons/fakeicon.jpg")]
    [InlineData("assets/mageblood/texture2d/icon.png")]
    public void TryIconName_Invalid(string asset)
    {
        bool ok = MagebloodFlaskIconAssets.TryIconName(asset, out _);

        Assert.False(ok);
    }

    [Theory]
    [InlineData("FakeIcon", "fakeicon")]
    [InlineData("fakeicon", "fakeicon")]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("  ", null)]
    public void Key_LowercasesAndBlankIsNull(string icon, string expected)
    {
        Assert.Equal(expected, MagebloodFlaskIconAssets.Key(icon));
    }
}
