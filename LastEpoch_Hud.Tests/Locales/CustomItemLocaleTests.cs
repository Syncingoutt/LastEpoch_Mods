using System.Text.Json;
using LastEpoch_Hud.Scripts.Core.CustomItems;
using LastEpoch_Hud.Scripts.Core.CustomItems.Headhunter;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Menu;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mjolner;
using LastEpoch_Hud.Tests.Support;

namespace LastEpoch_Hud.Tests.Locales;

/// <summary>Every custom item text key exists in base.json and is translated.</summary>
public sealed class CustomItemLocaleTests
{
    private static readonly string _localesDir = Path.Combine(
        GameEnvironment.ModProjectDir,
        "LastEpoch_Hud",
        "Locales"
    );

    [Fact]
    public void AllKeys_InBaseJson()
    {
        Dictionary<string, string>.KeyCollection keys = Read("base").Keys;
        var missing = CheckedKeys().Where(key => !keys.Contains(key)).ToList();

        Assert.True(missing.Count == 0, $"base.json misses: {string.Join(", ", missing)}");
    }

    [Theory]
    [InlineData("en")]
    [InlineData("fr")]
    [InlineData("zh")]
    [InlineData("ko")]
    public void Language_TranslatesEveryCustomItemKey(string language)
    {
        Dictionary<string, string> texts = Read(language);
        var missing = CheckedKeys()
            .Where(key => string.IsNullOrWhiteSpace(texts.GetValueOrDefault(key)))
            .ToList();

        Assert.True(missing.Count == 0, $"{language}.json misses: {string.Join(", ", missing)}");
    }

    [Theory]
    [InlineData("en")]
    [InlineData("fr")]
    [InlineData("ko")]
    [InlineData("zh")]
    public void Descriptions_FillWithCodeValues(string language)
    {
        Dictionary<string, string> texts = Merged(language);
        var filled = new Dictionary<string, (string Text, string[] Expected)>
        {
            [CustomItemLocaleKeys.HeadhunterDescription] = (
                HeadhunterDescription.Text(texts, 7001f, 7002),
                ["7001", "7002"]
            ),
            [CustomItemLocaleKeys.MagebloodDescription] = (
                MagebloodDescription.Text(texts, 7008f),
                [MagebloodFlaskSlots.RangeText, "7008"]
            ),
            [MagebloodFlasksTexts.Slot] = (MagebloodFlasksTexts.SlotLabel(texts, 0, false), ["1"]),
            [MagebloodFlasksTexts.SlotInactive] = (
                MagebloodFlasksTexts.SlotLabel(texts, 3, true),
                ["4"]
            ),
            [CustomItemLocaleKeys.MjolnerDescriptionProc] = (
                MjolnerDescription.LightningProc(texts, 7004, 7005, 1f, 0.5f),
                ["7004", "7005", "100", "50"]
            ),
            [CustomItemLocaleKeys.MjolnerDescriptionSocketed] = (
                MjolnerDescription.SocketedSkills(texts, 7006, 7007, 9000, "§a", "§b", "§c"),
                ["7006", "7007", "§a", "§b", "§c", "9"]
            ),
        };

        var broken = filled
            .Where(pair => !IsFilled(pair.Value.Text, pair.Value.Expected))
            .Select(pair => pair.Key)
            .ToList();

        Assert.True(broken.Count == 0, $"{language}: wrong fill of {string.Join(", ", broken)}");
    }

    private static IEnumerable<string> CheckedKeys() =>
        CustomItemLocaleKeys
            .All.Concat(MagebloodFlaskNames.Keys)
            .Concat(MagebloodMenuTexts.All)
            .Concat(MagebloodFlasksTexts.All);

    private static bool IsFilled(string text, string[] expected) =>
        text != null && !text.Contains('{') && expected.All(text.Contains);

    // Mirrors the overlay in Main.cs LoadDictionary: empty selected values keep English.
    private static Dictionary<string, string> Merged(string language)
    {
        Dictionary<string, string> merged = Read("en");
        foreach (
            KeyValuePair<string, string> pair in Read(language)
                .Where(pair => !string.IsNullOrEmpty(pair.Value))
        )
        {
            merged[pair.Key] = pair.Value;
        }

        return merged;
    }

    private static Dictionary<string, string> Read(string language) =>
        JsonSerializer.Deserialize<Dictionary<string, string>>(
            File.ReadAllText(Path.Combine(_localesDir, $"{language}.json"))
        );
}
