using System;
using System.Linq;
using LastEpoch_Hud.Scripts.Core.ModUI;

namespace LastEpoch_Hud.Tests.Core.ModUI;

public sealed class HudPageRoutesTests
{
    [Fact]
    public void SearchRoot_EveryPageExceptForceDrop_HasRoot()
    {
        HudPageId[] pages = Enum.GetValues<HudPageId>()
            .Where(id => id != HudPageId.ItemsForceDrop)
            .ToArray();

        Assert.All(pages, id => Assert.False(string.IsNullOrEmpty(HudPageRoutes.SearchRoot(id))));
    }

    [Fact]
    public void SearchRoot_ForceDrop_IsNull()
    {
        Assert.Null(HudPageRoutes.SearchRoot(HudPageId.ItemsForceDrop));
    }

    [Fact]
    public void SearchRoot_UndefinedValue_IsNull()
    {
        Assert.Null(HudPageRoutes.SearchRoot((HudPageId)999));
    }

    [Fact]
    public void SearchRoot_Roots_AreUnique()
    {
        string[] roots = Enum.GetValues<HudPageId>()
            .Select(HudPageRoutes.SearchRoot)
            .Where(root => root != null)
            .ToArray();

        Assert.Equal(roots.Length, roots.Distinct(StringComparer.Ordinal).Count());
    }
}
