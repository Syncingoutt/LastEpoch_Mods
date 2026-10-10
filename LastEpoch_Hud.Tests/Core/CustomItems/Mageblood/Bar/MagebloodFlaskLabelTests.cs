using System.Globalization;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Bar;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Bar;

public sealed class MagebloodFlaskLabelTests
{
    [Fact]
    public void Format_NameThenRows()
    {
        string text = MagebloodFlaskLabel.Format("FakeA", new[] { "r1", "r2" });

        Assert.Equal("FakeA\nr1\nr2", text);
    }

    [Fact]
    public void Format_SkipsBlankRows()
    {
        string text = MagebloodFlaskLabel.Format("FakeA", new[] { "r1", null, "", " ", "r2" });

        Assert.Equal("FakeA\nr1\nr2", text);
    }

    [Fact]
    public void Format_NullRows_NameOnly()
    {
        Assert.Equal("FakeA", MagebloodFlaskLabel.Format("FakeA", null));
    }

    [Fact]
    public void WithMore_Zero_Unchanged()
    {
        Assert.Equal("r", MagebloodFlaskLabel.WithMore("r", 0f));
    }

    [Fact]
    public void WithMore_Less()
    {
        Assert.Equal("r x0.85", MagebloodFlaskLabel.WithMore("r", -0.15f));
    }

    [Fact]
    public void WithMore_More()
    {
        Assert.Equal("r x1.2", MagebloodFlaskLabel.WithMore("r", 0.2f));
    }

    [Fact]
    public void WithMore_CommaCulture_UsesDot()
    {
        CultureInfo previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("de-DE");
        try
        {
            Assert.Equal("r x0.85", MagebloodFlaskLabel.WithMore("r", -0.15f));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }
}
