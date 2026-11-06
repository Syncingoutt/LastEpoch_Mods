using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings;

public sealed class LegacyImportGroupsTests
{
    [Fact]
    public void MissingFrom_AbsentGroup_Listed()
    {
        LegacyFieldMapping[] map = { Field("G1", "A") };

        IReadOnlyList<string> missing = LegacyImportGroups.MissingFrom(new JObject(), map);

        Assert.Equal(new[] { "G1" }, missing);
    }

    [Fact]
    public void MissingFrom_ObjectGroup_NotListed()
    {
        LegacyFieldMapping[] map = { Field("G1", "A") };
        JObject root = new() { ["G1"] = new JObject() };

        IReadOnlyList<string> missing = LegacyImportGroups.MissingFrom(root, map);

        Assert.Empty(missing);
    }

    [Fact]
    public void MissingFrom_NonObjectGroup_Listed()
    {
        LegacyFieldMapping[] map = { Field("G1", "A") };
        JObject root = new() { ["G1"] = 5 };

        IReadOnlyList<string> missing = LegacyImportGroups.MissingFrom(root, map);

        Assert.Equal(new[] { "G1" }, missing);
    }

    [Fact]
    public void MissingFrom_Distinct_InMapOrder()
    {
        LegacyFieldMapping[] map =
        {
            Field("G2", "A"),
            Field("G1", "A"),
            Field("G2", "B"),
            Field("G3", "A"),
        };

        IReadOnlyList<string> missing = LegacyImportGroups.MissingFrom(new JObject(), map);

        Assert.Equal(new[] { "G2", "G1", "G3" }, missing);
    }

    [Fact]
    public void MissingFrom_NullRoot_AllGroups()
    {
        LegacyFieldMapping[] map = { Field("G1", "A"), Field("G2", "A") };

        IReadOnlyList<string> missing = LegacyImportGroups.MissingFrom(null, map);

        Assert.Equal(new[] { "G1", "G2" }, missing);
    }

    private static LegacyFieldMapping Field(string group, string key)
    {
        return new LegacyFieldMapping("P", group, key, LegacySettingPart.Bool);
    }
}
