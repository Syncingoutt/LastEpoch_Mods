using LastEpoch_Hud.Scripts.Core.ForceDrop;
using Newtonsoft.Json;

namespace LastEpoch_Hud.Tests.Core.ForceDrop;

public class UniqueRegularSealRecordTests
{
    [Fact]
    public void SavedExactItemRecordSurvivesJsonReloadWithoutACreationToggle()
    {
        const string id = "BhBlbmNvZGVkTmF0aXZlSXRlbUlE";
        var saved = new Dictionary<string, UniqueRegularSealRecord>
        {
            [id] = new()
            {
                UniqueId = 370,
                ItemType = 1,
                SubType = 7,
                AffixId = 572,
            },
        };
        var restored = JsonConvert.DeserializeObject<Dictionary<string, UniqueRegularSealRecord>>(
            JsonConvert.SerializeObject(saved)
        );
        Assert.True(restored[id].Matches(370, 1, 7, new[] { 978, 643, 572 }));
        Assert.False(restored.ContainsKey(id + "changed"));
    }

    [Theory]
    [InlineData(371, 1, 7)]
    [InlineData(370, 2, 7)]
    [InlineData(370, 1, 8)]
    public void SealRecordCannotTransferToADifferentUniqueOrBase(int unique, int type, int sub)
    {
        var record = new UniqueRegularSealRecord
        {
            UniqueId = 370,
            ItemType = 1,
            SubType = 7,
            AffixId = 572,
        };
        Assert.False(record.Matches(unique, type, sub, new[] { 572 }));
    }

    [Fact]
    public void RemovingOrDuplicatingTheAffixInvalidatesTheRecord()
    {
        var record = new UniqueRegularSealRecord
        {
            UniqueId = 370,
            ItemType = 1,
            SubType = 7,
            AffixId = 572,
        };
        Assert.False(record.Matches(370, 1, 7, new[] { 978, 643 }));
        Assert.False(record.Matches(370, 1, 7, new[] { 572, 572 }));
    }

    [Fact]
    public void V6LegendaryDecodeRestoresOnlyTheExplicitIdAtAnActualAffixOffset()
    {
        var bytes = new byte[44];
        bytes[0] = 6;
        bytes[22] = 0x87; // Seven slots plus the native corruption flag.
        bytes[23] = 0x72;
        bytes[24] = 0x3c; // T8 affix 572, in the native nibble/byte encoding.
        bytes[25] = 255;
        var record = new UniqueRegularSealRecord { AffixId = 572 };
        Assert.True(record.IsEncodedAffix(bytes, 23));
        Assert.False(record.IsEncodedAffix(bytes, 24));
        Assert.False(record.IsEncodedAffix(bytes, 22));
        Assert.False(record.IsEncodedAffix(bytes, 44));
        Assert.False(record.IsEncodedAffix(bytes.Take(25).ToArray(), 23));
        Assert.False(new UniqueRegularSealRecord { AffixId = 978 }.IsEncodedAffix(bytes, 23));
        bytes[0] = 7;
        Assert.False(record.IsEncodedAffix(bytes, 23));
        bytes[0] = 6;
        bytes[22] = 0x80; // No affix slots, even if trailing bytes are present.
        Assert.False(record.IsEncodedAffix(bytes, 23));
    }

    [Fact]
    public void LegendaryCorruptionMustFollowEveryOtherAffixIncludingRegularSeal()
    {
        int[] ids = { 643, 978, 348, 192, 98, 725, 572 };
        var seals = new[]
        {
            ForceDropSeal.Primordial,
            ForceDropSeal.Corruption,
            ForceDropSeal.Primordial,
            ForceDropSeal.Primordial,
            ForceDropSeal.Primordial,
            ForceDropSeal.Primordial,
            ForceDropSeal.Regular,
        };
        var order = ForceDropPackingOrder.UniqueCorruptionLastIndices(seals);
        Assert.Equal(new[] { 643, 348, 192, 98, 725, 572, 978 }, order.Select(i => ids[i]));
        Assert.Equal(572, ids[order[order.Count - 2]]);
        Assert.Throws<ArgumentException>(() =>
            ForceDropPackingOrder.UniqueCorruptionLastIndices(
                new[] { ForceDropSeal.Corruption, ForceDropSeal.Corruption }
            )
        );
    }

    [Theory]
    [InlineData(7)]
    [InlineData(8)]
    public void LegendaryThreeBitCountRejectsAnEighthSlotBeforeCreation(int rarity)
    {
        ResolvedForceDrop Request(int count) =>
            new(
                1,
                7,
                370,
                rarity,
                0,
                0,
                0,
                true,
                new int[3],
                new int[8],
                Enumerable
                    .Range(1, count)
                    .Select(id => new ResolvedForceDropAffix(id, 6, 255, ForceDropSeal.None)),
                Array.Empty<int>(),
                new ResolvedForceDropAffix(978, 6, 255, ForceDropSeal.Corruption),
                ForceDropMode.Illegal
            );
        Assert.Equal(6, Request(6).Affixes.Count);
        Assert.Contains(
            "seven affixes",
            Assert.Throws<ArgumentException>(() => Request(7)).Message
        );
    }

    [Fact]
    public void UploadedBaseSevenT8RegressionAllowsNativeCountFiveButDetectsAffixLoss()
    {
        int[] ids = { 715, 572, 192, 943, 50, 46, 428 };
        var affixes = ids.Select(
                (id, i) =>
                    new PackedForceDropAffix(
                        id,
                        7,
                        255,
                        i == 0 ? ForceDropSeal.Regular
                            : i == 1 ? ForceDropSeal.Corruption
                            : ForceDropSeal.Primordial,
                        0,
                        i == 0 || i >= 3 ? 1 : 0
                    )
            )
            .ToArray();
        ForceDropPackingSnapshot Build(int sockets, PackedForceDropAffix[] data) =>
            new(
                1,
                74,
                0,
                4,
                0,
                0,
                0,
                true,
                sockets,
                true,
                true,
                true,
                new[] { 255, 255, 255 },
                Array.Empty<int>(),
                data
            );
        var live = Build(5, affixes);
        Assert.Equal("", live.Difference(Build(0, affixes), true));
        Assert.NotEqual("", live.Difference(Build(0, affixes.Skip(1).ToArray()), true));
        Assert.Contains("Socket count", Build(3, affixes).IntegrityError(allowIllegalT8: true));
    }
}
