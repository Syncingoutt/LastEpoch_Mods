using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LastEpoch_Hud.Tests.Interop;

/// <summary>
/// Finds static setter calls on Il2Cpp game types whose original field is a const.
/// Il2CppInterop still emits such a setter, and calling it corrupts native memory.
/// Fields Il2CppInterop renamed are not found and are skipped.
/// </summary>
internal sealed class ConstWriteScanner
{
    private const string SetterPrefix = "set_";
    private const string InteropPrefix = "Il2Cpp";

    private readonly string _il2CppDir;
    private readonly string _cpp2IlOutDir;
    private readonly Dictionary<string, ModuleDefinition> _dummies = new();

    public ConstWriteScanner(string il2CppDir, string cpp2IlOutDir)
    {
        _il2CppDir = il2CppDir;
        _cpp2IlOutDir = cpp2IlOutDir;
    }

    public IReadOnlyList<ConstWrite> Scan(ModuleDefinition mod) =>
        mod
            .Types.SelectMany(AllTypes)
            .SelectMany(type => type.Methods.Where(m => m.HasBody).Select(m => (type, m)))
            .SelectMany(pair => ConstWritesIn(pair.type, pair.m))
            .ToList();

    public bool IsConstSetter(MethodReference method)
    {
        if (method.HasThis || !method.Name.StartsWith(SetterPrefix))
        {
            return false;
        }
        if (!IsGameAssembly(method.DeclaringType.Scope))
        {
            return false;
        }

        TypeDefinition dummy = DummyType(method.DeclaringType);
        string field = method.Name.Substring(SetterPrefix.Length);
        return dummy?.Fields.Any(f => f.Name == field && f.IsLiteral) == true;
    }

    private static IEnumerable<TypeDefinition> AllTypes(TypeDefinition type) =>
        new[] { type }.Concat(type.NestedTypes.SelectMany(AllTypes));

    private static string DummyAssemblyFile(string assemblyName) =>
        (
            assemblyName.StartsWith(InteropPrefix)
                ? assemblyName.Substring(InteropPrefix.Length)
                : assemblyName
        ) + ".dll";

    private static string DummyTypeName(TypeReference type)
    {
        string name = type.FullName;
        if (name.StartsWith(InteropPrefix + "."))
        {
            return name.Substring(InteropPrefix.Length + 1);
        }
        return name.StartsWith(InteropPrefix) ? name.Substring(InteropPrefix.Length) : name;
    }

    private IEnumerable<ConstWrite> ConstWritesIn(TypeDefinition type, MethodDefinition method) =>
        method
            .Body.Instructions.Where(i => i.OpCode == OpCodes.Call)
            .Select(i => i.Operand)
            .OfType<MethodReference>()
            .Where(IsConstSetter)
            .Select(callee => new ConstWrite(
                $"{type.FullName}::{method.Name}",
                $"{callee.DeclaringType.FullName}::{callee.Name}"
            ));

    private bool IsGameAssembly(IMetadataScope scope)
    {
        string name = scope is ModuleDefinition module ? module.Assembly.Name.Name : scope.Name;
        return File.Exists(Path.Combine(_il2CppDir, name + ".dll"));
    }

    private TypeDefinition DummyType(TypeReference type)
    {
        IMetadataScope scope = type.Scope;
        string assembly = scope is ModuleDefinition module ? module.Assembly.Name.Name : scope.Name;
        string file = Path.Combine(_cpp2IlOutDir, DummyAssemblyFile(assembly));
        if (!File.Exists(file))
        {
            return null;
        }

        if (!_dummies.TryGetValue(file, out ModuleDefinition dummy))
        {
            dummy = ModuleDefinition.ReadModule(file);
            _dummies[file] = dummy;
        }
        return dummy.GetType(DummyTypeName(type));
    }
}
