using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu;

/// <summary>Builds resolved flasks from file rows. Fake data only.</summary>
internal static class MagebloodFlaskBuilder
{
    public static MagebloodFlask Flask(string name, params MagebloodStatEntry[] rows)
    {
        MagebloodBuffStat[] stats = rows.Select(row => Stat(name, row)).ToArray();
        return new MagebloodFlask(name, name, stats, rows);
    }

    public static IReadOnlyList<MagebloodFlask> Named(int count)
    {
        return Enumerable
            .Range(0, count)
            .Select(i => Flask($"F{i}", new MagebloodStatEntry("FakeA", Increased: 10f + i)))
            .ToArray();
    }

    private static MagebloodBuffStat Stat(string flask, MagebloodStatEntry row)
    {
        return new MagebloodBuffStat(
            1,
            0,
            $"MB_{flask}_{row.RowText}",
            row.Added,
            row.Increased,
            row.More
        );
    }
}
