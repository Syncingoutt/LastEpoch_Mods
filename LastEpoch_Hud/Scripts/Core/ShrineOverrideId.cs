namespace LastEpoch_Hud.Scripts.Core;

/// <summary>Shrine id a placed shrine becomes under Shrine Override.</summary>
public static class ShrineOverrideId
{
    public static int Resolve(float storedId, int shrineCount, int placedId)
    {
        if (float.IsNaN(storedId) || storedId < 0f)
        {
            return placedId;
        }
        if (storedId >= shrineCount)
        {
            return placedId;
        }
        return (int)storedId;
    }
}
