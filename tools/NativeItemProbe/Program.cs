using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Mono.Cecil;

if (args.Length != 2)
    throw new ArgumentException("Usage: NativeItemProbe <GamePath> <OutputZip>");
string gamePath = Path.GetFullPath(args[0]);
string output = Path.GetFullPath(args[1]);
string nativePath = Path.Combine(gamePath, "GameAssembly.dll");
string cppPath = Path.Combine(
    gamePath,
    "MelonLoader",
    "Dependencies",
    "Il2CppAssemblyGenerator",
    "Cpp2IL",
    "cpp2il_out"
);
if (!File.Exists(nativePath) || !Directory.Exists(cppPath))
    throw new IOException(
        "GameAssembly.dll or cpp2il_out was not found. No native probe was exported."
    );

var addresses = new List<NativeAddress>();
foreach (string dll in Directory.EnumerateFiles(cppPath, "*.dll"))
{
    using var module = ModuleDefinition.ReadModule(dll);
    foreach (var type in module.GetTypes())
    foreach (var method in type.Methods)
    {
        var address = method.CustomAttributes.FirstOrDefault(a =>
            a.AttributeType.Name == "AddressAttribute"
        );
        if (address == null)
            continue;
        long? offset = ReadAddress(address, "Offset");
        long? rva = ReadAddress(address, "RVA");
        if (offset is > 0 && rva is > 0)
            addresses.Add(
                new NativeAddress(type.FullName, method.FullName, offset.Value, rva.Value)
            );
    }
}
if (addresses.Count == 0)
    throw new IOException("No native addresses were found in cpp2il_out.");
var wanted = new HashSet<string>
{
    "ItemData",
    "ItemAffix",
    "TooltipItem",
    "TooltipItemManager",
    "GroundItemLabelButton",
    "UIMouseListener",
    "TooltipSystem",
    "ItemTooltip",
    "UITooltipItem",
    "Tooltip",
    "GroundItemLabel",
};

// Include nested update helpers on these exact types as well as their direct methods.
var targets = addresses
    .Where(a => wanted.Contains(a.Type.Split('/')[0].Split('.').Last()))
    .ToList();
if (!targets.Any(a => a.Type.Split('.').Last() == "ItemData"))
    throw new IOException("ItemData native addresses were not found. Check cpp2il_out.");
var offsets = addresses.Select(a => a.Offset).Distinct().Order().ToArray();
using var native = File.OpenRead(nativePath);
using var hash = SHA256.Create();
string nativeHash = Convert.ToHexString(hash.ComputeHash(native));
Directory.CreateDirectory(Path.GetDirectoryName(output)!);

// Replace only the explicitly requested generated archive.
if (File.Exists(output))
    File.Delete(output);
using var zip = ZipFile.Open(output, ZipArchiveMode.Create);
using (var writer = new StreamWriter(zip.CreateEntry("addresses.json").Open(), Encoding.UTF8))
    writer.Write(JsonSerializer.Serialize(addresses));
using (var writer = new StreamWriter(zip.CreateEntry("manifest.json").Open(), Encoding.UTF8))
    writer.Write(
        JsonSerializer.Serialize(
            new
            {
                GameAssemblySha256 = nativeHash,
                NativeLength = native.Length,
                MethodCount = targets.Count,
                MaximumMethodBytes = 16384,
            }
        )
    );
int count = 0;
foreach (var target in targets)
{
    if (target.Offset >= native.Length)
        throw new IOException("A native offset exceeds GameAssembly.dll; cpp2il_out may be stale.");
    int index = Array.BinarySearch(offsets, target.Offset);
    long next = index + 1 < offsets.Length ? offsets[index + 1] : native.Length;
    int length = (int)Math.Min(16384, Math.Min(next, native.Length) - target.Offset);
    if (length <= 0)
        continue;
    native.Position = target.Offset;
    var bytes = new byte[length];
    native.ReadExactly(bytes);
    using var entry = zip.CreateEntry($"methods/{count++:D4}-{target.Rva:X}.json").Open();
    JsonSerializer.Serialize(
        entry,
        new
        {
            target.Method,
            target.Offset,
            target.Rva,
            Truncated = next - target.Offset > length,
            Bytes = Convert.ToBase64String(bytes),
        }
    );
}
Console.WriteLine($"Exported {count} native method slices to {output}");

static long? ReadAddress(CustomAttribute address, string name)
{
    var field = address.Fields.FirstOrDefault(f => f.Name == name);
    return
        field.Argument.Value is string value
        && long.TryParse(
            value.Replace("0x", "", StringComparison.OrdinalIgnoreCase),
            NumberStyles.HexNumber,
            CultureInfo.InvariantCulture,
            out long parsed
        )
        ? parsed
        : null;
}

record NativeAddress(string Type, string Method, long Offset, long Rva);
