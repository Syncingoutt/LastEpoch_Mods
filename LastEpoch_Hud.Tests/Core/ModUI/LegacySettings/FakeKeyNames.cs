using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;

namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings;

internal sealed class FakeKeyNames : ILegacyKeyNames
{
    private static readonly Dictionary<string, int> _codes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["None"] = 0,
        ["F3"] = 284,
        ["F7"] = 288,
        ["A"] = 97,
    };

    public bool TryGetName(int code, out string name)
    {
        name = _codes.Where(pair => pair.Value == code).Select(pair => pair.Key).FirstOrDefault();
        return name != null;
    }

    public bool TryGetCode(string name, out int code)
    {
        return _codes.TryGetValue(name ?? string.Empty, out code);
    }
}
