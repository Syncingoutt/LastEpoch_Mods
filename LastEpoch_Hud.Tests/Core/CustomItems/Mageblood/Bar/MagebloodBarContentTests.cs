using System;
using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Bar;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;
using Xunit;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Bar;

public class MagebloodBarContentTests
{
    private static readonly MagebloodFlask _flask = new(
        "FakeA",
        "fakeicon",
        Array.Empty<MagebloodBuffStat>(),
        Array.Empty<MagebloodStatEntry>()
    );

    [Fact]
    public void New_NeedsRebuild()
    {
        var content = new MagebloodBarContent();

        Assert.True(content.NeedsRebuild(NewList(), 2));
        Assert.Equal(0, content.Version);
    }

    [Fact]
    public void AfterCompleteBuild_SameInput_NoRebuild()
    {
        var content = new MagebloodBarContent();
        List<MagebloodFlask> list = NewList();
        content.Built(list, 2, true);

        Assert.False(content.NeedsRebuild(list, 2));
    }

    [Fact]
    public void CountChanged_NeedsRebuild()
    {
        var content = new MagebloodBarContent();
        List<MagebloodFlask> list = NewList();
        content.Built(list, 2, true);

        Assert.True(content.NeedsRebuild(list, 3));
    }

    [Fact]
    public void OtherListInstance_NeedsRebuild()
    {
        var content = new MagebloodBarContent();
        content.Built(NewList(), 2, true);

        Assert.True(content.NeedsRebuild(NewList(), 2));
    }

    [Fact]
    public void IncompleteBuild_NeedsRebuild()
    {
        var content = new MagebloodBarContent();
        List<MagebloodFlask> list = NewList();
        content.Built(list, 2, false);

        Assert.True(content.NeedsRebuild(list, 2));
    }

    [Fact]
    public void CompleteThenIncompleteBuild_NeedsRebuild()
    {
        var content = new MagebloodBarContent();
        List<MagebloodFlask> list = NewList();
        content.Built(list, 2, true);
        content.Built(list, 2, false);

        Assert.True(content.NeedsRebuild(list, 2));
    }

    [Fact]
    public void IncompleteThenCompleteBuild_NoRebuild()
    {
        var content = new MagebloodBarContent();
        List<MagebloodFlask> list = NewList();
        content.Built(list, 2, false);
        content.Built(list, 2, true);

        Assert.False(content.NeedsRebuild(list, 2));
    }

    [Fact]
    public void Reset_NeedsRebuild()
    {
        var content = new MagebloodBarContent();
        List<MagebloodFlask> list = NewList();
        content.Built(list, 2, true);

        content.Reset();

        Assert.True(content.NeedsRebuild(list, 2));
    }

    [Fact]
    public void Version_CountsBuilds_ResetKeeps()
    {
        var content = new MagebloodBarContent();
        List<MagebloodFlask> list = NewList();

        content.Built(list, 2, true);
        Assert.Equal(1, content.Version);
        content.Built(list, 2, true);
        Assert.Equal(2, content.Version);
        content.Reset();
        Assert.Equal(2, content.Version);
    }

    private static List<MagebloodFlask> NewList() => new() { _flask };
}
