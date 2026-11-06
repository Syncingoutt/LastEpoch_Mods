using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>Skills &gt; Summon HUD page. State resets on HUD re-bind (Build).</summary>
internal static class SkillsSummonPage
{
    private static HudFormPage page;

    public static void Build(HudPageId pageId, GameObject parent, GameObject hud, Font font)
    {
        page = HudFormPage.Build(parent, hud, font, pageId);
        if (page == null)
            return;
        var card = page.AddCard("Summon", "Summon");
        page.AddToggle(
            card,
            "GodMode",
            "Summon God Mode",
            () => ModSettings.Summon.GodMode.Value,
            ModSettings.Summon.GodMode.Set
        );
        page.AddToggle(
            card,
            "Forever",
            "Forever",
            () => ModSettings.Summon.Forever.Value,
            ModSettings.Summon.Forever.Set
        );
        page.AddToggle(
            card,
            "DontCollide",
            "Don't Collide",
            () => ModSettings.Summon.DontCollide.Value,
            ModSettings.Summon.DontCollide.Set
        );
    }

    public static void Show() => page?.Show();

    public static void Hide() => page?.Hide();

    public static void Refresh() => page?.RefreshValues();
}
