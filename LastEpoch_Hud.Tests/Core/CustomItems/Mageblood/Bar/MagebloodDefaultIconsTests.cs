using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Bar;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Bar;

public sealed class MagebloodDefaultIconsTests
{
    [Theory]
    [InlineData("Basalt")]
    [InlineData("BASALT")]
    [InlineData("basalt")]
    public void BuffIconFor_Basalt_IsArmour(string flask)
    {
        Assert.Equal("Armour", MagebloodDefaultIcons.BuffIconFor(flask));
    }

    [Fact]
    public void BuffIconFor_KeysMatchDefaultFlasks()
    {
        Assert.Contains(
            MagebloodConfigDefaults.Flasks,
            flask => MagebloodDefaultIcons.BuffIconFor(flask.Name) != null
        );
    }

    [Theory]
    [InlineData("FakeA")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void BuffIconFor_Other_IsNull(string flask)
    {
        Assert.Null(MagebloodDefaultIcons.BuffIconFor(flask));
    }
}
