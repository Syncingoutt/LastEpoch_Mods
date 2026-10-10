using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;
using LastEpoch_Hud.Tests.Support;
using Mono.Cecil;

namespace LastEpoch_Hud.Tests.Interop;

/// <summary>Our copies of game consts match the game's metadata.</summary>
public sealed class GameConstantTests
{
    [Fact]
    public void ResistanceCap_MatchesGame()
    {
        GameEnvironment.SkipWithoutGame();
        GameEnvironment.SkipWithoutCpp2IlOut();
        ModuleDefinition game = GameEnvironment.ReadGameModule(
            Path.Combine(GameEnvironment.Cpp2IlOutDir, "LE.dll")
        );

        FieldDefinition field = game.GetType("ProtectionClass")
            .Fields.Single(f => f.Name == "resistanceCap");

        Assert.True(field.IsLiteral);
        Assert.Equal(MagebloodMaxResistance.GameCap, (float)field.Constant);
    }
}
