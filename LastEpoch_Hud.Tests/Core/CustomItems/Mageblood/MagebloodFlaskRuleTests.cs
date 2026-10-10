using LastEpoch_Hud.Scripts.Core.CustomItems.Headhunter.Buffs;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood;

public sealed class MagebloodFlaskRuleTests
{
    private static readonly string[] _none = Array.Empty<string>();

    [Theory]
    [InlineData(5, -1, 0)]
    [InlineData(5, 0, 0)]
    [InlineData(5, 3, 3)]
    [InlineData(2, 4, 2)]
    [InlineData(0, 4, 0)]
    public void ActiveCount_Clamps(int flaskCount, int slots, int expected)
    {
        Assert.Equal(expected, MagebloodFlaskRule.ActiveCount(flaskCount, slots));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Plan_FirstNFlasks(int slots)
    {
        IReadOnlyList<BuffAction> actions = MagebloodFlaskRule.Plan(FiveFlasks(), slots, _none);

        string[] expected = Enumerable
            .Range(0, slots)
            .SelectMany(i => new[] { $"MB_F{i}_A", $"MB_F{i}_B" })
            .ToArray();
        Assert.Equal(expected, actions.Select(a => a.BuffName));
        Assert.All(actions, a => Assert.Equal(BuffActionKind.Add, a.Kind));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Plan_ZeroOrNegativeSlots_NoAdds(int slots)
    {
        IReadOnlyList<BuffAction> actions = MagebloodFlaskRule.Plan(FiveFlasks(), slots, _none);

        Assert.Empty(actions);
    }

    [Fact]
    public void Plan_SlotsAboveCount_AllFlasks()
    {
        IReadOnlyList<BuffAction> actions = MagebloodFlaskRule.Plan(FiveFlasks(), 99, _none);

        Assert.Equal(10, actions.Count);
    }

    [Fact]
    public void Plan_ZeroSlots_RemovesAllApplied()
    {
        IReadOnlyList<BuffAction> actions = MagebloodFlaskRule.Plan(
            FiveFlasks(),
            0,
            new[] { "MB_X", "MB_F0_A" }
        );

        Assert.Equal(new[] { "MB_X", "MB_F0_A" }, actions.Select(a => a.BuffName));
        Assert.All(actions, a => Assert.Equal(BuffActionKind.Remove, a.Kind));
    }

    [Fact]
    public void Plan_CopiesRowValues()
    {
        var flask = new MagebloodFlask(
            "FakeX",
            "FakeX",
            new[] { new MagebloodBuffStat(5, 2, "MB_X_A", 1.5f, 0.25f, -0.5f) },
            new[] { new MagebloodStatEntry("FakeA", 1.5f, 25f, -50f) }
        );

        IReadOnlyList<BuffAction> actions = MagebloodFlaskRule.Plan(new[] { flask }, 1, _none);

        Assert.Equal(
            new BuffAction(
                BuffActionKind.Add,
                "MB_X_A",
                5,
                1.5f,
                0.25f,
                MagebloodFlaskRule.PermanentSeconds,
                1,
                2,
                -0.5f
            ),
            Assert.Single(actions)
        );
    }

    [Fact]
    public void Plan_RemovesLeftoversFirst_InAppliedOrder()
    {
        IReadOnlyList<BuffAction> actions = MagebloodFlaskRule.Plan(
            FiveFlasks(),
            1,
            new[] { "MB_Z", "MB_F0_A", "MB_Y" }
        );

        Assert.Equal(new[] { "MB_Z", "MB_Y" }, Removes(actions).Select(a => a.BuffName));
        Assert.DoesNotContain(Removes(actions), a => a.BuffName == "MB_F0_A");
        Assert.Equal(BuffActionKind.Remove, actions[0].Kind);
        Assert.Equal(BuffActionKind.Remove, actions[1].Kind);
    }

    [Fact]
    public void Plan_AddsAfterRemoves()
    {
        IReadOnlyList<BuffAction> actions = MagebloodFlaskRule.Plan(
            FiveFlasks(),
            2,
            new[] { "MB_Z", "MB_Y" }
        );

        int lastRemove = LastIndex(actions, BuffActionKind.Remove);
        int firstAdd = FirstIndex(actions, BuffActionKind.Add);
        Assert.True(lastRemove < firstAdd);
    }

    [Fact]
    public void Plan_RemoveAction_HasOnlyName()
    {
        IReadOnlyList<BuffAction> actions = MagebloodFlaskRule.Plan(
            Array.Empty<MagebloodFlask>(),
            2,
            new[] { "MB_Z" }
        );

        Assert.Equal(
            new BuffAction(BuffActionKind.Remove, "MB_Z", 0, 0f, 0f, 0f, 0),
            Assert.Single(actions)
        );
    }

    [Fact]
    public void Plan_DuplicateApplied_RemovedOnce()
    {
        IReadOnlyList<BuffAction> actions = MagebloodFlaskRule.Plan(
            Array.Empty<MagebloodFlask>(),
            2,
            new[] { "MB_Z", "MB_Z" }
        );

        Assert.Single(actions);
    }

    [Fact]
    public void Plan_NullApplied_NoRemoves()
    {
        IReadOnlyList<BuffAction> actions = MagebloodFlaskRule.Plan(FiveFlasks(), 2, null);

        Assert.Empty(Removes(actions));
        Assert.Equal(4, actions.Count);
    }

    [Fact]
    public void Plan_NullFlasks_RemovesAllApplied()
    {
        IReadOnlyList<BuffAction> actions = MagebloodFlaskRule.Plan(
            null,
            3,
            new[] { "MB_Z", "MB_Y" }
        );

        Assert.Equal(new[] { "MB_Z", "MB_Y" }, actions.Select(a => a.BuffName));
        Assert.All(actions, a => Assert.Equal(BuffActionKind.Remove, a.Kind));
    }

    [Fact]
    public void Plan_NullFlasksAndApplied_Empty()
    {
        Assert.Empty(MagebloodFlaskRule.Plan(null, 3, null));
    }

    private static MagebloodFlask[] FiveFlasks()
    {
        return Enumerable.Range(0, 5).Select(Flask).ToArray();
    }

    private static MagebloodFlask Flask(int index)
    {
        return new MagebloodFlask(
            $"F{index}",
            $"F{index}",
            new[]
            {
                new MagebloodBuffStat(1, 0, $"MB_F{index}_A", 1f, 0f, 0f),
                new MagebloodBuffStat(2, 0, $"MB_F{index}_B", 1f, 0f, 0f),
            },
            new[]
            {
                new MagebloodStatEntry("FakeA", Added: 1f),
                new MagebloodStatEntry("FakeB", Added: 1f),
            }
        );
    }

    private static IEnumerable<BuffAction> Removes(IReadOnlyList<BuffAction> actions)
    {
        return actions.Where(a => a.Kind == BuffActionKind.Remove);
    }

    private static int FirstIndex(IReadOnlyList<BuffAction> actions, BuffActionKind kind)
    {
        return actions.Select((a, i) => (a, i)).First(x => x.a.Kind == kind).i;
    }

    private static int LastIndex(IReadOnlyList<BuffAction> actions, BuffActionKind kind)
    {
        return actions.Select((a, i) => (a, i)).Last(x => x.a.Kind == kind).i;
    }
}
