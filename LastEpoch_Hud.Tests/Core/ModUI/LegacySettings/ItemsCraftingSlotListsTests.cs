using LastEpoch_Hud.Tests.Support;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings;

public sealed class ItemsCraftingSlotListsTests
{
    private const string SlotType = "LastEpoch_Hud.Scripts.ModUI.ModSettings/ItemsCraftingSlot";
    private const string BackingSuffix = ">k__BackingField";

    [Fact]
    public void Lists_AreSlotOrdered()
    {
        GameEnvironment.SkipWithoutModBuild();
        Dictionary<string, List<string>> lists = ReadLists().Lists;

        Assert.Equal(new[] { "Implicit0", "Implicit1", "Implicit2" }, lists["Implicits"]);
        Assert.Equal(Slots("Affix{0}Tier", 4), lists["AffixTiers"]);
        Assert.Equal(Slots("Affix{0}Value", 4), lists["AffixValues"]);
        Assert.Equal(Slots("UniqueMod{0}", 8), lists["UniqueMods"]);
    }

    [Fact]
    public void Overrides_HoldsEveryFloatOnce()
    {
        GameEnvironment.SkipWithoutModBuild();
        (Dictionary<string, List<string>> lists, _, List<string> floats) = ReadLists();

        Assert.Equal(24, floats.Count);
        Assert.Equal(floats.OrderBy(n => n), lists["Overrides"].OrderBy(n => n));
    }

    [Fact]
    public void Lists_InitAfterTheirFields()
    {
        GameEnvironment.SkipWithoutModBuild();
        (Dictionary<string, List<string>> lists, Dictionary<string, int> order, _) = ReadLists();

        IEnumerable<string> bad = lists
            .SelectMany(l => l.Value.Select(f => (List: l.Key, Field: f)))
            .Where(p => !order.TryGetValue(p.Field, out int at) || at > order[p.List])
            .Select(p => p.List + " before " + p.Field);

        Assert.Empty(bad);
    }

    private static List<string> Slots(string pattern, int count)
    {
        return Enumerable.Range(0, count).Select(i => string.Format(pattern, i)).ToList();
    }

    private static (
        Dictionary<string, List<string>> Lists,
        Dictionary<string, int> Order,
        List<string> Floats
    ) ReadLists()
    {
        Dictionary<string, List<string>> lists = new();
        Dictionary<string, int> order = new();
        List<string> floats = new();
        List<string> loaded = new();
        using var module = ModuleDefinition.ReadModule(GameEnvironment.ModDll);
        TypeDefinition type = module.GetTypes().First(t => t.FullName == SlotType);
        MethodDefinition initializer = type.Methods.First(m => m.IsConstructor && m.IsStatic);
        int position = 0;
        foreach (Instruction instruction in initializer.Body.Instructions)
        {
            if (instruction.OpCode == OpCodes.Ldsfld && instruction.Operand is FieldReference read)
            {
                loaded.Add(read.Name);
                continue;
            }
            if (instruction.OpCode != OpCodes.Stsfld)
            {
                continue;
            }
            var written = (FieldReference)instruction.Operand;
            string name = written.Name.TrimStart('<').Replace(BackingSuffix, string.Empty);
            order[name] = position++;
            if (written.FieldType.Name.StartsWith("IReadOnlyList", StringComparison.Ordinal))
            {
                lists[name] = new List<string>(loaded);
            }
            if (written.FieldType.Name == "FloatSetting")
            {
                floats.Add(name);
            }
            loaded.Clear();
        }
        return (lists, order, floats);
    }
}
