using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings;

public sealed class LegacyMapCheckTests
{
    [Fact]
    public void UnknownTargets_AllKnown_Empty()
    {
        LegacyFieldMapping[] map = { new("A", "G", "K", LegacySettingPart.Bool) };

        IReadOnlyList<string> unknown = LegacyMapCheck.UnknownTargets(map, (_, _) => true);

        Assert.Empty(unknown);
    }

    [Fact]
    public void UnknownTargets_Unknown_DistinctGroupKeyInMapOrder()
    {
        LegacyFieldMapping[] map =
        {
            new("A", "G", "Z", LegacySettingPart.FloatEnabled),
            new("B", "G", "Z", LegacySettingPart.FloatValue),
            new("C", "G", "A", LegacySettingPart.Bool),
            new("D", "H", "A", LegacySettingPart.Bool),
            new("E", "G", "Known", LegacySettingPart.Bool),
        };

        IReadOnlyList<string> unknown = LegacyMapCheck.UnknownTargets(
            map,
            (group, key) => group == "G" && key == "Known"
        );

        Assert.Equal(new[] { "G.Z", "G.A", "H.A" }, unknown);
    }
}
