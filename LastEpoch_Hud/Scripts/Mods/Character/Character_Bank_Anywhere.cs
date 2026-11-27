using LastEpoch_Hud.Scripts.ModUI;
using LastEpoch_Hud.Scripts.ModUI.Keybind;
using LastEpoch_Hud.Scripts.ModUI.Shell.Host;
using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character;

[RegisterTypeInIl2Cpp]
public class Character_Bank_Anywhere : MonoBehaviour
{
    private static readonly KeyCodeCache _bankKey = new();

    public static Character_Bank_Anywhere instance { get; private set; }

    public Character_Bank_Anywhere(System.IntPtr ptr)
        : base(ptr) { }

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        KeyCode key = _bankKey.Get(ModSettings.KeyBinds.BankStashs.Value);
        if (key == KeyCode.None)
        {
            return;
        }
        if (key == HudMenuToggle.MenuKey)
        {
            return;
        }
        if (!Input.GetKeyDown(key))
        {
            return;
        }
        if (!Scenes.IsGameScene())
        {
            return;
        }
        if (Refs_Manager.game_uibase.IsNullOrDestroyed())
        {
            return;
        }
        ToggleStash();
    }

    private static void ToggleStash()
    {
        if (IsOpen())
        {
            Refs_Manager.game_uibase.closeStash(false);
            return;
        }
        Refs_Manager.game_uibase.openStash(true, false);
    }

    private static bool IsOpen()
    {
        bool result = false;
        if (!Refs_Manager.game_uibase.IsNullOrDestroyed())
        {
            result = Refs_Manager.game_uibase.IsStashOpen();
        }

        return result;
    }
}
