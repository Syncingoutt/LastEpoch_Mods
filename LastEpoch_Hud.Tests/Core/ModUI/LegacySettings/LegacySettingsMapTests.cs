using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings;

public sealed class LegacySettingsMapTests
{
    [Fact]
    public void All_LegacyPathsExistInDefaultFixture()
    {
        IEnumerable<string> missing = LegacySettingsMap
            .All.Where(m => LegacyJsonPath.Find(LegacyDefaultSave.Root, m.LegacyPath) == null)
            .Select(m => m.LegacyPath);

        Assert.Empty(missing);
    }

    [Fact]
    public void All_GroupKeyPartIsUnique()
    {
        IEnumerable<IGrouping<(string, string, LegacySettingPart), LegacyFieldMapping>> dupes =
            LegacySettingsMap.All.GroupBy(m => (m.Group, m.Key, m.Part)).Where(g => g.Count() > 1);

        Assert.Empty(dupes);
    }

    [Fact]
    public void All_EachKeyUsesOneSettingKind()
    {
        IEnumerable<IGrouping<(string, string), LegacyFieldMapping>> mixed = LegacySettingsMap
            .All.GroupBy(m => (m.Group, m.Key))
            .Where(g => g.Select(m => KindOf(m.Part)).Distinct().Count() > 1);

        Assert.Empty(mixed);
    }

    [Fact]
    public void All_LimitsUnsetOrOrdered()
    {
        IEnumerable<string> bad = LegacySettingsMap
            .All.Where(m => !(m.MinLimit < m.MaxLimit || (m.MinLimit == 0f && m.MaxLimit == 0f)))
            .Select(m => m.LegacyPath);

        Assert.Empty(bad);
    }

    [Fact]
    public void DefaultFixture_IsLegacyDefaultConfig()
    {
        Assert.Equal(284, (int)LegacyDefaultSave.Root["KeyBinds"]["BankStashs"]);
        Assert.Equal(288, (int)LegacyDefaultSave.Root["KeyBinds"]["DamageMeter"]);
        Assert.True((bool)LegacyDefaultSave.Root["Items"]["Pickup"]["AutoShatter_Configured"]);
    }

    private static string KindOf(LegacySettingPart part)
    {
        return part switch
        {
            LegacySettingPart.FloatEnabled or LegacySettingPart.FloatValue => "Float",
            LegacySettingPart.RangeEnabled
            or LegacySettingPart.RangeMin
            or LegacySettingPart.RangeMax => "Range",
            _ => part.ToString(),
        };
    }
}
