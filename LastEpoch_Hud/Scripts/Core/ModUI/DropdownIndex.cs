namespace LastEpoch_Hud.Scripts.Core.ModUI;

/// <summary>Saved dropdown index checked against the option count.</summary>
public static class DropdownIndex
{
    public static int OrFirst(int index, int optionCount)
    {
        if (index < 0 || index >= optionCount)
        {
            return 0;
        }
        return index;
    }
}
