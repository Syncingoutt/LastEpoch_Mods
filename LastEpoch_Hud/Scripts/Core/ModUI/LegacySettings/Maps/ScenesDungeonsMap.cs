using System.Collections.Generic;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Scenes.Dungeons fields mapped to the ScenesDungeons group.</summary>
public static class ScenesDungeonsMap
{
    public const string Group = "ScenesDungeons";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } =
        new LegacyFieldMapping[]
        {
            new(
                "Scenes.Dungeons.Enable_EnterWithoutKey",
                Group,
                "EnterWithoutKey",
                LegacySettingPart.Bool
            ),
        };
}
