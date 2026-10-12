using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Items.Drop;

public class Items_Drop_Affixs
{
    private static bool CanRun()
    {
        if (Hud_Manager.IsPauseOpen() && ModUI.ForceDrop.ForceDropModel.Enabled)
        {
            return false;
        }
        if (!Scenes.IsGameScene())
        {
            return false;
        }
        return ModSettings.ItemsDrop.ForceUnique.Value
            || ModSettings.ItemsDrop.ForceSet.Value
            || ModSettings.ItemsDrop.ForceLegendary.Value
            || ModSettings.ItemsDrop.AffixCount.Min != 0
            || ModSettings.ItemsDrop.AffixCount.Max != 4
            || ModSettings.ItemsDrop.AffixTiers.Enabled
            || ModSettings.ItemsDrop.AffixValues.Enabled
            || ModSettings.ItemsDrop.ForceSeal.Value
            || ModSettings.ItemsDrop.SealTier.Enabled
            || ModSettings.ItemsDrop.SealValue.Enabled;
    }

    private static byte Roll(RangeSetting setting)
    {
        if (setting.Min == setting.Max)
        {
            return (byte)setting.Max;
        }
        return (byte)Random.RandomRange(setting.Min, setting.Max);
    }

    private static bool RollsAffixes()
    {
        return ModSettings.ItemsDrop.AffixTiers.Enabled
            || ModSettings.ItemsDrop.AffixValues.Enabled
            || ModSettings.ItemsDrop.SealTier.Enabled
            || ModSettings.ItemsDrop.SealValue.Enabled;
    }

    private static void RollAffix(ItemAffix aff)
    {
        if (aff.IsSealed)
        {
            RollSealed(aff);
            return;
        }
        if (ModSettings.ItemsDrop.AffixTiers.Enabled)
        {
            aff.affixTier = Roll(ModSettings.ItemsDrop.AffixTiers);
        }
        if (ModSettings.ItemsDrop.AffixValues.Enabled)
        {
            aff.affixRoll = Roll(ModSettings.ItemsDrop.AffixValues);
        }
    }

    private static void RollSealed(ItemAffix aff)
    {
        if (ModSettings.ItemsDrop.SealTier.Enabled)
        {
            aff.affixTier = Roll(ModSettings.ItemsDrop.SealTier);
        }
        if (ModSettings.ItemsDrop.SealValue.Enabled)
        {
            aff.affixRoll = Roll(ModSettings.ItemsDrop.SealValue);
        }
    }

    private static void RollEquipment(ItemDataUnpacked item)
    {
        if (item.isUnique() || item.isSet()) //Base item and Legendary only
        {
            return;
        }
        if (!RollsAffixes())
        {
            return;
        }
        foreach (ItemAffix aff in item.affixes)
        {
            RollAffix(aff);
        }
        item.RefreshIDAndValues();
    }

    private static void RollAffixValues(ItemDataUnpacked item)
    {
        if (!ModSettings.ItemsDrop.AffixValues.Enabled)
        {
            return;
        }
        foreach (ItemAffix aff in item.affixes)
        {
            aff.affixRoll = Roll(ModSettings.ItemsDrop.AffixValues);
        }
        item.RefreshIDAndValues();
    }

    [HarmonyPatch(typeof(GenerateItems), "DropItemAtPoint")]
    public class GenerateItems_DropItemAtPoint
    {
        [HarmonyPrefix]
        static void Prefix(
            GenerateItems __instance,
            ref ItemDataUnpacked __0,
            ref UnityEngine.Vector3 __1,
            int __2
        )
        {
            if (!CanRun() || __0.itemType >= 100)
            {
                return;
            }
            if (__0.itemType < 25)
            {
                RollEquipment(__0);
                return;
            }
            //Idols (25-33) and new items (have to check this) roll affix values only
            RollAffixValues(__0);
        }
    }
}
