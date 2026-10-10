using LastEpoch_Hud.Scripts.Core.CustomItems;

namespace LastEpoch_Hud.Tests.Core.CustomItems.Mageblood.Menu;

public sealed class FakeConfigText : IConfigText
{
    private readonly bool _readFails;
    private readonly bool _writeFails;

    public FakeConfigText(string text, bool readFails = false, bool writeFails = false)
    {
        Text = text;
        _readFails = readFails;
        _writeFails = writeFails;
    }

    public string Text { get; private set; }

    public int WriteCount { get; private set; }

    public bool Exists()
    {
        return Text != null;
    }

    public string Read()
    {
        return _readFails ? null : Text;
    }

    public bool Write(string text)
    {
        if (_writeFails)
        {
            return false;
        }

        Text = text;
        WriteCount++;
        return true;
    }
}
