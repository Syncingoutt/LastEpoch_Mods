namespace LastEpoch_Hud.Tests.Core.ModUI.LegacySettings.Maps;

public static class SkillsMinionsRows
{
    private const string Root = "Skills.Minions.";

    public static TheoryData<string, string, string> Bools { get; } =
        new()
        {
            { Root + "Skeletons", "Enable_forceArcher", "SkeletonForceArcher" },
            { Root + "Skeletons", "Enable_forceBrawler", "SkeletonForceBrawler" },
            { Root + "Skeletons", "Enable_forceRogue", "SkeletonForceRogue" },
            { Root + "Skeletons", "Enable_forceWarrior", "SkeletonForceWarrior" },
            { Root + "Wraiths", "Enable_limitedTo2Wraiths", "WraithNoLimit" },
            { Root + "Wraiths", "Enable_wraithsDoNotDecay", "WraithNoDecay" },
            { Root + "Mages", "Enable_singleSummon", "MageNoSingleSummon" },
            { Root + "Mages", "Enable_forceCryomancer", "MageForceCryomancer" },
            { Root + "Mages", "Enable_forceDeathKnight", "MageForceDeathKnight" },
            { Root + "Mages", "Enable_forcePyromancer", "MageForcePyromancer" },
            { Root + "Mages", "Enable_doubleProjectiles", "MageDoubleProjectiles" },
            { Root + "BoneGolems", "Enable_twins", "BoneGolemTwins" },
            { Root + "BoneGolems", "Enable_hasSlamAttack", "BoneGolemSlamAttack" },
            { Root + "DreadShades", "Enable_DisableLimit", "DreadShadeNoLimit" },
            { Root + "DreadShades", "Enable_DisableHealthDrain", "DreadShadeNoHealthDrain" },
        };

    public static TheoryData<string, string, string, string> Floats { get; } =
        new()
        {
            {
                Root + "Skeletons",
                "Enable_additionalSkeletonsFromPassives",
                "additionalSkeletonsFromPassives",
                "SkeletonFromPassives"
            },
            {
                Root + "Skeletons",
                "Enable_additionalSkeletonsFromSkillTree",
                "additionalSkeletonsFromSkillTree",
                "SkeletonFromSkillTree"
            },
            {
                Root + "Skeletons",
                "Enable_additionalSkeletonsPerCast",
                "additionalSkeletonsPerCast",
                "SkeletonPerCast"
            },
            {
                Root + "Skeletons",
                "Enable_chanceToResummonOnDeath",
                "chanceToResummonOnDeath",
                "SkeletonResummonChance"
            },
            {
                Root + "Wraiths",
                "Enable_additionalMaxWraiths",
                "additionalMaxWraiths",
                "WraithMaximum"
            },
            { Root + "Wraiths", "Enable_delayedWraiths", "delayedWraiths", "WraithDelayed" },
            {
                Root + "Wraiths",
                "Enable_increasedCastSpeed",
                "increasedCastSpeed",
                "WraithCastSpeed"
            },
            {
                Root + "Mages",
                "Enable_additionalSkeletonsFromPassives",
                "additionalSkeletonsFromPassives",
                "MageFromPassives"
            },
            {
                Root + "Mages",
                "Enable_additionalSkeletonsFromItems",
                "additionalSkeletonsFromItems",
                "MageFromItems"
            },
            {
                Root + "Mages",
                "Enable_additionalSkeletonsFromSkillTree",
                "additionalSkeletonsFromSkillTree",
                "MageFromSkillTree"
            },
            {
                Root + "Mages",
                "Enable_additionalSkeletonsPerCast",
                "additionalSkeletonsPerCast",
                "MagePerCast"
            },
            {
                Root + "Mages",
                "Enable_chanceForTwoExtraProjectiles",
                "chanceForTwoExtraProjectiles",
                "MageProjectileChance"
            },
            {
                Root + "BoneGolems",
                "Enable_addedGolemsPer4Skeletons",
                "addedGolemsPer4Skeletons",
                "BoneGolemPerSkeletons"
            },
            {
                Root + "BoneGolems",
                "Enable_selfResurrectChance",
                "selfResurrectChance",
                "BoneGolemResurrectChance"
            },
            {
                Root + "BoneGolems",
                "Enable_increasedFireAuraArea",
                "increasedFireAuraArea",
                "BoneGolemFireAura"
            },
            {
                Root + "BoneGolems",
                "Enable_undeadArmorAura",
                "undeadArmorAura",
                "BoneGolemArmorAura"
            },
            {
                Root + "BoneGolems",
                "Enable_undeadMovespeedAura",
                "undeadMovespeedAura",
                "BoneGolemMoveSpeedAura"
            },
            {
                Root + "BoneGolems",
                "Enable_increasedMoveSpeed",
                "increasedMoveSpeed",
                "BoneGolemMoveSpeed"
            },
            {
                Root + "VolatileZombies",
                "Enable_chanceToCastFromMinionDeath",
                "chanceToCastFromMinionDeath",
                "ZombieMinionDeathChance"
            },
            {
                Root + "VolatileZombies",
                "Enable_chanceToCastInfernalShadeOnDeath",
                "chanceToCastInfernalShadeOnDeath",
                "ZombieInfernalShadeChance"
            },
            {
                Root + "VolatileZombies",
                "Enable_chanceToCastMarrowShardsOnDeath",
                "chanceToCastMarrowShardsOnDeath",
                "ZombieMarrowShardsChance"
            },
            { Root + "DreadShades", "Enable_Duration", "Duration", "DreadShadeDuration" },
            { Root + "DreadShades", "Enable_Max", "max", "DreadShadeMaximum" },
            { Root + "DreadShades", "Enable_ReduceDecay", "decay", "DreadShadeDecay" },
            { Root + "DreadShades", "Enable_Radius", "radius", "DreadShadeRadius" },
        };

    public static TheoryData<string, string, string> FloatValues { get; } = ToValues();

    private static TheoryData<string, string, string> ToValues()
    {
        TheoryData<string, string, string> values = new();
        foreach (ITheoryDataRow row in Floats)
        {
            object[] data = row.GetData();
            values.Add((string)data[0], (string)data[2], (string)data[3]);
        }
        return values;
    }
}
