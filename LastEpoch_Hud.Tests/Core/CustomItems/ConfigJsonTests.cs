using LastEpoch_Hud.Scripts.Core.CustomItems;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.CustomItems;

public sealed class ConfigJsonTests
{
    [Theory]
    [InlineData("5", 5)]
    [InlineData("2147483647", int.MaxValue)]
    [InlineData("-2147483648", int.MinValue)]
    public void TryGetInt_WholeNumber_ReadsValue(string json, int expected)
    {
        bool ok = ConfigJson.TryGetInt(JToken.Parse(json), out int value);

        Assert.True(ok);
        Assert.Equal(expected, value);
    }

    [Theory]
    [InlineData("2147483648")]
    [InlineData("-2147483649")]
    [InlineData("5.5")]
    [InlineData("\"5\"")]
    [InlineData("null")]
    [InlineData("true")]
    [InlineData("1099511627776")]
    [InlineData("-1099511627776")]
    public void TryGetInt_NotIntInRange_ReturnsFalse(string json)
    {
        Assert.False(ConfigJson.TryGetInt(JToken.Parse(json), out _));
    }

    [Theory]
    [InlineData("5", 5f)]
    [InlineData("0.5", 0.5f)]
    [InlineData("-2.25", -2.25f)]
    public void TryReadNumber_Number_ReadsValue(string json, float expected)
    {
        bool ok = ConfigJson.TryReadNumber(JToken.Parse(json), out float value);

        Assert.True(ok);
        Assert.Equal(expected, value);
    }

    [Theory]
    [InlineData("\"x\"")]
    [InlineData("null")]
    [InlineData("true")]
    [InlineData("1e300")]
    public void TryReadNumber_NotFiniteNumber_ReturnsFalse(string json)
    {
        Assert.False(ConfigJson.TryReadNumber(JToken.Parse(json), out _));
    }

    [Fact]
    public void TryReadNumber_NaN_ReturnsFalse()
    {
        Assert.False(ConfigJson.TryReadNumber(new JValue(double.NaN), out _));
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void TryReadBool_Bool_ReadsValue(string json, bool expected)
    {
        bool ok = ConfigJson.TryReadBool(JToken.Parse(json), out bool value);

        Assert.True(ok);
        Assert.Equal(expected, value);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("\"true\"")]
    [InlineData("null")]
    public void TryReadBool_NotBool_ReturnsFalse(string json)
    {
        Assert.False(ConfigJson.TryReadBool(JToken.Parse(json), out _));
    }

    [Fact]
    public void TryReadText_Text_ReadsValue()
    {
        bool ok = ConfigJson.TryReadText(JToken.Parse("\"a\""), out string value);

        Assert.True(ok);
        Assert.Equal("a", value);
    }

    [Theory]
    [InlineData("\"\"")]
    [InlineData("\"  \"")]
    [InlineData("5")]
    [InlineData("null")]
    [InlineData("true")]
    public void TryReadText_NotUsableText_ReturnsFalse(string json)
    {
        Assert.False(ConfigJson.TryReadText(JToken.Parse(json), out _));
    }
}
