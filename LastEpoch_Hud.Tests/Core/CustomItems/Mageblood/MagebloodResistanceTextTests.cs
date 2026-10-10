using System.Globalization;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood;

public sealed class MagebloodResistanceTextTests
{
    [Theory]
    [InlineData(1.01f, 5f, 80, 101)]
    [InlineData(0.86f, 5f, 80, 86)]
    [InlineData(0.81f, 5f, 80, 81)]
    [InlineData(1.3f, 25f, 100, 130)]
    // float multiply: double would give 101
    [InlineData(1.015f, 5f, 80, 102)]
    // cap 0.775f: float multiply gives 77.5 -> 78, truncation would give 77
    [InlineData(1.01f, 2.5f, 78, 101)]
    public void TryFormat_OverRaisedCap_ShowsRaisedCap(
        float uncapped,
        float bonus,
        int capped,
        int shown
    )
    {
        bool ok = MagebloodResistanceText.TryFormat(uncapped, bonus, out string text);

        Assert.True(ok);
        Assert.Equal(Expected(capped, shown), text);
    }

    [Theory]
    [InlineData(0.78f, 5f, "78%")]
    [InlineData(0.754f, 5f, "75%")]
    [InlineData(0.9f, 40f, "90%")]
    [InlineData(0.8f, 5f, "80%")]
    // float multiply: double would give 79
    [InlineData(0.785f, 5f, "78%")]
    public void TryFormat_NotCappedByRaisedCap_NoSuffix(float uncapped, float bonus, string plain)
    {
        bool ok = MagebloodResistanceText.TryFormat(uncapped, bonus, out string text);

        Assert.True(ok);
        Assert.Equal(plain, text);
    }

    [Theory]
    [InlineData(0.40f)]
    [InlineData(0.75f)]
    [InlineData(0f)]
    [InlineData(-0.2f)]
    [InlineData(float.NaN)]
    public void TryFormat_AtOrBelowGameCap_False(float uncapped)
    {
        bool ok = MagebloodResistanceText.TryFormat(uncapped, 5f, out string text);

        Assert.False(ok);
        Assert.Null(text);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(-5f)]
    [InlineData(float.NaN)]
    public void TryFormat_UnusableBonus_False(float bonus)
    {
        bool ok = MagebloodResistanceText.TryFormat(1.01f, bonus, out string text);

        Assert.False(ok);
        Assert.Null(text);
    }

    [Fact]
    public void TryFormat_HalfPercent_RoundsToEven()
    {
        bool ok = MagebloodResistanceText.TryFormat(1.125f, 5f, out string text);

        Assert.True(ok);
        Assert.Equal(Expected(80, 112), text);
    }

    private static string Expected(int capped, int uncapped)
    {
        return capped
            + "%"
            + MagebloodResistanceText.UncappedColour
            + "\n("
            + uncapped.ToString(CultureInfo.InvariantCulture)
            + "%)";
    }
}
