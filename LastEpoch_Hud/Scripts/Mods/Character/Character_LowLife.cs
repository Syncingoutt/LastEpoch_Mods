using LastEpoch_Hud.Scripts.ModUI;
using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character;

[RegisterTypeInIl2Cpp]
public class Character_LowLife : MonoBehaviour
{
    public static Character_LowLife instance { get; private set; }

    public Character_LowLife(System.IntPtr ptr)
        : base(ptr) { }

    void Awake()
    {
        instance = this;
        if (Main.debug)
        {
            Main.logger_instance?.Msg("Character_LowLife : Awake");
        }
    }

    bool CanRun()
    {
        return Scenes.IsGameScene() && ModSettings.CharacterCheats.LowLife.Value;
    }

    void Update()
    {
        if (!Refs_Manager.player_stats.IsNullOrDestroyed())
        {
            if (CanRun())
            {
                if (!Refs_Manager.player_stats.atLowHealth)
                {
                    Refs_Manager.player_stats.atLowHealth = true;
                }
            }
            /*else
            {
                if (Hud_Manager.player_stats.atLowHealth) { Hud_Manager.player_stats.atLowHealth = false; }
            }*/
        }
    }
}
