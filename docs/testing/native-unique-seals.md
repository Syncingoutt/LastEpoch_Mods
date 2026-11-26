# Native v6 unique seal repair

Evidence: supplied ItemNativeProbe.zip, GameAssembly SHA256
`D5F9FB458F0E9878E697A92B590A586A52FD77E645F8BF87BB4F2B74C6C56567`.

| Native method | RVA | Observed behavior |
| --- | --- | --- |
| ItemData.RebuildID | 0x1205E80 | Legendary affixes begin at byte 23 as three-byte ID/tier/roll records. Header byte 22 includes count and corruption. No regular-seal flag is written for rarity 9. |
| ItemData.setValuesFromSerialisation | 0x12127E0 | Legendary count is byte 22 masked with 7. It passes regular=false for every Legendary affix and corruption=true only on the last affix when the corruption flag is set. |
| ItemData.loadAffixFromSerialisation | 0x1211380 | Seal precedence is Regular, then Corruption, then automatic Primordial for zero-based tier 7, then None. The affix ID is the low nibble of the first byte shifted by 8, plus the following byte. |

## Change

Before native packing, a Legendary's corruption affix is placed last. Relative order of every other affix is preserved, including fixed unique modifiers.

An explicitly requested illegal unique regular seal is recorded in
`Mods/LastEpoch_Hud/IllegalUniqueSeals.json`, keyed by the complete native item ID.
Its record includes the unique ID, base/subtype and regular affix ID. Native decoding
receives regular=true only for that registered ID at a valid v6 Legendary affix
offset, and never on the native corruption slot. No positional guess or creation
toggle is used when restoring an existing item. Native saved bytes remain valid.

During creation, records are provisional. Only the final item is committed after
request verification and native pack/unpack comparison both pass. Existing known
items carry their record to a new native ID when rebuilt, provided the unique/base
identity and exactly one matching affix remain. Removing the affix invalidates the
association. Writes replace the JSON atomically; failures during new creation reject
the drop. Keep the JSON file with the mod's other persistent data.

The native three-bit Legendary count supports seven affixes total, including
exclusive unique modifiers. The picker and resolved request reject an eighth slot
before item construction.

The supplied Silk Robe base-item failure had seven unchanged affixes but a live
socket count of five. Native unchecked insertion can exclude both new regular and
corruption seals from that live count. Validation now accepts this exact count
shape; identity, tier, roll, special type, seal ownership and presence flags still
must survive the complete native round trip.

## Validation and game checks

Full .NET 6 source compile, available regression/interop/patch checks, and CSharpier
pass. Both new detours' first six native bytes were checked against the uploaded
code slices for unsafe short branches. Full native Cpp2IL checks still require the
complete game binary/metadata and run in the Windows test script.

Test a standard Unique with four affixes, a regular seal, a Primordial affix and
corruption at T7/T8. Repeat without corruption and with only a regular seal. Pick
up, equip, change area, turn Illegal mode off, and restart the game. Confirm the
regular seal survives and corruption stays on its chosen affix. Test the Silk
Robe seven-T8 setup from the screenshot. Retest set display/counting and tooltip
hover separately; the newest supplied log contained no Ground Tooltip trace.
