using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;
using MelonLoader;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.Mods.Character;

[RegisterTypeInIl2Cpp]
public class Character_PotionReplenishment : MonoBehaviour
{
    public static Character_PotionReplenishment instance { get; private set; }

    public Character_PotionReplenishment(System.IntPtr ptr)
        : base(ptr) { }

    public bool mod_enable = false; //Set to true if you want this mod to run

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (!ModSettings.ModsNotInHud.PotionReplenishment.Value)
        {
            return;
        }
        if (!Scenes.IsGameScene())
        {
            return;
        }
        HealthPotion potion = Refs_Manager.health_potion;
        if (potion.IsNullOrDestroyed())
        {
            return;
        }
        if (potion.currentCharges >= potion.maxCharges)
        {
            return;
        }
        potion.currentCharges = potion.maxCharges;
    }
}
