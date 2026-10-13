using System.Collections.Generic;
using System.Linq;

namespace LastEpoch_Hud.Scripts.Core.ForceDrop;

// Legendary native IDs cannot encode a regular seal. This extra identity is
// registered only for explicitly created illegal items, keyed by their full ID.
public sealed class UniqueRegularSealRecord
{
    public int UniqueId { get; set; }
    public int ItemType { get; set; }
    public int SubType { get; set; }
    public int AffixId { get; set; }

    public bool IsEncodedAffix(byte[] nativeId, int offset)
    {
        if (
            nativeId == null
            || nativeId.Length < 23
            || nativeId[0] != 6
            || offset < 23
            || (offset - 23) % 3 != 0
            || offset + 2 >= nativeId.Length
            || (offset - 23) / 3 >= (nativeId[22] & 7)
        )
            return false;
        return ((nativeId[offset] & 15) << 8 | nativeId[offset + 1]) == AffixId;
    }

    public bool MatchesIdentity(int uniqueId, int itemType, int subType) =>
        UniqueId == uniqueId
        && ItemType == itemType
        && SubType == subType
        && AffixId >= 0
        && AffixId <= 4095;

    public bool Matches(int uniqueId, int itemType, int subType, IEnumerable<int> affixIds) =>
        MatchesIdentity(uniqueId, itemType, subType)
        && affixIds != null
        && affixIds.Count(id => id == AffixId) == 1;
}
