namespace LastEpoch_Hud.Scripts.Core.Items;

/// <summary>Assigns crafting-slot affix override slots in affix order.</summary>
public sealed class CraftingSlotAffixSlots
{
    public const int SlotCount = 4;

    private const int NoSlot = -1;
    private const int PrefixStart = 0;
    private const int SuffixStart = 2;
    private const int MaxPerKind = 3;

    private int _prefixes;
    private int _suffixes;

    public int NextPrefix() => Next(PrefixStart, ref _prefixes);

    public int NextSuffix() => Next(SuffixStart, ref _suffixes);

    private static int Next(int start, ref int used)
    {
        if (used >= MaxPerKind)
        {
            return NoSlot;
        }
        int slot = start + used;
        used++;
        return slot < SlotCount ? slot : NoSlot;
    }
}
