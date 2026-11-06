using LastEpoch_Hud.Scripts.Core;

namespace LastEpoch_Hud.Tests.Core;

public class ShrineOverrideIdTests
{
    private const int Placed = 5;

    [Theory]
    [InlineData(0f, 41, 0)]
    [InlineData(2.7f, 41, 2)]
    [InlineData(40f, 41, 40)]
    public void Resolve_ReturnsStoredIdInList(float stored, int count, int expected) =>
        Assert.Equal(expected, ShrineOverrideId.Resolve(stored, count, Placed));

    [Theory]
    [InlineData(-1f, 41)]
    [InlineData(41f, 41)]
    [InlineData(100000f, 41)]
    [InlineData(float.NaN, 41)]
    [InlineData(float.PositiveInfinity, 41)]
    [InlineData(0f, 0)]
    public void Resolve_KeepsPlacedIdOutsideList(float stored, int count) =>
        Assert.Equal(Placed, ShrineOverrideId.Resolve(stored, count, Placed));
}
