using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Config;

/// <summary>Builders and name maps for the Mageblood config tests. Fake names only.</summary>
internal static class MagebloodTestData
{
    public static readonly IReadOnlyDictionary<string, int> StatIds = new Dictionary<string, int>(
        StringComparer.Ordinal
    )
    {
        ["FakeA"] = 7,
        ["FakeB"] = 9,
    };

    public static readonly IReadOnlyDictionary<string, int> TagIds = new Dictionary<string, int>(
        StringComparer.Ordinal
    )
    {
        ["FakeTag"] = 3,
    };

    public static MagebloodFlaskEntry Flask(string name, params MagebloodStatEntry[] rows)
    {
        return new MagebloodFlaskEntry
        {
            Name = name,
            Icon = null,
            Stats = rows,
        };
    }

    public static MagebloodConfig Config(params MagebloodFlaskEntry[] flasks)
    {
        return new MagebloodConfig { Version = 1, Flasks = flasks };
    }
}
