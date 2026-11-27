using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public sealed class ItemsRequirementsMapTests
{
    [Fact]
    public void DefaultFixture_GivesDefaults()
    {
        var problems = new List<LegacyImportProblem>();

        JObject sections = Import(LegacyDefaultSave.Text, problems);

        var section = (JObject)sections[ItemsRequirementsMap.Group];
        Assert.Equal(3, section.Count);
        Assert.False((bool)section["Level"]);
        Assert.False((bool)section["Class"]);
        Assert.False((bool)section["Set"]);
        Assert.Empty(problems);
    }

    [Theory]
    [InlineData("level", "Level")]
    [InlineData("classe", "Class")]
    [InlineData("set", "Set")]
    public void Field_MapsToItsKey(string legacyField, string key)
    {
        var problems = new List<LegacyImportProblem>();
        JObject expected = new()
        {
            ["Level"] = key == "Level",
            ["Class"] = key == "Class",
            ["Set"] = key == "Set",
        };

        JObject sections = Import(Legacy(legacyField), problems);

        Assert.True(JToken.DeepEquals(expected, sections[ItemsRequirementsMap.Group]));
        Assert.Empty(problems);
    }

    [Fact]
    public void PartialFile_EmitsOnlyPresent()
    {
        JObject sections = Import(
            "{\"Items\":{\"Req\":{\"classe\":true}}}",
            new List<LegacyImportProblem>()
        );

        var section = (JObject)sections[ItemsRequirementsMap.Group];
        JProperty only = Assert.Single(section.Properties());
        Assert.Equal("Class", only.Name);
        Assert.True((bool)only.Value);
    }

    [Fact]
    public void LegacySettingsMap_ContainsArea()
    {
        IEnumerable<LegacyFieldMapping> missing = ItemsRequirementsMap.All.Where(m =>
            !LegacySettingsMap.All.Contains(m)
        );

        Assert.Empty(missing);
    }

    private static string Legacy(string trueField)
    {
        JObject req = new();
        foreach (string field in new[] { "level", "classe", "set" })
        {
            req[field] = field == trueField;
        }
        return new JObject { ["Items"] = new JObject { ["Req"] = req } }.ToString();
    }

    private static JObject Import(string text, List<LegacyImportProblem> problems)
    {
        Assert.True(
            LegacySettingsImporter.TryImport(
                text,
                ItemsRequirementsMap.All,
                new[] { ItemsRequirementsMap.Group },
                new FakeKeyNames(),
                out JObject sections,
                problems
            )
        );
        return sections;
    }
}
