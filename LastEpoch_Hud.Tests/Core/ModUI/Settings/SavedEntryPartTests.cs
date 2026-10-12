using LastEpoch_Hud.Scripts.Core.ModUI.Settings;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.Settings;

public sealed class SavedEntryPartTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Bool_Absent_KeepsCurrent(bool current)
    {
        Assert.Equal(current, SavedEntryPart.Bool(new JObject(), "Enabled", current));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Bool_Present_ReadsValue(bool stored)
    {
        JObject entry = new() { ["Enabled"] = stored };

        Assert.Equal(stored, SavedEntryPart.Bool(entry, "Enabled", !stored));
    }

    [Fact]
    public void Float_Absent_KeepsCurrent()
    {
        Assert.Equal(7.5f, SavedEntryPart.Float(new JObject(), "Value", 7.5f));
    }

    [Theory]
    [InlineData("3", 3f)]
    [InlineData("2.5", 2.5f)]
    public void Float_Present_ReadsValue(string raw, float expected)
    {
        var entry = JObject.Parse("{\"Value\":" + raw + "}");

        Assert.Equal(expected, SavedEntryPart.Float(entry, "Value", 99f));
    }

    [Fact]
    public void ReadFloat_ValueOnly_KeepsCurrentEnabled()
    {
        var entry = JObject.Parse("{\"Value\":3}");

        SavedFloat read = SavedEntryPart.ReadFloat(entry, true, 1f, 0f, 10f, true);

        Assert.Equal(new SavedFloat(true, 3f, 3f, false), read);
    }

    [Fact]
    public void ReadFloat_ValueOutOfLimits_ClampsAndReportsRaw()
    {
        var entry = JObject.Parse("{\"Enabled\":false,\"Value\":100}");

        SavedFloat read = SavedEntryPart.ReadFloat(entry, true, 1f, 0f, 10f, true);

        Assert.Equal(new SavedFloat(false, 10f, 100f, true), read);
    }

    [Fact]
    public void ReadFloat_NoLimits_DoesNotClamp()
    {
        var entry = JObject.Parse("{\"Value\":100}");

        SavedFloat read = SavedEntryPart.ReadFloat(entry, true, 1f, 0f, 10f, false);

        Assert.Equal(new SavedFloat(true, 100f, 100f, false), read);
    }

    [Fact]
    public void ReadFloat_EmptyEntry_KeepsCurrent()
    {
        SavedFloat read = SavedEntryPart.ReadFloat(new JObject(), false, 4f, 0f, 10f, true);

        Assert.Equal(new SavedFloat(false, 4f, 4f, false), read);
    }

    [Fact]
    public void ReadRange_MinOnly_KeepsCurrentMax()
    {
        var entry = JObject.Parse("{\"Min\":2}");

        SavedRange read = SavedEntryPart.ReadRange(entry, true, 1f, 8f, 0f, 10f);

        Assert.Equal(new SavedRange(true, 2f, 8f, false), read);
    }

    [Fact]
    public void ReadRange_Inverted_SwapsAndReportsSwapped()
    {
        var entry = JObject.Parse("{\"Min\":9,\"Max\":3}");

        SavedRange read = SavedEntryPart.ReadRange(entry, true, 1f, 8f, 0f, 10f);

        Assert.Equal(new SavedRange(true, 3f, 9f, true), read);
    }

    [Fact]
    public void ReadRange_OutOfLimitsNotInverted_ClampsWithoutSwap()
    {
        var entry = JObject.Parse("{\"Min\":-5,\"Max\":50}");

        SavedRange read = SavedEntryPart.ReadRange(entry, false, 1f, 8f, 0f, 10f);

        Assert.Equal(new SavedRange(false, 0f, 10f, false), read);
    }

    [Fact]
    public void ReadRange_EmptyEntry_KeepsCurrent()
    {
        SavedRange read = SavedEntryPart.ReadRange(new JObject(), false, 1f, 8f, 0f, 10f);

        Assert.Equal(new SavedRange(false, 1f, 8f, false), read);
    }
}
