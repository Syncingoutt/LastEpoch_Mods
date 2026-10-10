using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;
using Code = LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config.MagebloodConfigProblemCode;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Config;

public sealed class MagebloodConfigResolverTests
{
    [Fact]
    public void Resolve_MapsIdsAndFractions()
    {
        IReadOnlyList<MagebloodFlask> flasks = Resolve(
            out List<MagebloodConfigProblem> problems,
            MagebloodTestData.Flask(
                "FakeFlask",
                new MagebloodStatEntry("FakeA", 1.5f, 50f, -25f, "FakeTag")
            )
        );

        Assert.Empty(problems);
        Assert.Equal(
            new MagebloodBuffStat(7, 3, "MB_FakeFlask_FakeA_FakeTag", 1.5f, 0.5f, -0.25f),
            Assert.Single(Assert.Single(flasks).Stats)
        );
    }

    [Fact]
    public void Resolve_BuffNames_WithAndWithoutTag()
    {
        IReadOnlyList<MagebloodFlask> flasks = Resolve(
            out _,
            MagebloodTestData.Flask(
                "FakeFlask",
                new MagebloodStatEntry("FakeA", Added: 1f),
                new MagebloodStatEntry("FakeB", Added: 1f, Tag: "FakeTag")
            )
        );

        Assert.Equal(
            new[] { "MB_FakeFlask_FakeA", "MB_FakeFlask_FakeB_FakeTag" },
            flasks[0].Stats.Select(row => row.BuffName)
        );
        Assert.Equal(new[] { 0, 3 }, flasks[0].Stats.Select(row => row.Tags));
    }

    [Fact]
    public void Resolve_IconFallsBackToName()
    {
        MagebloodFlaskEntry withIcon = new()
        {
            Name = "FakeWith",
            Icon = "IconA",
            Stats = new[] { new MagebloodStatEntry("FakeA", Added: 1f) },
        };
        MagebloodFlaskEntry without = MagebloodTestData.Flask(
            "FakeWithout",
            new MagebloodStatEntry("FakeA", Added: 1f)
        );

        IReadOnlyList<MagebloodFlask> flasks = Resolve(out _, withIcon, without);

        Assert.Equal(new[] { "IconA", "FakeWithout" }, flasks.Select(f => f.IconName));
    }

    [Fact]
    public void Resolve_KeepsFlaskOrder()
    {
        IReadOnlyList<MagebloodFlask> flasks = Resolve(
            out _,
            MagebloodTestData.Flask("FakeZ", new MagebloodStatEntry("FakeA", Added: 1f)),
            MagebloodTestData.Flask("FakeY", new MagebloodStatEntry("FakeA", Added: 1f))
        );

        Assert.Equal(new[] { "FakeZ", "FakeY" }, flasks.Select(f => f.Name));
    }

    [Fact]
    public void Resolve_UnknownStat_DropsRow()
    {
        IReadOnlyList<MagebloodFlask> flasks = Resolve(
            out List<MagebloodConfigProblem> problems,
            MagebloodTestData.Flask(
                "FakeFlask",
                new MagebloodStatEntry("FakeNope", Added: 1f),
                new MagebloodStatEntry("FakeA", Added: 1f)
            )
        );

        MagebloodConfigProblem problem = Assert.Single(problems);
        Assert.Equal(Code.UnknownStat, problem.Code);
        Assert.Equal("flasks[FakeFlask].stats[FakeNope].stat", problem.Path);
        Assert.Equal(7, Assert.Single(Assert.Single(flasks).Stats).StatId);
    }

    [Fact]
    public void Resolve_UnknownTag_DropsRow()
    {
        IReadOnlyList<MagebloodFlask> flasks = Resolve(
            out List<MagebloodConfigProblem> problems,
            MagebloodTestData.Flask(
                "FakeFlask",
                new MagebloodStatEntry("FakeA", Added: 1f, Tag: "FakeNope"),
                new MagebloodStatEntry("FakeB", Added: 1f)
            )
        );

        MagebloodConfigProblem problem = Assert.Single(problems);
        Assert.Equal(Code.UnknownTag, problem.Code);
        Assert.Equal("flasks[FakeFlask].stats[FakeA_FakeNope].tag", problem.Path);
        Assert.Equal(9, Assert.Single(Assert.Single(flasks).Stats).StatId);
    }

    [Fact]
    public void Resolve_NoRowLeft_DropsFlask_NextMovesUp()
    {
        IReadOnlyList<MagebloodFlask> flasks = Resolve(
            out List<MagebloodConfigProblem> problems,
            MagebloodTestData.Flask("FakeBad", new MagebloodStatEntry("FakeNope", Added: 1f)),
            MagebloodTestData.Flask("FakeGood", new MagebloodStatEntry("FakeA", Added: 1f))
        );

        Assert.Equal("FakeGood", Assert.Single(flasks).Name);
        Assert.Contains(problems, p => p.Code == Code.NoStats && p.Path == "flasks[FakeBad]");
        Assert.Contains(problems, p => p.Code == Code.UnknownStat);
    }

    [Fact]
    public void Resolve_Rows_ParallelToStats()
    {
        var a = new MagebloodStatEntry("FakeA", Increased: 30f);
        var b = new MagebloodStatEntry("FakeB", Added: 5f);

        MagebloodFlask flask = Assert.Single(
            Resolve(out _, MagebloodTestData.Flask("FakeFlask", a, b))
        );

        Assert.Equal(new[] { a, b }, flask.Rows);
        Assert.Equal(flask.Stats.Count, flask.Rows.Count);
    }

    [Fact]
    public void Resolve_Rows_SkipDroppedRows()
    {
        var kept = new MagebloodStatEntry("FakeA", Added: 1f);

        MagebloodFlask flask = Assert.Single(
            Resolve(
                out _,
                MagebloodTestData.Flask(
                    "FakeFlask",
                    new MagebloodStatEntry("FakeNope", Added: 1f),
                    kept
                )
            )
        );

        Assert.Equal(new[] { kept }, flask.Rows);
        Assert.Equal(flask.Stats.Count, flask.Rows.Count);
    }

    private static IReadOnlyList<MagebloodFlask> Resolve(
        out List<MagebloodConfigProblem> problems,
        params MagebloodFlaskEntry[] entries
    )
    {
        problems = new List<MagebloodConfigProblem>();
        return MagebloodConfigResolver.Resolve(
            MagebloodTestData.Config(entries),
            MagebloodTestData.StatIds,
            MagebloodTestData.TagIds,
            problems
        );
    }
}
