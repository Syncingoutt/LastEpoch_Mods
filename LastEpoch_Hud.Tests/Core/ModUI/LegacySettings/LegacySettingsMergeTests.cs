using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings;

public sealed class LegacySettingsMergeTests
{
    private static readonly LegacyFieldMapping[] _map =
    {
        new("A.B", "G", "K", LegacySettingPart.Bool),
        new("A.C", "H", "K", LegacySettingPart.Bool),
    };

    [Fact]
    public void ImportMissing_NullText_NoLegacyFile()
    {
        JObject root = new();

        LegacyImportOutcome outcome = Run(null, root, new List<LegacyImportProblem>());

        Assert.Equal(LegacyImportOutcome.NoLegacyFile, outcome);
        Assert.Empty(root);
    }

    [Fact]
    public void ImportMissing_AllGroupsPresent_NothingMissingAndKeepsValues()
    {
        var root = JObject.Parse("{\"G\":{\"K\":false},\"H\":{\"K\":false}}");
        var problems = new List<LegacyImportProblem>();

        LegacyImportOutcome outcome = Run("{ bad", root, problems);

        Assert.Equal(LegacyImportOutcome.NothingMissing, outcome);
        Assert.Empty(problems);
        Assert.False((bool)root["G"]["K"]);
        Assert.False((bool)root["H"]["K"]);
    }

    [Fact]
    public void ImportMissing_MissingGroup_MergedIntoRoot()
    {
        JObject root = new();

        LegacyImportOutcome outcome = Run(
            "{\"A\":{\"B\":true,\"C\":true}}",
            root,
            new List<LegacyImportProblem>()
        );

        Assert.Equal(LegacyImportOutcome.Imported, outcome);
        Assert.True((bool)root["G"]["K"]);
        Assert.True((bool)root["H"]["K"]);
    }

    [Fact]
    public void ImportMissing_PresentGroup_NotOverwritten()
    {
        var root = JObject.Parse("{\"G\":{\"K\":false}}");

        LegacyImportOutcome outcome = Run(
            "{\"A\":{\"B\":true,\"C\":true}}",
            root,
            new List<LegacyImportProblem>()
        );

        Assert.Equal(LegacyImportOutcome.Imported, outcome);
        Assert.False((bool)root["G"]["K"]);
        Assert.True((bool)root["H"]["K"]);
    }

    [Fact]
    public void ImportMissing_NoMappedFields_RootUnchanged()
    {
        JObject root = new();
        var problems = new List<LegacyImportProblem>();

        LegacyImportOutcome outcome = Run("{}", root, problems);

        Assert.Equal(LegacyImportOutcome.NothingMissing, outcome);
        Assert.Empty(problems);
        Assert.Empty(root);
    }

    [Fact]
    public void ImportMissing_BadJson_FileFailedRootUnchanged()
    {
        JObject root = new();
        var problems = new List<LegacyImportProblem>();

        LegacyImportOutcome outcome = Run("{ bad", root, problems);

        Assert.Equal(LegacyImportOutcome.FileFailed, outcome);
        Assert.Equal(LegacyImportProblemCode.InvalidJson, Assert.Single(problems).Code);
        Assert.Empty(root);
    }

    [Fact]
    public void ImportMissing_BadField_OthersMerged()
    {
        JObject root = new();
        var problems = new List<LegacyImportProblem>();

        LegacyImportOutcome outcome = Run("{\"A\":{\"B\":\"x\",\"C\":true}}", root, problems);

        Assert.Equal(LegacyImportOutcome.Imported, outcome);
        LegacyImportProblem problem = Assert.Single(problems);
        Assert.Equal(LegacyImportProblemCode.InvalidValue, problem.Code);
        Assert.Equal("A.B", problem.Path);
        Assert.True((bool)root["H"]["K"]);
        Assert.False(root.ContainsKey("G"));
    }

    [Fact]
    public void ImportMissing_AllFieldsBad_FieldsFailedProblemsKept()
    {
        JObject root = new();
        var problems = new List<LegacyImportProblem>();

        LegacyImportOutcome outcome = Run("{\"A\":{\"B\":\"x\",\"C\":\"y\"}}", root, problems);

        Assert.Equal(LegacyImportOutcome.FieldsFailed, outcome);
        Assert.Equal(
            new[]
            {
                (LegacyImportProblemCode.InvalidValue, "A.B"),
                (LegacyImportProblemCode.InvalidValue, "A.C"),
            },
            problems.Select(p => (p.Code, p.Path))
        );
        Assert.Empty(root);
    }

    [Fact]
    public void ImportMissing_ReadFailedGroupMissing_ReturnsReadFailed()
    {
        JObject root = new();
        var problems = new List<LegacyImportProblem>();

        LegacyImportOutcome outcome = Run(null, root, problems, legacyReadFailed: true);

        Assert.Equal(LegacyImportOutcome.ReadFailed, outcome);
        Assert.Empty(root);
        LegacyImportProblem problem = Assert.Single(problems);
        Assert.Equal(LegacyImportProblemCode.ReadFailed, problem.Code);
        Assert.Equal("", problem.Path);
    }

    [Fact]
    public void ImportMissing_ReadFailedNothingMissing_ReturnsNothingMissing()
    {
        var root = JObject.Parse("{\"G\":{\"K\":false},\"H\":{\"K\":false}}");
        var problems = new List<LegacyImportProblem>();

        LegacyImportOutcome outcome = Run(null, root, problems, legacyReadFailed: true);

        Assert.Equal(LegacyImportOutcome.NothingMissing, outcome);
        Assert.Empty(problems);
    }

    private static LegacyImportOutcome Run(
        string text,
        JObject root,
        List<LegacyImportProblem> problems,
        bool legacyReadFailed = false
    )
    {
        return LegacySettingsMerge.ImportMissing(
            text,
            legacyReadFailed,
            root,
            _map,
            new FakeKeyNames(),
            problems
        );
    }
}
