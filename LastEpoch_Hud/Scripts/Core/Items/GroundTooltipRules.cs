namespace LastEpoch_Hud.Scripts.Core.Items;

public static class GroundTooltipRules
{
    public static bool KeepHoveredGroundTooltip(
        bool inGame,
        bool activeGroundItem,
        bool validItem,
        bool pointerOnOpener,
        bool openingTooltip,
        bool mouseHeld,
        bool cancelPressed,
        bool worldBlocked
    ) =>
        inGame
        && activeGroundItem
        && validItem
        && pointerOnOpener
        && !openingTooltip
        && !mouseHeld
        && !cancelPressed
        && !worldBlocked;
}
