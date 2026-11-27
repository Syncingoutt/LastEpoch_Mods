using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>World &gt; Misc HUD page. Page, session totals label and pause button reset on HUD re-bind (Build).</summary>
internal static class WorldMiscPage
{
    private static HudFormPage page;
    private static UnityEngine.UI.Text sessionTotals;
    private static UnityEngine.UI.Button sessionPause;

    public static void Build(HudPageId pageId, GameObject parent, GameObject hud, Font font)
    {
        page = HudFormPage.Build(parent, hud, font, pageId);
        if (page == null)
            return;

        var dungeons = page.AddCard("Dungeons", "Dungeons");
        page.AddToggle(
            dungeons,
            "RevealObjectives",
            "Reveal Dungeon Objectives",
            () => ModSettings.DungeonReveal.Enabled.Value,
            ModSettings.DungeonReveal.Enabled.Set
        );
        page.AddToggle(
            dungeons,
            "EnterWithoutKey",
            "Enter Without Key",
            () => ModSettings.ScenesDungeons.EnterWithoutKey.Value,
            ModSettings.ScenesDungeons.EnterWithoutKey.Set
        );

        var misc = page.AddCard("Misc", "Misc");
        page.AddToggle(
            misc,
            "SafeTeleport",
            "Enable Safe Teleport",
            () => ModSettings.SafeTeleport.Enabled.Value,
            ModSettings.SafeTeleport.Enabled.Set
        );
        page.AddKeybind(misc, "SafeTeleportKey", "Teleport Key", ModSettings.SafeTeleport.Key);
        page.AddText(
            misc,
            "SafeTeleportNote",
            "End of Time waypoint required. Bind a key or modifier + key."
        );
        page.AddToggle(
            misc,
            "TravelAnywhere",
            "Enable Travel Anywhere",
            () => ModSettings.TravelAnywhere.Enabled.Value,
            value => ModSettings.TravelAnywhere.Enabled.Set(value)
        );

        var gains = page.AddCard("SessionGains", "Session Gains");
        sessionTotals = page.AddText(
            gains,
            "SessionTotals",
            Mods.UI.SessionGainCounters.Format(),
            124f
        );
        page.AddToggle(
            gains,
            "ShowSessionOverlay",
            "Show counter HUD",
            () => ModSettings.SessionStats.ShowOverlay.Value,
            value => ModSettings.SessionStats.ShowOverlay.Set(value)
        );
        sessionPause = page.AddButton(
            gains,
            "PauseSession",
            "Pause",
            () =>
            {
                Mods.UI.SessionGainCounters.SetPaused(!Mods.UI.SessionGainCounters.Session.Paused);
                RefreshSession();
            }
        );
        page.AddButton(
            gains,
            "ResetSession",
            "Reset",
            () =>
            {
                Mods.UI.SessionGainCounters.Reset();
                RefreshSession();
            }
        );
        page.AddButton(
            gains,
            "ResetCounterPosition",
            "Reset counter position",
            Mods.UI.SessionGainCounters.ResetPosition
        );
        page.AddText(gains, "DragCounterHint", "Hold Alt and drag the counter to move it.", 48f);

        var minimap = page.AddCard("Minimap", "Minimap");
        page.AddToggle(
            minimap,
            "MaxZoomOut",
            "Max Zoom Out",
            () => ModSettings.ScenesMinimap.MaxZoomOut.Value,
            ModSettings.ScenesMinimap.MaxZoomOut.Set
        );
        page.AddToggle(
            minimap,
            "RemoveFog",
            "Remove Fog Of War",
            () => ModSettings.ScenesMinimap.RemoveFogOfWar.Value,
            ModSettings.ScenesMinimap.RemoveFogOfWar.Set
        );
    }

    public static void Show()
    {
        page?.Show();
        RefreshSession();
    }

    public static void Hide() => page?.Hide();

    public static void Refresh()
    {
        page?.RefreshValues();
        RefreshSession();
    }

    public static void RefreshSession()
    {
        if (!sessionTotals.IsNullOrDestroyed())
            sessionTotals.text = Mods.UI.SessionGainCounters.Format();
        if (sessionPause.IsNullOrDestroyed())
            return;
        var text = sessionPause.GetComponentInChildren<UnityEngine.UI.Text>(true);
        if (!text.IsNullOrDestroyed())
            LocaleRegistry.Apply(
                text,
                Mods.UI.SessionGainCounters.Session.Paused ? "Resume" : "Pause"
            );
    }
}
