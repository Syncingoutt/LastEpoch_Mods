using System;
using System.Collections.Generic;

namespace LastEpoch_Hud.Scripts.Core.ForceDrop;

public static class ForceDropPackingOrder
{
    // The observed v6 Legendary decoder marks only the final affix as
    // FromCorruption. Base items use their separate leading sealed slots.
    public static IReadOnlyList<int> UniqueCorruptionLastIndices(IReadOnlyList<ForceDropSeal> seals)
    {
        if (seals == null)
            throw new ArgumentNullException(nameof(seals));
        var order = new List<int>();
        int corruption = -1;
        for (int i = 0; i < seals.Count; i++)
            if (seals[i] == ForceDropSeal.Corruption)
            {
                if (corruption >= 0)
                    throw new ArgumentException("Multiple corruption slots.", nameof(seals));
                corruption = i;
            }
            else
                order.Add(i);
        if (corruption >= 0)
            order.Add(corruption);
        return order.AsReadOnly();
    }

    // Fixed unique modifiers occupy the unique prefix before the native seal
    // and ordinary-affix sequence. Return indices so callers move existing
    // objects without reconstructing or changing any selected affix metadata.
    public static IReadOnlyList<int> UniquePrefixIndices(
        IReadOnlyList<int> affixIds,
        IEnumerable<int> variantIds
    )
    {
        if (affixIds == null)
            throw new ArgumentNullException(nameof(affixIds));
        if (variantIds == null)
            throw new ArgumentNullException(nameof(variantIds));
        var variants = new HashSet<int>();
        foreach (int id in variantIds)
            if (!variants.Add(id))
                throw new ArgumentException("Duplicate unique modifier IDs.", nameof(variantIds));
        var seen = new HashSet<int>();
        var prefix = new List<int>();
        var remaining = new List<int>();
        for (int i = 0; i < affixIds.Count; i++)
        {
            int id = affixIds[i];
            if (!seen.Add(id))
                throw new ArgumentException("Duplicate item affix IDs.", nameof(affixIds));
            if (variants.Contains(id))
                prefix.Add(i);
            else
                remaining.Add(i);
        }
        if (prefix.Count != variants.Count)
            throw new ArgumentException("A selected unique modifier is missing.", nameof(affixIds));
        prefix.AddRange(remaining);
        return prefix.AsReadOnly();
    }
}
