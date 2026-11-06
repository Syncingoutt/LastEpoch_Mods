using LastEpoch_Hud.Scripts.ModUI;
using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character;

[RegisterTypeInIl2Cpp]
public class Character_GodMode : MonoBehaviour
{
    public static Character_GodMode instance { get; private set; }

    public Character_GodMode(System.IntPtr ptr)
        : base(ptr) { }

    void Awake()
    {
        instance = this;
    }

    bool CanRun()
    {
        return Scenes.IsGameScene() && ModSettings.CharacterCheats.GodMode.Value;
    }

    void Update()
    {
        if (Refs_Manager.player_health.IsNullOrDestroyed())
        {
            return;
        }
        SetInvulnerable(CanRun());
    }

    // Update stops with the object, so the game values are restored here.
    private void OnDisable()
    {
        if (Refs_Manager.player_health.IsNullOrDestroyed())
        {
            return;
        }
        SetInvulnerable(false);
    }

    private static void SetInvulnerable(bool invulnerable)
    {
        if (Refs_Manager.player_health.damageable == invulnerable)
        {
            Refs_Manager.player_health.damageable = !invulnerable;
        }
        if (Refs_Manager.player_health.canDie == invulnerable)
        {
            Refs_Manager.player_health.canDie = !invulnerable;
        }
    }
}
