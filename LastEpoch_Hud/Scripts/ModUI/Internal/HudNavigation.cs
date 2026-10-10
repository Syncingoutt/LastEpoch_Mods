using System;
using System.Collections.Generic;
using LastEpoch_Hud.Scripts.Core.CustomItems;
using LastEpoch_Hud.Scripts.ModUI.Pages;

namespace LastEpoch_Hud.Scripts.ModUI;

internal enum HudArea
{
    Character,
    Items,
    World,
    Skills,
    ForceDrop,
}

internal sealed class HudPanelUse
{
    public readonly string Panel;
    public readonly string[] VisibleRows;
    public readonly string[] VisibleChildren;
    public readonly string Title;

    public HudPanelUse(
        string panel,
        string[] visibleRows = null,
        string[] visibleChildren = null,
        string title = null
    )
    {
        Panel = panel;
        VisibleRows = visibleRows;
        VisibleChildren = visibleChildren;
        Title = title;
    }
}

internal sealed class HudPageDefinition
{
    public readonly string Id;
    public readonly string Label;
    public readonly HudArea[] Areas;
    public readonly HudPanelUse[] Panels;

    public HudPageDefinition(string id, string label, HudArea[] areas, params HudPanelUse[] panels)
    {
        Id = id;
        Label = label;
        Areas = areas;
        Panels = panels;
    }
}

internal sealed class HudSectionDefinition
{
    public readonly string Id;
    public readonly string Label;
    public readonly bool Accordion;
    public readonly HudPageDefinition[] Pages;

    public HudSectionDefinition(
        string id,
        string label,
        bool accordion,
        params HudPageDefinition[] pages
    )
    {
        Id = id;
        Label = label;
        Accordion = accordion;
        Pages = pages;
    }
}

internal static class HudPanelIds
{
    public const string CharacterCheats = "character.cheats";
    public const string CharacterData = "character.data";
    public const string CharacterFactions = "character.factions";
    public const string CharacterBuffs = "character.buffs";
    public const string ItemsDrop = "items.drop";
    public const string ItemsUtility = "items.utility";
    public const string ItemsCrafting = "items.crafting";
    public const string Camera = "world.camera";
    public const string WorldMisc = "world.misc";
    public const string Difficulty = "world.difficulty";
    public const string Monoliths = "world.monoliths";
    public const string SkillsGeneral = "skills.general";
    public const string Companions = "skills.companions";
    public const string Minions = "skills.minions";
    public const string ForceDrop = "force-drop";
}

internal sealed class HudPanelSource
{
    public readonly string Id;
    public readonly string Path;

    public HudPanelSource(string id, string path)
    {
        Id = id;
        Path = path;
    }
}

internal static class HudNavigation
{
    private static readonly Dictionary<string, string> SearchPageIds = new(StringComparer.Ordinal)
    {
        ["Utilities_Character"] = "character.main",
        ["Utilities_Multipliers"] = "character.multipliers",
        ["Utilities_Currency"] = "character.currency",
        ["Utilities_Buffs"] = "character.buffs",
        ["Utilities_QOL"] = "character.qol",
        ["Items_Drop"] = "items.drop",
        ["Items_CraftingSlot"] = "items.crafting",
        [Items_CustomItems.RootName] = Items_CustomItems.PageId,
        ["World_Difficulty"] = "world.difficulty",
        ["World_Monoliths"] = "world.monoliths",
        ["World_Misc"] = "world.misc",
        ["World_Camera"] = "world.camera",
        ["Skills_Minions"] = "skills.minions",
        ["Skills_Companions"] = "skills.companions",
        ["Skills_Summon"] = "skills.summon",
        ["Skills_QOL"] = "skills.qol",
    };

    // Paths are relative to Hud/Content. Keeping them beside the page definitions
    // makes this file the only map between legacy prefab names and the new UI.
    public static readonly HudPanelSource[] PanelSources =
    {
        new(HudPanelIds.CharacterCheats, "Character_Content/Character_Cheats"),
        new(HudPanelIds.CharacterData, "Character_Content/Character_Data"),
        new(HudPanelIds.CharacterFactions, "Character_Content/Character_Factions"),
        new(HudPanelIds.CharacterBuffs, "Character_Content/Character_Buffs"),
        new(HudPanelIds.ItemsDrop, "Items_Content/Items_Drop"),
        new(HudPanelIds.ItemsUtility, "Items_Content/Items_Pickup"),
        new(HudPanelIds.ItemsCrafting, "Items_Content/Items_Craft"),
        new(HudPanelIds.Camera, "Scenes_Content/Camera"),
        new(HudPanelIds.WorldMisc, "Scenes_Content/Center"),
        new(HudPanelIds.Difficulty, "Scenes_Content/Difficulty"),
        new(HudPanelIds.Monoliths, "Scenes_Content/Monoliths"),
        new(HudPanelIds.SkillsGeneral, "Skill_Tree_Content/Left"),
        new(HudPanelIds.Companions, "Skill_Tree_Content/Center"),
        new(HudPanelIds.Minions, "Skill_Tree_Content/Right"),
        new(HudPanelIds.ForceDrop, "Old_ForceDrop_Content"),
    };

    private static readonly string[] CharacterRows =
    {
        "GodMode",
        "ForceLowLife",
        "AllowChoosingBlessings",
        "UnlockAllIdolsSlots",
        "AutoPotions",
        "TwoHandeWithShield",
        "WaypointsUnlock",
        "Btn_Character_Cheats_LevelOnce",
        "Btn_Character_Cheats_LevelToMax",
        "Btn_Character_Cheats_CompleteQuest",
        "Btn_Character_Cheats_Masterie",
        "Btn_Character_Cheats_DicoverAllBlessings",
    };

    private static readonly string[] CharacterSkillRows =
    {
        "RemoveManaCost",
        "RemoveChannelCost",
        "ManaRegenWhenChanneling",
        "DontStopWhenOOM",
        "NoCooldown",
        "UnlockAllSkills",
        "RemoveNodeRequirements",
        "SpecializationSlots",
        "SkillLevel",
        "PassivePoints",
    };

    private static readonly string[] RequirementsChildren = { "Title_Req", "Items_Req_Content" };

    public static readonly HudSectionDefinition[] Sections =
    {
        new(
            "character",
            "Utilities",
            true,
            new HudPageDefinition(
                "character.main",
                "Character",
                new[] { HudArea.Character, HudArea.Items, HudArea.Skills },
                new HudPanelUse(HudPanelIds.CharacterCheats, CharacterRows, title: "Character"),
                new HudPanelUse(HudPanelIds.SkillsGeneral, CharacterSkillRows, title: "Skills"),
                new HudPanelUse(HudPanelIds.CharacterFactions),
                new HudPanelUse(HudPanelIds.ItemsUtility, visibleChildren: RequirementsChildren)
            ),
            new HudPageDefinition("character.multipliers", "Multipliers", new HudArea[0]),
            new HudPageDefinition("character.currency", "Currency", new HudArea[0]),
            new HudPageDefinition("character.buffs", "Buffs", new HudArea[0]),
            new HudPageDefinition("character.qol", "QOL", new HudArea[0])
        ),
        new(
            "items",
            "Items",
            true,
            new HudPageDefinition("items.drop", "Drop", new HudArea[0]),
            new HudPageDefinition("items.force-drop", "Force Drop", new HudArea[0]),
            new HudPageDefinition("items.crafting", "Crafting Slot", Array.Empty<HudArea>()),
            new HudPageDefinition(
                Items_CustomItems.PageId,
                CustomItemLocaleKeys.CustomItemsPage,
                Array.Empty<HudArea>()
            )
        ),
        new(
            "world",
            "World",
            true,
            new HudPageDefinition("world.difficulty", "Difficulty", new HudArea[0]),
            new HudPageDefinition("world.monoliths", "Monoliths", new HudArea[0]),
            new HudPageDefinition("world.misc", "Misc", new HudArea[0]),
            new HudPageDefinition("world.camera", "Camera", new HudArea[0])
        ),
        new(
            "skills",
            "Skills",
            true,
            new HudPageDefinition("skills.minions", "Minions", new HudArea[0]),
            new HudPageDefinition("skills.companions", "Companions", new HudArea[0]),
            new HudPageDefinition("skills.summon", "Summon", new HudArea[0]),
            new HudPageDefinition("skills.qol", "QOL", new HudArea[0])
        ),
    };

    public static string SearchPageId(string rootName) =>
        rootName != null && SearchPageIds.TryGetValue(rootName, out string pageId) ? pageId : null;

    public static bool TryGetPage(
        string pageId,
        out HudSectionDefinition section,
        out HudPageDefinition page
    )
    {
        foreach (var candidateSection in Sections)
        foreach (var candidatePage in candidateSection.Pages)
            if (string.Equals(candidatePage.Id, pageId, StringComparison.Ordinal))
            {
                section = candidateSection;
                page = candidatePage;
                return true;
            }
        section = null;
        page = null;
        return false;
    }
}
