using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;

namespace LastEpoch_Hud.Tests.Core.ModUI.Sliders;

public sealed class BuffPercentTests
{
    [Theory]
    [InlineData(BuffPercentScale.Fraction, 1f, 100f)]
    [InlineData(BuffPercentScale.Fraction, 0.05f, 5f)]
    [InlineData(BuffPercentScale.Direct, 37f, 37f)]
    [InlineData(BuffPercentScale.CriticalChance, 0f, 0f)]
    [InlineData(BuffPercentScale.CriticalChance, 0.5f, 51f)]
    [InlineData(BuffPercentScale.CriticalChance, 0.95f, 96f)]
    public void ToPercent_ConvertsStoredToPage(BuffPercentScale scale, float stored, float expected)
    {
        Assert.Equal(expected, BuffPercent.ToPercent(scale, stored), 3);
    }

    [Theory]
    [InlineData(BuffPercentScale.Fraction, 250f, 0f, 20f, 2.5f)]
    [InlineData(BuffPercentScale.Fraction, 5000f, 0f, 20f, 20f)]
    [InlineData(BuffPercentScale.Direct, 37f, 0f, 255f, 37f)]
    [InlineData(BuffPercentScale.CriticalChance, 51f, 0f, 0.95f, 0.5f)]
    [InlineData(BuffPercentScale.CriticalChance, 1f, 0f, 0.95f, 0f)]
    [InlineData(BuffPercentScale.CriticalChance, 0f, 0f, 0.95f, 0f)]
    [InlineData(BuffPercentScale.CriticalChance, 200f, 0f, 0.95f, 0.95f)]
    public void ToStored_ConvertsPageToStoredWithinLimits(
        BuffPercentScale scale,
        float percent,
        float min,
        float max,
        float expected
    )
    {
        Assert.Equal(expected, BuffPercent.ToStored(scale, percent, min, max), 3);
    }

    [Theory]
    [InlineData(nameof(CharacterSliderLimits.BuffMoveSpeed), BuffPercentScale.Fraction, 2000f)]
    [InlineData(nameof(CharacterSliderLimits.BuffDamage), BuffPercentScale.Fraction, 25500f)]
    [InlineData(nameof(CharacterSliderLimits.BuffAreaOfEffect), BuffPercentScale.Fraction, 10000f)]
    [InlineData(nameof(CharacterSliderLimits.BuffAttackSpeed), BuffPercentScale.Fraction, 1000f)]
    [InlineData(nameof(CharacterSliderLimits.BuffCastSpeed), BuffPercentScale.Fraction, 1000f)]
    [InlineData(nameof(CharacterSliderLimits.BuffHealthRegen), BuffPercentScale.Fraction, 1000f)]
    [InlineData(nameof(CharacterSliderLimits.BuffManaRegen), BuffPercentScale.Fraction, 1000f)]
    [InlineData(
        nameof(CharacterSliderLimits.BuffCriticalMultiplier),
        BuffPercentScale.Fraction,
        25500f
    )]
    [InlineData(
        nameof(CharacterSliderLimits.BuffCriticalChance),
        BuffPercentScale.CriticalChance,
        96f
    )]
    [InlineData(nameof(CharacterSliderLimits.BuffStrength), BuffPercentScale.Direct, 255f)]
    [InlineData(nameof(CharacterSliderLimits.BuffIntelligence), BuffPercentScale.Direct, 255f)]
    [InlineData(nameof(CharacterSliderLimits.BuffDexterity), BuffPercentScale.Direct, 255f)]
    [InlineData(nameof(CharacterSliderLimits.BuffVitality), BuffPercentScale.Direct, 255f)]
    [InlineData(nameof(CharacterSliderLimits.BuffAttunement), BuffPercentScale.Direct, 255f)]
    public void PageMaximum_EqualsFormerInlineMaximum(
        string field,
        BuffPercentScale scale,
        float expected
    )
    {
        SliderRange? range = SliderLimitsReflection.Find(typeof(CharacterSliderLimits), field);

        Assert.True(range.HasValue, $"Limits field missing: {field}");
        Assert.Equal(expected, BuffPercent.ToPercent(scale, range.Value.Max), 3);
    }
}
