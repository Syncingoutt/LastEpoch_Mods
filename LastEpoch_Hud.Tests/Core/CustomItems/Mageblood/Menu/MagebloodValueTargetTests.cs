using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu;

public sealed class MagebloodValueTargetTests
{
    [Fact]
    public void ForMaxResistances_SetsKindAndNoStatFields()
    {
        Assert.Equal(
            new MagebloodValueTarget(
                MagebloodValueKind.MaxResistances,
                null,
                null,
                MagebloodStatField.Added
            ),
            MagebloodValueTarget.ForMaxResistances()
        );
    }

    [Fact]
    public void ForStat_SetsKindAndFields()
    {
        var target = MagebloodValueTarget.ForStat("F", "R", MagebloodStatField.More);

        Assert.Equal(
            new MagebloodValueTarget(
                MagebloodValueKind.StatValue,
                "F",
                "R",
                MagebloodStatField.More
            ),
            target
        );
    }
}
