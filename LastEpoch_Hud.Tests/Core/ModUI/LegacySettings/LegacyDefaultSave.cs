using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings;

internal static class LegacyDefaultSave
{
    public static string Text { get; } =
        File.ReadAllText(
            Path.Combine(
                AppContext.BaseDirectory,
                "Core",
                "ModUI",
                "LegacySettings",
                "LegacyDefaultSave.json"
            )
        );

    public static JObject Root { get; } = JObject.Parse(Text);
}
