using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>Skills &gt; QOL HUD page. State resets on HUD re-bind (Build).</summary>
internal static class SkillsQolPage
{
    private static HudFormPage page;

    public static void Build(HudPageId pageId, GameObject parent, GameObject hud, Font font)
    {
        page = HudFormPage.Build(parent, hud, font, pageId);
        if (page == null)
            return;
        var general = page.AddCard("General", "General");
        page.AddToggle(
            general,
            "UnlockAllSkills",
            "Unlock All Skills",
            () => ModSettings.SkillsGeneral.UnlockAllSkills.Value,
            ModSettings.SkillsGeneral.UnlockAllSkills.Set
        );

        var autoCast = page.AddCard("AutoCast", "Auto-Cast");
        page.AddText(
            autoCast,
            "Description",
            "Hold the modifier key and press the skill's normal keybind to toggle auto-cast on or off.",
            58f
        );
        page.AddKeybind(
            autoCast,
            "Modifier",
            "Modifier Key",
            ModSettings.SkillsAutoCast.ModifierKey,
            "Reset"
        );
        page.AddToggle(
            autoCast,
            "PauseZoneChange",
            "Pause Auto-Cast On Zone Change",
            () => ModSettings.SkillsAutoCast.PauseOnZoneChange.Value,
            ModSettings.SkillsAutoCast.PauseOnZoneChange.Set
        );
        page.AddToggle(
            autoCast,
            "PauseNonCombat",
            "Pause Auto-Cast In Non-Combat Zones",
            () => ModSettings.SkillsAutoCast.DisableInNonCombatZone.Value,
            ModSettings.SkillsAutoCast.DisableInNonCombatZone.Set
        );

        var movement = page.AddCard("MovementSkills", "Movement Skills");
        page.AddToggle(
            movement,
            "NoTarget",
            "No Target Required",
            () => ModSettings.SkillsMovement.NoTarget.Value,
            ModSettings.SkillsMovement.NoTarget.Set
        );
        page.AddToggle(
            movement,
            "Immune",
            "Immune During Movement",
            () => ModSettings.SkillsMovement.ImmuneDuringMovement.Value,
            ModSettings.SkillsMovement.ImmuneDuringMovement.Set
        );
        page.AddToggle(
            movement,
            "SimplePath",
            "Disable Simple Path Requirement",
            () => ModSettings.SkillsMovement.IgnoreSimplePath.Value,
            ModSettings.SkillsMovement.IgnoreSimplePath.Set
        );
    }

    public static void Show() => page?.Show();

    public static void Hide() => page?.Hide();

    public static void Refresh() => page?.RefreshValues();
}
