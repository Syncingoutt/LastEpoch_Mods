# Exulis native corruption pool

Exulis was excluded by the fixed-modifier adapter because its specific pool contains tiered Corrupted affixes instead of FakeUniqueMod affixes. A supported native pool with exactly two Corrupted selections now uses two distinct native-pool selectors with independent T1–T7 (bounded by each definition) and roll-percent controls. Fixed FakeUniqueMod pools retain T1/max-roll behavior.

Resolved requests copy tiers and rolls independently; creation and post-packing verification compare those exact values. Pool membership, duplicate exclusion, prefix ordering and the seven-total-affix storage limit remain enforced. The item stays corrupted; FP, LP and Weaver's Will normalize to zero. These native pool modifiers are distinct from the normal single corruption-seal action. The normal corruption picker is disabled for this inherently corrupted selection.

Run scripts/Test-Exulis.ps1 to fetch the test branch, build, test, back up and transfer the DLL. Verify Exulis with and without ordinary affixes, both distinct pool selections, minimum/maximum and unequal rolls, equipment stats, zone changes, pickup/drop/re-pickup and relog. Retest Unsated Rage, Withstand the Elements, and ordinary corruption on another unique. Native runtime behavior still needs game testing.

Illegal sealed regression: two native pool modifiers plus four ordinary affixes plus one regular seal is valid (seven total). Adding a Primordial slot makes eight and must be rejected; remove one ordinary selection to use both seals. Validation feedback now sits directly beneath Drop/Reset with a disabled-button prompt and a counted capacity warning. The native storage limit is unchanged.
