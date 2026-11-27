using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.Core.ModUI.Sliders;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>Skills &gt; Minions HUD page. State resets on HUD re-bind (Build).</summary>
internal static class SkillsMinionsPage
{
    private static HudFormPage page;

    public static void Build(HudPageId pageId, GameObject parent, GameObject hud, Font font)
    {
        page = HudFormPage.Build(parent, hud, font, pageId);
        if (page == null)
            return;
        BuildSkeletons();
        BuildWraiths();
        BuildMages();
        BuildBoneGolems();
        BuildVolatileZombies();
        BuildDreadShades();
    }

    public static void Show() => page?.Show();

    public static void Hide() => page?.Hide();

    public static void Refresh() => page?.RefreshValues();

    private static void BuildSkeletons()
    {
        var c = page.AddCard("Skeletons", "Skeletons");
        AddSlider(
            c,
            "SkeletonPassive",
            "Additional Skeletons From Passives",
            SkillSliderLimits.SkeletonFromPassives,
            ModSettings.SkillsMinions.SkeletonFromPassives
        );
        AddSlider(
            c,
            "SkeletonTree",
            "Additional Skeletons From Skill Tree",
            SkillSliderLimits.SkeletonFromSkillTree,
            ModSettings.SkillsMinions.SkeletonFromSkillTree
        );
        AddSlider(
            c,
            "SkeletonPerCast",
            "Additional Skeletons Per Cast",
            SkillSliderLimits.SkeletonPerCast,
            ModSettings.SkillsMinions.SkeletonPerCast
        );
        AddSlider(
            c,
            "SkeletonResummon",
            "Chance To Resummon On Death",
            SkillSliderLimits.SkeletonResummonChance,
            ModSettings.SkillsMinions.SkeletonResummonChance
        );
        AddToggle(c, "ForceArcher", "Force Archer", ModSettings.SkillsMinions.SkeletonForceArcher);
        AddToggle(
            c,
            "ForceBrawler",
            "Force Brawler",
            ModSettings.SkillsMinions.SkeletonForceBrawler
        );
        AddToggle(
            c,
            "ForceWarrior",
            "Force Warrior",
            ModSettings.SkillsMinions.SkeletonForceWarrior
        );
    }

    private static void BuildWraiths()
    {
        var c = page.AddCard("Wraiths", "Wraiths");
        AddSlider(
            c,
            "WraithMax",
            "Additional Maximum Wraiths",
            SkillSliderLimits.WraithMaximum,
            ModSettings.SkillsMinions.WraithMaximum
        );
        AddSlider(
            c,
            "WraithDelay",
            "Delayed Wraiths",
            SkillSliderLimits.WraithDelayed,
            ModSettings.SkillsMinions.WraithDelayed
        );
        AddSlider(
            c,
            "WraithCastSpeed",
            "Increased Cast Speed",
            SkillSliderLimits.WraithCastSpeed,
            ModSettings.SkillsMinions.WraithCastSpeed
        );
        AddToggle(
            c,
            "WraithNoLimit",
            "Disable Limit To 2 Wraiths",
            ModSettings.SkillsMinions.WraithNoLimit
        );
        AddToggle(
            c,
            "WraithNoDecay",
            "Wraiths Do Not Decay",
            ModSettings.SkillsMinions.WraithNoDecay
        );
    }

    private static void BuildMages()
    {
        var c = page.AddCard("Mages", "Mage");
        AddSlider(
            c,
            "MagePassive",
            "Additional Mages From Passives",
            SkillSliderLimits.MageFromPassives,
            ModSettings.SkillsMinions.MageFromPassives
        );
        AddSlider(
            c,
            "MageItems",
            "Additional Mages From Items",
            SkillSliderLimits.MageFromItems,
            ModSettings.SkillsMinions.MageFromItems
        );
        AddSlider(
            c,
            "MageTree",
            "Additional Mages From Skill Tree",
            SkillSliderLimits.MageFromSkillTree,
            ModSettings.SkillsMinions.MageFromSkillTree
        );
        AddSlider(
            c,
            "MagePerCast",
            "Additional Mages Per Cast",
            SkillSliderLimits.MagePerCast,
            ModSettings.SkillsMinions.MagePerCast
        );
        AddSlider(
            c,
            "MageProjectile",
            "Chance For Two Extra Projectiles",
            SkillSliderLimits.MageProjectileChance,
            ModSettings.SkillsMinions.MageProjectileChance
        );
        AddToggle(
            c,
            "Cryomancer",
            "Force Cryomancer",
            ModSettings.SkillsMinions.MageForceCryomancer
        );
        AddToggle(
            c,
            "DeathKnight",
            "Force Death Knight",
            ModSettings.SkillsMinions.MageForceDeathKnight
        );
        AddToggle(
            c,
            "Pyromancer",
            "Force Pyromancer",
            ModSettings.SkillsMinions.MageForcePyromancer
        );
    }

    private static void BuildBoneGolems()
    {
        var c = page.AddCard("BoneGolem", "Bone Golem");
        AddSlider(
            c,
            "GolemsPerSkeletons",
            "Added Golems Per 4 Skeletons",
            SkillSliderLimits.BoneGolemPerSkeletons,
            ModSettings.SkillsMinions.BoneGolemPerSkeletons
        );
        AddSlider(
            c,
            "Resurrect",
            "Self Resurrect Chance",
            SkillSliderLimits.BoneGolemResurrectChance,
            ModSettings.SkillsMinions.BoneGolemResurrectChance
        );
        AddSlider(
            c,
            "FireAura",
            "Increased Fire Aura Area",
            SkillSliderLimits.BoneGolemFireAura,
            ModSettings.SkillsMinions.BoneGolemFireAura
        );
        AddSlider(
            c,
            "ArmorAura",
            "Undead Armor Aura",
            SkillSliderLimits.BoneGolemArmorAura,
            ModSettings.SkillsMinions.BoneGolemArmorAura
        );
        AddSlider(
            c,
            "MoveSpeedAura",
            "Undead Movement Speed Aura",
            SkillSliderLimits.BoneGolemMoveSpeedAura,
            ModSettings.SkillsMinions.BoneGolemMoveSpeedAura
        );
        AddSlider(
            c,
            "MoveSpeed",
            "Increased Movement Speed",
            SkillSliderLimits.BoneGolemMoveSpeed,
            ModSettings.SkillsMinions.BoneGolemMoveSpeed
        );
        AddToggle(c, "Twins", "Twins", ModSettings.SkillsMinions.BoneGolemTwins);
        AddToggle(c, "Slam", "Has Slam Attack", ModSettings.SkillsMinions.BoneGolemSlamAttack);
    }

    private static void BuildVolatileZombies()
    {
        var c = page.AddCard("VolatileZombies", "Volatile Zombies");
        AddSlider(
            c,
            "ZombieMinionDeath",
            "Chance To Cast From Minion Death",
            SkillSliderLimits.ZombieMinionDeathChance,
            ModSettings.SkillsMinions.ZombieMinionDeathChance
        );
        AddSlider(
            c,
            "ZombieInfernalShade",
            "Infernal Shade Chance On Death",
            SkillSliderLimits.ZombieInfernalShadeChance,
            ModSettings.SkillsMinions.ZombieInfernalShadeChance
        );
        AddSlider(
            c,
            "ZombieMarrowShards",
            "Marrow Shards Chance On Death",
            SkillSliderLimits.ZombieMarrowShardsChance,
            ModSettings.SkillsMinions.ZombieMarrowShardsChance
        );
    }

    private static void BuildDreadShades()
    {
        var c = page.AddCard("DreadShades", "Dread Shades");
        AddSlider(
            c,
            "DreadDuration",
            "Duration",
            SkillSliderLimits.DreadShadeDuration,
            ModSettings.SkillsMinions.DreadShadeDuration
        );
        AddSlider(
            c,
            "DreadMax",
            "Maximum Dread Shades",
            SkillSliderLimits.DreadShadeMaximum,
            ModSettings.SkillsMinions.DreadShadeMaximum
        );
        AddSlider(
            c,
            "DreadDecay",
            "Reduced Decay",
            SkillSliderLimits.DreadShadeDecay,
            ModSettings.SkillsMinions.DreadShadeDecay
        );
        AddSlider(
            c,
            "DreadRadius",
            "Radius",
            SkillSliderLimits.DreadShadeRadius,
            ModSettings.SkillsMinions.DreadShadeRadius
        );
        AddToggle(
            c,
            "DreadNoLimit",
            "Disable Summon Limit",
            ModSettings.SkillsMinions.DreadShadeNoLimit
        );
        AddToggle(
            c,
            "DreadNoDrain",
            "Disable Health Drain",
            ModSettings.SkillsMinions.DreadShadeNoHealthDrain
        );
    }

    private static void AddToggle(
        HudFormPage.Card card,
        string id,
        string label,
        BoolSetting setting
    )
    {
        page.AddToggle(card, id, label, () => setting.Value, setting.Set);
    }

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
}
