using LastEpoch_Hud.Scripts.ModUI;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts;

[RegisterTypeInIl2Cpp]
public class Mods_Manager : MonoBehaviour
{
    public Mods_Manager(System.IntPtr ptr)
        : base(ptr) { }

    public static Mods_Manager instance { get; private set; }

    GameObject character_autopotion_obj = null;
    GameObject character_potionreplenishment_obj = null;
    GameObject character_blessings_obj = null;
    GameObject character_godmode_obj = null;
    GameObject character_lowlife_obj = null;
    GameObject character_bank_from_anywhere = null;
    GameObject character_permanentbuffs_obj = null;
    GameObject character_safetp_obj = null;
    GameObject character_twohanded_shield_obj = null;
    GameObject items_nbsocket_obj = null;
    GameObject items_autosell_timer_obj = null;
    GameObject items_headhunter_obj = null;
    GameObject items_mjolner_obj = null;
    GameObject items_sandsofsilk_obj = null;
    GameObject items_essentiasanguis_obj = null;
    GameObject items_temporalis_obj = null;

    //GameObject items_crafting_obj = null;
    GameObject monoliths_complete_objectives_obj = null;
    GameObject bank_quad_obj = null;
    GameObject damage_meter_obj = null;
    GameObject teleport_to_scene_obj = null;

    bool initialized = false;

    void Awake()
    {
        instance = this;
        Main.logger_instance?.Msg("Mods Manager : Initialize");
        Il2CppSystem.Collections.Generic.List<GameObject> Mods_Objects =
            new Il2CppSystem.Collections.Generic.List<GameObject>();

        character_autopotion_obj = Object.Instantiate(
            new GameObject { name = "Mod_Character_AutoPotion" },
            Vector3.zero,
            Quaternion.identity
        );
        character_autopotion_obj.active = false;
        character_autopotion_obj.AddComponent<Mods.Character.Character_AutoPotions>();
        Mods_Objects.Add(character_autopotion_obj);

        character_potionreplenishment_obj = Object.Instantiate(
            new GameObject { name = "Mod_Character_PotionReplenishment" },
            Vector3.zero,
            Quaternion.identity
        );
        character_potionreplenishment_obj.AddComponent<Mods.Character.Character_PotionReplenishment>();
        Mods_Objects.Add(character_potionreplenishment_obj);

        character_blessings_obj = Object.Instantiate(
            new GameObject { name = "Mod_Character_Blessings" },
            Vector3.zero,
            Quaternion.identity
        );
        character_blessings_obj.AddComponent<Mods.Character.Character_Blessings>();
        Mods_Objects.Add(character_blessings_obj);

        character_godmode_obj = Object.Instantiate(
            new GameObject { name = "Mod_Character_GodMode" },
            Vector3.zero,
            Quaternion.identity
        );
        character_godmode_obj.active = false;
        character_godmode_obj.AddComponent<Mods.Character.Character_GodMode>();
        Mods_Objects.Add(character_godmode_obj);

        character_lowlife_obj = Object.Instantiate(
            new GameObject { name = "Mod_Character_LowLife" },
            Vector3.zero,
            Quaternion.identity
        );
        character_lowlife_obj.active = false;
        character_lowlife_obj.AddComponent<Mods.Character.Character_LowLife>();
        Mods_Objects.Add(character_lowlife_obj);

        character_permanentbuffs_obj = Object.Instantiate(
            new GameObject { name = "Mod_Character_Buffs" },
            Vector3.zero,
            Quaternion.identity
        );
        character_permanentbuffs_obj.AddComponent<Mods.Character.Character_PermanentBuffs>();
        Mods_Objects.Add(character_permanentbuffs_obj);

        character_safetp_obj = Object.Instantiate(
            new GameObject { name = "Mod_Character_SafeTp" },
            Vector3.zero,
            Quaternion.identity
        );
        character_safetp_obj.AddComponent<Mods.Character.Character_TpSafe>();
        Mods_Objects.Add(character_safetp_obj);

        character_twohanded_shield_obj = Object.Instantiate(
            new GameObject { name = "Mod_Character_TwoHandedShield" },
            Vector3.zero,
            Quaternion.identity
        );
        character_twohanded_shield_obj.AddComponent<Mods.Character.Character_TwoHandedShield>();
        Mods_Objects.Add(character_twohanded_shield_obj);

        items_nbsocket_obj = Object.Instantiate(
            new GameObject { name = "Mod_Items_NbSocket" },
            Vector3.zero,
            Quaternion.identity
        );
        items_nbsocket_obj.AddComponent<Mods.Items.Drop.Items_SocketsNb>();
        Mods_Objects.Add(items_nbsocket_obj);

        items_autosell_timer_obj = Object.Instantiate(
            new GameObject { name = "Mod_Items_AutoStore_All10Sec" },
            Vector3.zero,
            Quaternion.identity
        );
        items_autosell_timer_obj.active = true;
        items_autosell_timer_obj.AddComponent<Mods.Items.Pickup.Items_AutoStore_WithTimer>();
        Mods_Objects.Add(items_autosell_timer_obj);

        items_headhunter_obj = Object.Instantiate(
            new GameObject { name = "Mod_Items_Headhunter" },
            Vector3.zero,
            Quaternion.identity
        );
        items_headhunter_obj.AddComponent<Mods.Items.CustomItems.Headhunter.Items_HeadHunter>();
        Mods_Objects.Add(items_headhunter_obj);

        items_mjolner_obj = Object.Instantiate(
            new GameObject { name = "Mod_Items_Mjolner" },
            Vector3.zero,
            Quaternion.identity
        );
        items_mjolner_obj.AddComponent<Mods.Items.CustomItems.Mjolner.Items_Mjolner>();
        Mods_Objects.Add(items_mjolner_obj);

        items_sandsofsilk_obj = Object.Instantiate(
            new GameObject { name = "Mod_Items_SandsOfSilk" },
            Vector3.zero,
            Quaternion.identity
        );
        items_sandsofsilk_obj.AddComponent<Mods.Items.CustomItems.SandsOfSilk.Items_SandsOfSilk>();
        Mods_Objects.Add(items_sandsofsilk_obj);

        items_essentiasanguis_obj = Object.Instantiate(
            new GameObject { name = "Mod_Items_EssentiaSanguis" },
            Vector3.zero,
            Quaternion.identity
        );
        items_essentiasanguis_obj.AddComponent<Mods.Items.CustomItems.EssentiaSanguis.Items_EssentiaSanguis>();
        Mods_Objects.Add(items_essentiasanguis_obj);

        GameObject magebloodObj = Object.Instantiate(
            new GameObject { name = "Mod_Items_Mageblood" },
            Vector3.zero,
            Quaternion.identity
        );
        magebloodObj.AddComponent<Mods.Items.CustomItems.Mageblood.Items_Mageblood>();
        Mods_Objects.Add(magebloodObj);

        GameObject customIconsObj = Object.Instantiate(
            new GameObject { name = "Mod_Items_CustomItemIcons" },
            Vector3.zero,
            Quaternion.identity
        );
        customIconsObj.AddComponent<Mods.Items.CustomItems.CustomItemIconUpdater>();
        Mods_Objects.Add(customIconsObj);

        items_temporalis_obj = Object.Instantiate(
            new GameObject { name = "Mod_Items_Temporalis" },
            Vector3.zero,
            Quaternion.identity
        );
        items_temporalis_obj.AddComponent<Mods.Items.Items_Temporalis>();
        Mods_Objects.Add(items_temporalis_obj);

        //items_crafting_obj = Object.Instantiate(new GameObject { name = "Mod_Items_Crafting" }, Vector3.zero, Quaternion.identity);
        //items_crafting_obj.AddComponent<Mods.Items.Items_Crafting>();
        //Mods_Objects.Add(items_crafting_obj);

        character_bank_from_anywhere = Object.Instantiate(
            new GameObject { name = "Mod_Bank_Anywhere" },
            Vector3.zero,
            Quaternion.identity
        );
        character_bank_from_anywhere.AddComponent<Mods.Character.Character_Bank_Anywhere>();
        Mods_Objects.Add(character_bank_from_anywhere);

        monoliths_complete_objectives_obj = Object.Instantiate(
            new GameObject { name = "Mod_Monoliths_Complete_Objectives" },
            Vector3.zero,
            Quaternion.identity
        );
        monoliths_complete_objectives_obj.AddComponent<Mods.Monoliths.Monoliths_CompleteObjective>();
        Mods_Objects.Add(monoliths_complete_objectives_obj);

        bank_quad_obj = Object.Instantiate(
            new GameObject { name = "mod_bank_quad" },
            Vector3.zero,
            Quaternion.identity
        );
        bank_quad_obj.AddComponent<Mods.Bank.Bank_Quad>();
        Mods_Objects.Add(bank_quad_obj);

        damage_meter_obj = Object.Instantiate(
            new GameObject { name = "mod_damage_meter" },
            Vector3.zero,
            Quaternion.identity
        );
        damage_meter_obj.AddComponent<Mods.UI.DamageMeter>();
        Mods_Objects.Add(damage_meter_obj);

        // A standalone worker keeps session statistics active even while the HUD is hidden.
        var sessionCounter = new GameObject { name = "Mod_SessionGainCounters" };
        sessionCounter.AddComponent<Mods.UI.SessionCounterRuntime>();
        Mods_Objects.Add(sessionCounter);

        teleport_to_scene_obj = Object.Instantiate(
            new GameObject { name = "Mod_Teleport_To_Scene" },
            Vector3.zero,
            Quaternion.identity
        );
        teleport_to_scene_obj.AddComponent<Mods.Teleport.Teleport_ToScene>();
        teleport_to_scene_obj.AddComponent<Mods.Teleport.TravelAnywhere>();
        Mods_Objects.Add(teleport_to_scene_obj);

        GameObject summon_options = new GameObject { name = "Mod_Summon_Options" };
        summon_options.AddComponent<Mods.Summon.Summon_Options>();
        Mods_Objects.Add(summon_options);

        GameObject skills_autocast = new GameObject { name = "Mod_Skills_AutoCast" };
        skills_autocast.AddComponent<Mods.Skills.Skills_AutoCast>();
        Mods_Objects.Add(skills_autocast);

        foreach (GameObject mod in Mods_Objects)
        {
            Object.DontDestroyOnLoad(mod);
        }
        Mods_Objects.Clear();

        initialized = true;
        Main.logger_instance?.Msg("Mods Manager : Mods initialized");
        Subscribe();
    }

    private void Subscribe()
    {
        ModSettings.CharacterCheats.GodMode.Changed += OnGodModeChanged;
        ModSettings.CharacterCheats.LowLife.Changed += OnLowLifeChanged;
        ModSettings.CharacterCheats.AutoPot.Changed += OnAutoPotChanged;
    }

    private void OnDestroy()
    {
        ModSettings.CharacterCheats.GodMode.Changed -= OnGodModeChanged;
        ModSettings.CharacterCheats.LowLife.Changed -= OnLowLifeChanged;
        ModSettings.CharacterCheats.AutoPot.Changed -= OnAutoPotChanged;
    }

    private void OnGodModeChanged(bool value)
    {
        if (!initialized)
        {
            return;
        }
        character_godmode_obj.SetActive(value);
    }

    private void OnLowLifeChanged(bool value)
    {
        if (!initialized)
        {
            return;
        }
        character_lowlife_obj.SetActive(value);
    }

    private void OnAutoPotChanged()
    {
        if (!initialized)
        {
            return;
        }
        character_autopotion_obj.SetActive(ModSettings.CharacterCheats.AutoPot.Enabled);
    }

    // Runs until the saved settings and the item list are ready, then applies them once.
    private void Update()
    {
        if (!initialized || !SaveManager.IsLoaded || Refs_Manager.item_list.IsNullOrDestroyed())
        {
            return;
        }
        ApplyStartup();
        enabled = false;
    }

    private void ApplyStartup()
    {
        character_godmode_obj.SetActive(ModSettings.CharacterCheats.GodMode.Value);
        character_lowlife_obj.SetActive(ModSettings.CharacterCheats.LowLife.Value);
        character_autopotion_obj.SetActive(ModSettings.CharacterCheats.AutoPot.Enabled);
        items_autosell_timer_obj.SetActive(true); // worker also services debounced AutoStore-on-drop
        Mods.Items.Requirements.Items_Update.Reqs(); // applies saved requirement removal
    }
}
