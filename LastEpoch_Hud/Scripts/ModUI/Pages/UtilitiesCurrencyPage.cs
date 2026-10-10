using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>Utilities &gt; Currency HUD page. State resets on HUD re-bind (Build).</summary>
internal static class UtilitiesCurrencyPage
{
    private static HudActionCard page;

    public static void Build(HudPageId pageId, GameObject parent, Font font)
    {
        page = HudActionCard.Build(
            parent,
            font,
            pageId,
            "Currency",
            new List<HudActionCard.Definition>
            {
                Action(
                    "Runes",
                    "Add All Runes x99",
                    Hud_Manager.Content.Character.Cheats.AddRunes_Click
                ),
                Action(
                    "Glyphs",
                    "Add All Glyphs x99",
                    Hud_Manager.Content.Character.Cheats.AddGlyphs_Click
                ),
                Action(
                    "Affixes",
                    "Add All Affixes x10",
                    Hud_Manager.Content.Character.Cheats.AddAffixs_Click
                ),
                Action(
                    "AncientBones",
                    "Add Ancient Bones x10,000",
                    Hud_Manager.Content.Character.Cheats.AddAncientBones_Click
                ),
                Action(
                    "MemoryAmber",
                    "Add 10,000 Memory Amber",
                    Mods.Character.Character_MemoryAmber.Add10000
                ),
                Action(
                    "GoldMillion",
                    "Spawn 1,000,000 Gold",
                    Mods.Character.Character_Gold.SpawnMillion
                ),
                Action(
                    "SoulEmbers",
                    "Add 1,000 Soul Embers",
                    Hud_Manager.Content.Character.Data.AddSoulEmbers
                ),
            }
        );
    }

    public static void Show() => page?.Show();

    public static void Hide() => page?.Hide();

    private static HudActionCard.Definition Action(string id, string label, System.Action click) =>
        new()
        {
            Id = id,
            Label = label,
            Click = click,
        };
}
