using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;
using LastEpoch_Hud.Tests.Support;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Config;

/// <summary>The default flasks must name stats and tags the game's SP and AT enums have.</summary>
public sealed class MagebloodConfigDefaultsGameTests
{
    [Fact]
    public void DefaultConfig_ResolvesAgainstGameStatsAndTags()
    {
        GameEnvironment.SkipWithoutGame();
        var problems = new List<MagebloodConfigProblem>();

        IReadOnlyList<MagebloodFlask> flasks = MagebloodConfigResolver.Resolve(
            MagebloodConfigDefaults.Config,
            GameEnumIds.Read("SP"),
            GameEnumIds.Read("AT"),
            problems
        );

        Assert.Empty(problems);
        Assert.Equal(11, flasks.Count);
    }
}
