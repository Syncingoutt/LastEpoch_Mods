using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

internal static class LegacyMapImport
{
    public static JObject Import(
        string text,
        IReadOnlyList<LegacyFieldMapping> map,
        string group,
        List<LegacyImportProblem> problems
    )
    {
        Assert.True(
            LegacySettingsImporter.TryImport(
                text,
                map,
                new[] { group },
                new FakeKeyNames(),
                out JObject sections,
                problems
            )
        );
        return sections;
    }

    public static string Wrap(string sectionPath, params (string Name, object Value)[] fields)
    {
        JObject section = new();
        foreach ((string name, object value) in fields)
        {
            section[name] = JToken.FromObject(value);
        }
        JObject root = section;
        foreach (string part in sectionPath.Split('.').Reverse())
        {
            root = new JObject { [part] = root };
        }
        return root.ToString();
    }
}
