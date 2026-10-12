using LastEpoch_Hud.Scripts.Core.Character;
using LastEpoch_Hud.Scripts.ModUI;
using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character;

[RegisterTypeInIl2Cpp]
public class Character_AutoPotions : MonoBehaviour
{
    public static Character_AutoPotions instance { get; private set; }

    public Character_AutoPotions(System.IntPtr ptr)
        : base(ptr) { }

    void Awake()
    {
        instance = this;
    }

    float nextPotionTime = 0f;

    void Update()
    {
        if (!ModSettings.CharacterCheats.AutoPot.Enabled)
        {
            return;
        }
        if (UnityEngine.Time.unscaledTime < nextPotionTime)
        {
            return;
        }
        if (
            !Scenes.IsGameScene()
            || Refs_Manager.player_health.IsNullOrDestroyed()
            || Refs_Manager.health_potion.IsNullOrDestroyed()
        )
        {
            return;
        }
        if (
            !AutoPotionRule.ShouldDrink(
                Refs_Manager.player_health.currentHealth,
                Refs_Manager.player_health.maxHealth,
                ModSettings.CharacterCheats.AutoPot.Value
            )
        )
        {
            return;
        }
        nextPotionTime = UnityEngine.Time.unscaledTime + 0.35f;
        Refs_Manager.health_potion.UsePotion();
    }
}
