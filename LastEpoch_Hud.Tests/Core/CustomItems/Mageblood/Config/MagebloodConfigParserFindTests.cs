using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood.Config;
using Newtonsoft.Json.Linq;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Config;

public sealed class MagebloodConfigParserFindTests
{
    private const string ValidFlask =
        """{"name":"FakeFlask","stats":[{"stat":"FakeA","added":1}]}""";

    [Fact]
    public void FindFlask_BrokenEarlierSameName_ReturnsKeptEntry()
    {
        var list = JArray.Parse("""[{"name":"FakeFlask"},""" + ValidFlask + "]");

        Assert.Same(list[1], MagebloodConfigParser.FindFlask(list, "FakeFlask"));
    }

    [Fact]
    public void FindFlask_DuplicateNames_ReturnsFirst()
    {
        var list = JArray.Parse("[" + ValidFlask + "," + ValidFlask + "]");

        Assert.Same(list[0], MagebloodConfigParser.FindFlask(list, "FakeFlask"));
    }

    [Theory]
    [InlineData("FakeMissing")]
    [InlineData(null)]
    public void FindFlask_UnknownOrNull_ReturnsNull(string name)
    {
        var list = JArray.Parse("[" + ValidFlask + "]");

        Assert.Null(MagebloodConfigParser.FindFlask(list, name));
    }

    [Fact]
    public void FindRow_BrokenEarlierSameRow_ReturnsKeptRow()
    {
        var list = JArray.Parse("""[{"stat":"FakeA"},{"stat":"FakeA","added":1}]""");

        Assert.Same(list[1], MagebloodConfigParser.FindRow(list, "FakeA"));
    }

    [Fact]
    public void FindRow_DuplicateRows_ReturnsFirst()
    {
        var list = JArray.Parse("""[{"stat":"FakeA","added":1},{"stat":"FakeA","added":2}]""");

        Assert.Same(list[0], MagebloodConfigParser.FindRow(list, "FakeA"));
    }

    [Fact]
    public void FindRow_TaggedRowText_MatchesTaggedRow()
    {
        var list = JArray.Parse(
            """[{"stat":"FakeA","added":1},{"stat":"FakeA","tag":"FakeTag","added":2}]"""
        );

        Assert.Same(list[1], MagebloodConfigParser.FindRow(list, "FakeA_FakeTag"));
    }

    [Fact]
    public void FindRow_Unknown_ReturnsNull()
    {
        var list = JArray.Parse("""[{"stat":"FakeA","added":1}]""");

        Assert.Null(MagebloodConfigParser.FindRow(list, "FakeMissing"));
    }
}
