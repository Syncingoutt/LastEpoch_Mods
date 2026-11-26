using LastEpoch_Hud.Scripts.Core.Items;

namespace LastEpoch_Hud.Tests.Core.Items;

public class GroundTooltipRulesTests
{
    [Fact]
    public void ActualGroundHoverSurvivesAnUnpromptedClose() =>
        Assert.True(
            GroundTooltipRules.KeepHoveredGroundTooltip(
                true,
                true,
                true,
                true,
                false,
                false,
                false,
                false
            )
        );

    [Theory]
    [InlineData(false, true, true, true, false, false, false, false)]
    [InlineData(true, false, true, true, false, false, false, false)]
    [InlineData(true, true, false, true, false, false, false, false)]
    [InlineData(true, true, true, false, false, false, false, false)]
    [InlineData(true, true, true, true, true, false, false, false)]
    [InlineData(true, true, true, true, false, true, false, false)]
    [InlineData(true, true, true, true, false, false, true, false)]
    [InlineData(true, true, true, true, false, false, false, true)]
    public void NativeCloseHandlesInvalidHoverReplacementClicksCancellationAndMenus(
        bool inGame,
        bool activeGround,
        bool validItem,
        bool pointerOnOpener,
        bool openingTooltip,
        bool mouseHeld,
        bool cancelPressed,
        bool worldBlocked
    ) =>
        Assert.False(
            GroundTooltipRules.KeepHoveredGroundTooltip(
                inGame,
                activeGround,
                validItem,
                pointerOnOpener,
                openingTooltip,
                mouseHeld,
                cancelPressed,
                worldBlocked
            )
        );
}
