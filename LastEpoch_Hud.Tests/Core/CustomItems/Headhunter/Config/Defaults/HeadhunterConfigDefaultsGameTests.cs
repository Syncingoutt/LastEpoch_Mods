using LastEpoch_Hud.Scripts.Core.CustomItems.Headhunter.Config;
using LastEpoch_Hud.Scripts.Core.CustomItems.Headhunter.Config.Defaults;
using LastEpoch_Hud.Scripts.Core.CustomItems.Headhunter.Config.Json;
using LastEpoch_Hud.Scripts.Core.CustomItems.Headhunter.Config.Resolve;
using LastEpoch_Hud.Tests.Support;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Headhunter.Config.Defaults;

/// <summary>The exported default stats must exist in the game's SP enum.</summary>
public sealed class HeadhunterConfigDefaultsGameTests
{
    [Fact]
    public void DefaultStats_AreAllGameStatNames()
    {
        GameEnvironment.SkipWithoutGame();
        HashSet<string> spNames = ReadStatNames();

        HeadhunterConfigParseResult result = HeadhunterConfigParser.Parse(
            HeadhunterConfigWriter.Write(HeadhunterConfigDefaults.Config),
            spNames
        );

        Assert.Empty(result.Problems);
        Assert.Equal(HeadhunterConfigDefaults.Stats.Count, result.Config.Stats.Count);
    }

    [Fact]
    public void DefaultStats_ResolveAgainstGameStatsAndTags()
    {
        GameEnvironment.SkipWithoutGame();
        Dictionary<string, int> spIds = GameEnumIds.Read("SP");
        Dictionary<string, int> atIds = GameEnumIds.Read("AT");
        var problems = new List<HeadhunterConfigProblem>();

        HeadhunterResolvedConfig resolved = HeadhunterConfigResolver.Resolve(
            HeadhunterConfigDefaults.Config,
            spIds,
            atIds,
            problems
        );

        Assert.Empty(problems);
        Assert.Equal(HeadhunterConfigDefaults.Stats.Count, resolved.Stats.Count);
        Assert.Equal(HeadhunterAffixDefaults.AffixMap.Count, resolved.AffixCount);
    }

    private static HashSet<string> ReadStatNames()
    {
        return GameEnumIds.Read("SP").Keys.ToHashSet(StringComparer.Ordinal);
    }
}
