using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.Core.ForceDrop;
using Newtonsoft.Json;

namespace LastEpoch_Hud.Scripts.ModUI.ForceDrop;

internal static class UniqueRegularSealExtension
{
    static readonly object gate = new();
    static Dictionary<string, UniqueRegularSealRecord> saved = new(StringComparer.Ordinal);
    static bool loaded;
    static bool dirty;
    static Exception loadFailure;

    [ThreadStatic]
    static CreationScope creation;

    public sealed class CreationScope : IDisposable
    {
        readonly CreationScope parent;
        internal IntPtr Pointer;
        internal UniqueRegularSealRecord Record;
        internal readonly Dictionary<string, UniqueRegularSealRecord> Pending = new(
            StringComparer.Ordinal
        );

        internal CreationScope()
        {
            parent = creation;
            creation = this;
        }

        public void Commit(ItemData item)
        {
            if (Record == null)
                return;
            string key = Key(item);
            if (key == null || !Pending.TryGetValue(key, out var record) || !Matches(item, record))
                throw new InvalidOperationException(
                    "The illegal unique seal could not be associated with its final saved ID; no item was dropped."
                );
            Save(key, record);
        }

        public void Dispose()
        {
            creation = parent;
        }
    }

    public static CreationScope Begin() => new();

    public static void Track(ItemData item, ResolvedForceDrop request)
    {
        if (creation == null || !request.IsIllegal || request.Rarity < 7)
            return;
        foreach (var selected in request.Affixes)
            if (selected.Seal == ForceDropSeal.Regular)
            {
                Load();
                if (loadFailure != null)
                    throw new InvalidOperationException(
                        "Illegal unique seal records could not be loaded; no item was dropped.",
                        loadFailure
                    );
                creation.Pointer = item.Pointer;
                creation.Record = new UniqueRegularSealRecord
                {
                    UniqueId = item.uniqueID,
                    ItemType = item.itemType,
                    SubType = item.subType,
                    AffixId = selected.Id,
                };
                return;
            }
    }

    static bool Matches(ItemData item, UniqueRegularSealRecord record)
    {
        if (
            record == null
            || item.IsNullOrDestroyed()
            || item.rarity != 9
            || item.affixes.IsNullOrDestroyed()
        )
            return false;
        var ids = new List<int>();
        foreach (var affix in item.affixes)
            if (!affix.IsNullOrDestroyed())
                ids.Add(affix.affixId);
        return record.Matches(item.uniqueID, item.itemType, item.subType, ids);
    }

    static string Key(ItemData item)
    {
        if (item.IsNullOrDestroyed() || item.id.IsNullOrDestroyed() || item.id.Length < 23)
            return null;
        var bytes = new byte[item.id.Length];
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] = item.id[i];
        return Convert.ToBase64String(bytes);
    }

    static string FilePath()
    {
        string folder =
            !Save_Manager.instance.IsNullOrDestroyed()
            && !string.IsNullOrEmpty(Save_Manager.instance.path)
                ? Save_Manager.instance.path
                : Path.Combine(
                    MelonLoader.Utils.MelonEnvironment.GameRootDirectory,
                    "Mods",
                    Main.mod_name
                );
        return Path.Combine(folder, "IllegalUniqueSeals.json");
    }

    static void Load()
    {
        lock (gate)
        {
            if (loaded)
                return;
            loaded = true;
            try
            {
                if (File.Exists(FilePath()))
                    saved =
                        JsonConvert.DeserializeObject<Dictionary<string, UniqueRegularSealRecord>>(
                            File.ReadAllText(FilePath())
                        )
                        ?? throw new InvalidDataException(
                            "IllegalUniqueSeals.json contains no record dictionary."
                        );
            }
            catch (Exception ex)
            {
                loadFailure = ex;
                Main.logger_instance?.Warning("Illegal unique seal load failed: " + ex.Message);
            }
        }
    }

    static UniqueRegularSealRecord Find(ItemData item, bool requireAffix = true)
    {
        if (item.IsNullOrDestroyed() || item.rarity != 9)
            return null;
        bool Valid(UniqueRegularSealRecord record) =>
            record != null
            && (
                requireAffix
                    ? Matches(item, record)
                    : record.MatchesIdentity(item.uniqueID, item.itemType, item.subType)
            );
        if (creation != null && creation.Pointer == item.Pointer && Valid(creation.Record))
            return creation.Record;
        string key = Key(item);
        if (key == null)
            return null;
        if (
            creation != null
            && creation.Pending.TryGetValue(key, out var pending)
            && Valid(pending)
        )
            return pending;
        Load();
        lock (gate)
            return saved.TryGetValue(key, out var record) && Valid(record) ? record : null;
    }

    static void Save(string key, UniqueRegularSealRecord record)
    {
        Load();
        lock (gate)
        {
            if (loadFailure != null)
                throw new IOException(
                    "Illegal unique seal records could not be loaded.",
                    loadFailure
                );
            if (
                !dirty
                && saved.TryGetValue(key, out var existing)
                && existing != null
                && existing.AffixId == record.AffixId
                && existing.UniqueId == record.UniqueId
                && existing.ItemType == record.ItemType
                && existing.SubType == record.SubType
            )
                return;
            var next = new Dictionary<string, UniqueRegularSealRecord>(
                saved,
                StringComparer.Ordinal
            )
            {
                [key] = record,
            };
            string path = FilePath();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path + ".tmp", JsonConvert.SerializeObject(next));
            File.Move(path + ".tmp", path, true);
            saved = next;
            dirty = false;
        }
    }

    // Native v6 Legendary decoding passes regular=false for every affix.
    // Supply the flag only for the explicitly registered saved item and ID.
    [HarmonyPatch(typeof(ItemData), nameof(ItemData.loadAffixFromSerialisation))]
    static class Decode
    {
        [HarmonyPrefix]
        static void Prefix(ItemData __instance, byte __0, ref bool __2, bool __3, out bool __state)
        {
            __state = false;
            if (__3 || __instance.IsNullOrDestroyed() || __instance.id.IsNullOrDestroyed())
                return;
            var record = Find(__instance, false);
            if (record == null)
                return;
            var bytes = new byte[__instance.id.Length];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = __instance.id[i];
            if (!record.IsEncodedAffix(bytes, __0))
                return;
            __2 = true;
            __state = true;
        }

        [HarmonyPostfix]
        static void Postfix(ItemData __instance, bool __state)
        {
            if (__state)
                __instance.hasSealedRegularAffix = true;
        }
    }

    [HarmonyPatch(typeof(ItemData), nameof(ItemData.RebuildID))]
    static class Encode
    {
        [HarmonyPrefix]
        static void Prefix(ItemData __instance, out UniqueRegularSealRecord __state)
        {
            __state = Find(__instance);
            UniqueVariantAdapter.MoveUniqueCorruptionLast(__instance);
        }

        [HarmonyPostfix]
        static void Postfix(ItemData __instance, UniqueRegularSealRecord __state)
        {
            if (__state == null || !Matches(__instance, __state))
                return;
            string key = Key(__instance);
            if (key == null)
                return;
            if (creation != null && __state == creation.Record)
                creation.Pending[key] = __state;
            else
            {
                try
                {
                    Save(key, __state);
                }
                catch (Exception ex)
                {
                    lock (gate)
                    {
                        saved[key] = __state;
                        dirty = true;
                    }
                    Main.logger_instance?.Warning("Illegal unique seal save failed: " + ex.Message);
                }
            }
        }
    }
}
