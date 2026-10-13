using System.Collections.Generic;

namespace LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings.Maps;

/// <summary>Legacy Scenes.Minimap fields mapped to the ScenesMinimap group.</summary>
public static class ScenesMinimapMap
{
    public const string Group = "ScenesMinimap";

    public static IReadOnlyList<LegacyFieldMapping> All { get; } =
        new LegacyFieldMapping[]
        {
            new("Scenes.Minimap.Enable_MaxZoomOut", Group, "MaxZoomOut", LegacySettingPart.Bool),
            new(
                "Scenes.Minimap.Enable_RemoveFogOfWar",
                Group,
                "RemoveFogOfWar",
                LegacySettingPart.Bool
            ),
        };
}
