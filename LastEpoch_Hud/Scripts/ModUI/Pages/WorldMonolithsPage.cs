using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>World &gt; Monoliths HUD page. Page and the timeline editor reset on HUD re-bind (Build).</summary>
internal static class WorldMonolithsPage
{
    private static HudFormPage page;

    public static void Build(HudPageId pageId, GameObject parent, GameObject hud, Font font)
    {
        page = HudFormPage.Build(parent, hud, font, pageId);
        if (page == null)
            return;
        var card = page.AddCard("Monoliths", "Monoliths");
        AddSlider(
            card,
            "MaxStability",
            "Max Stability",
            SceneSliderLimits.MonolithMaxStability,
            ModSettings.ScenesMonoliths.MaxStability
        );
        AddToggle(
            card,
            "MaxStabilityOnStart",
            "Stability = Max Stability",
            ModSettings.ScenesMonoliths.MaxStabilityOnStart
        );
        AddSlider(
            card,
            "MobsDensity",
            "Mobs Density",
            SceneSliderLimits.MonolithMobsDensity,
            ModSettings.ScenesMonoliths.MobsDensity
        );
        AddSlider(
            card,
            "MobsDefeat",
            "Mobs Defeat",
            SceneSliderLimits.MonolithMobsDefeat,
            ModSettings.ScenesMonoliths.MobsDefeatOnStart
        );
        AddToggle(
            card,
            "ObjectiveReveal",
            "Objective Reveal",
            ModSettings.ScenesMonoliths.ObjectiveReveal
        );
        AddToggle(
            card,
            "CompleteObjective",
            "Complete Objective",
            ModSettings.ScenesMonoliths.CompleteObjective
        );
        page.AddText(
            card,
            "StartNote",
            "The options above must be set before entering a Monolith."
        );
        AddSlider(
            card,
            "BlessingSlots",
            "Blessing Slots Reward",
            SceneSliderLimits.MonolithBlessingSlots,
            ModSettings.ScenesMonoliths.BlessingSlots
        );
        AddToggle(
            card,
            "NoLoss",
            "No Stability Lost When You Die",
            ModSettings.ScenesMonoliths.NoLostWhenDie
        );
        AddToggle(
            card,
            "MaxOnChange",
            "Stability = Max Stability On Stability Change",
            ModSettings.ScenesMonoliths.MaxStabilityOnStabilityChanged
        );

        // Echo encounter: checked = guaranteed in every echo; unchecked = the game's own rate.
        var encounters = page.AddCard("EchoEncounters", "Echo Encounters");
        AddToggle(
            encounters,
            "AlwaysNemesis",
            "Always Spawn Nemesis",
            ModSettings.EchoEncounters.AlwaysNemesis
        );
        AddToggle(
            encounters,
            "AlwaysBloodRitual",
            "Always Spawn Blood Ritual",
            ModSettings.EchoEncounters.AlwaysBloodRitual
        );
        AddToggle(
            encounters,
            "AlwaysLostCache",
            "Always Spawn Lost Cache",
            ModSettings.EchoEncounters.AlwaysLostCache
        );
        AddToggle(
            encounters,
            "AlwaysRiftBeast",
            "Always Spawn Rift Beast",
            ModSettings.EchoEncounters.AlwaysRiftBeast
        );
        AddToggle(
            encounters,
            "AlwaysOmenWindow",
            "Always Spawn Omen Window",
            ModSettings.EchoEncounters.AlwaysOmenWindow
        );
        AddCount(
            encounters,
            "RunePrisonCount",
            "Rune Prisons Per Echo",
            ModSettings.EchoEncounters.RunePrisonCount
        );
        AddCount(
            encounters,
            "FavourCacheCount",
            "Favour Caches Per Echo",
            ModSettings.EchoEncounters.FavourCacheCount
        );
        page.AddText(
            encounters,
            "EncountersNote",
            "Applies from the next echo zone. Off = the game's own rates."
        );
        page.AddText(
            encounters,
            "EncountersSpotsWarning",
            "Warning: echoes have a limited number of encounter spots. High Rune Prison or Favour Cache counts can crowd out other spawns such as shrines, champions and Nemesis."
        );

        MonolithTimelineEditor.Build(page, page.AddCard("Timelines", "Timelines").Body, font);
    }

    public static void Show()
    {
        page?.Show();
        MonolithTimelineEditor.Reload();
    }

    public static void Hide() => page?.Hide();

    public static void Refresh()
    {
        page?.RefreshValues();
    }

    private static void AddToggle(
        HudFormPage.Card card,
        string id,
        string label,
        BoolSetting setting
    ) => page.AddToggle(card, id, label, () => setting.Value, setting.Set);

    private static void AddSlider(
        HudFormPage.Card card,
        string id,
        string label,
        SliderRange range,
        FloatSetting setting
    )
    {
        page.AddToggleSlider(
            card,
            id,
            label,
            string.Empty,
            range.Min,
            range.Max,
            range.WholeNumbers,
            () => setting.Enabled,
            setting.SetEnabled,
            () => setting.Value,
            setting.SetValue
        );
    }

    // Echo encounter count: off = the game's own rate; on = exactly 0-10 per echo zone.
    private static void AddCount(
        HudFormPage.Card card,
        string id,
        string label,
        FloatSetting setting
    ) =>
        page.AddToggleSlider(
            card,
            id,
            label,
            string.Empty,
            0f,
            10f,
            true,
            () => setting.Enabled,
            v => setting.SetEnabled(v),
            () => setting.Value,
            v => setting.SetValue(v)
        );
}
