using Mono.Cecil;

namespace LastEpoch_Hud.Tests.Support;

/// <summary>Reads the ids of a game enum from the Il2Cpp assemblies without loading them.</summary>
internal static class GameEnumIds
{
    public static Dictionary<string, int> Read(string typeName)
    {
        using ModuleDefinition module = GameEnvironment.ReadGameModule(
            Path.Combine(GameEnvironment.Il2CppDir, "Il2CppLE.dll")
        );
        TypeDefinition type = module.GetType("Il2Cpp", typeName);
        Assert.NotNull(type);
        return type
            .Fields.Where(field => field.IsStatic && field.IsLiteral)
            .ToDictionary(
                field => field.Name,
                field => Convert.ToInt32(field.Constant),
                StringComparer.Ordinal
            );
    }
}
