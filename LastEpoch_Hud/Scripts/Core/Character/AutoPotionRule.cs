using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Scripts.Core.Character;

/// <summary>Decides when auto potion drinks.</summary>
public static class AutoPotionRule
{
    public static bool ShouldDrink(float currentHealth, float maxHealth, float threshold)
    {
        if (maxHealth <= 0f)
        {
            return false;
        }
        int healthPercent = (int)(currentHealth / maxHealth * 100f);
        int thresholdPercent = (int)(threshold / CharacterSliderLimits.AutoPotion.Max * 100f);
        return healthPercent < thresholdPercent;
    }
}
