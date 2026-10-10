using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;
using static LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu.MagebloodFlaskBuilder;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu;

public sealed class MagebloodMenuLayoutTests
{
    public static TheoryData<MagebloodFlask[]> ChangedShapes =>
        new()
        {
            // order
            { With(0, Base()[1], 1, Base()[0]) },
            // name
            { With(0, Flask("Renamed", Base()[0].Rows.ToArray())) },
            // row text
            { With(0, Flask("F0", new MagebloodStatEntry("FakeC", Increased: 10f), Row(0, 1))) },
            // field
            { With(0, Flask("F0", new MagebloodStatEntry("FakeA", Added: 10f), Row(0, 1))) },
            // slot count
            { Base().Take(3).ToArray() },
            // a 5th flask's name
            { With(4, Flask("Other", Base()[4].Rows.ToArray())) },
        };

    [Theory]
    [InlineData(6, 4)]
    [InlineData(2, 2)]
    public void Build_SlotsCappedAtMax(int count, int slots)
    {
        var layout = MagebloodMenuLayout.Build(Named(count));

        Assert.Equal(slots, layout.Slots.Count);
        Assert.Equal(Named(count).Select(f => f.Name), layout.Names);
        Assert.Equal(
            Named(count).Take(slots).Select(f => f.Name),
            layout.Slots.Select(s => s.Flask.Name)
        );
    }

    [Fact]
    public void Build_SlidersPerNonZeroField()
    {
        MagebloodFlask flask = Flask(
            "F0",
            new MagebloodStatEntry("FakeA", Added: 2f, More: -3f),
            new MagebloodStatEntry("FakeB", Increased: 4f)
        );

        MagebloodSlotLayout slot = Assert.Single(MagebloodMenuLayout.Build([flask]).Slots);

        Assert.Equal(
            new[]
            {
                (0, MagebloodStatField.Added),
                (0, MagebloodStatField.More),
                (1, MagebloodStatField.Increased),
            },
            slot.Sliders.Select(s => (s.RowIndex, s.Target.Field))
        );
    }

    [Fact]
    public void Build_TargetsByNameAndRowText()
    {
        MagebloodFlask flask = Flask("F0", new MagebloodStatEntry("FakeA", 1f, Tag: "FakeTag"));

        MagebloodSlotLayout slot = Assert.Single(MagebloodMenuLayout.Build([flask]).Slots);

        Assert.Equal(
            MagebloodValueTarget.ForStat("F0", "FakeA_FakeTag", MagebloodStatField.Added),
            Assert.Single(slot.Sliders).Target
        );
    }

    [Fact]
    public void OptionIndex_MapsSlotToOptions()
    {
        string custom = "ZCustom";
        string first = MagebloodConfigDefaults.Flasks[2].Name;
        string second = MagebloodConfigDefaults.Flasks[0].Name;
        var layout = MagebloodMenuLayout.Build([
            Flask(custom, new MagebloodStatEntry("FakeA", Added: 1f)),
            Flask(first, new MagebloodStatEntry("FakeA", Added: 1f)),
            Flask(second, new MagebloodStatEntry("FakeA", Added: 1f)),
        ]);

        Assert.Equal(new[] { second, first, custom }, layout.Options);
        Assert.Equal(2, layout.OptionIndex(0));
        Assert.Equal(1, layout.OptionIndex(1));
        Assert.Equal(0, layout.OptionIndex(2));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void OptionIndex_OutOfRange_MinusOne(int slot)
    {
        Assert.Equal(-1, MagebloodMenuLayout.Build(Named(2)).OptionIndex(slot));
    }

    [Fact]
    public void FileValue_ReadsField()
    {
        MagebloodFlask flask = Flask("F0", new MagebloodStatEntry("FakeA", Added: 2f, More: -3f));
        MagebloodSlotLayout slot = Assert.Single(MagebloodMenuLayout.Build([flask]).Slots);

        Assert.Equal(new[] { 2f, -3f }, slot.Sliders.Select(slot.FileValue));
    }

    [Fact]
    public void Build_Null_Empty()
    {
        var layout = MagebloodMenuLayout.Build(null);

        Assert.Empty(layout.Slots);
        Assert.Empty(layout.Names);
        Assert.Empty(layout.Options);
    }

    [Fact]
    public void Range_AddedPercent_UsesDisplayValue()
    {
        MagebloodFlask flask = Flask("F0", new MagebloodStatEntry("AllResistances", Added: 0.35f));
        MagebloodSlotLayout slot = Assert.Single(MagebloodMenuLayout.Build([flask]).Slots);

        Assert.Equal(
            new MagebloodSliderRange(1f, 75f, true),
            slot.Range(Assert.Single(slot.Sliders), true)
        );
    }

    [Fact]
    public void Build_NoFlasks_Empty()
    {
        Assert.Empty(MagebloodMenuLayout.Build([]).Slots);
    }

    [Fact]
    public void SameShape_ValueChange_True()
    {
        MagebloodFlask changedFirst = Flask(
            "F0",
            new MagebloodStatEntry("FakeA", Increased: 99f),
            new MagebloodStatEntry("FakeB", Added: 77f)
        );

        Assert.True(
            MagebloodMenuLayout
                .Build(Base())
                .SameShape(MagebloodMenuLayout.Build(With(0, changedFirst)))
        );
    }

    [Theory]
    [MemberData(nameof(ChangedShapes))]
    public void SameShape_Changed_False(MagebloodFlask[] changed)
    {
        Assert.False(
            MagebloodMenuLayout.Build(Base()).SameShape(MagebloodMenuLayout.Build(changed))
        );
    }

    [Fact]
    public void SameShape_Null_False()
    {
        Assert.False(MagebloodMenuLayout.Build(Base()).SameShape(null));
    }

    private static MagebloodStatEntry Row(int flask, int row) => Base()[flask].Rows[row];

    private static MagebloodFlask[] With(int index, MagebloodFlask flask)
    {
        MagebloodFlask[] flasks = Base();
        flasks[index] = flask;
        return flasks;
    }

    private static MagebloodFlask[] With(
        int first,
        MagebloodFlask firstFlask,
        int second,
        MagebloodFlask secondFlask
    )
    {
        MagebloodFlask[] flasks = With(first, firstFlask);
        flasks[second] = secondFlask;
        return flasks;
    }

    private static MagebloodFlask[] Base() =>
        [
            Flask(
                "F0",
                new MagebloodStatEntry("FakeA", Increased: 10f),
                new MagebloodStatEntry("FakeB", Added: 5f)
            ),
            Flask("F1", new MagebloodStatEntry("FakeA", More: -5f)),
            Flask("F2", new MagebloodStatEntry("FakeA", Increased: 1f)),
            Flask("F3", new MagebloodStatEntry("FakeA", Increased: 2f)),
            Flask("F4", new MagebloodStatEntry("FakeA", Increased: 3f)),
        ];
}
