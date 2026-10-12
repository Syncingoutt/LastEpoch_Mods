using HarmonyLib;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Skills;

public class Skills_Use
{
    [HarmonyPatch(typeof(CharacterMutator), "OnAbilityUse")]
    public class OnAbilityUse
    {
        [HarmonyPrefix]
        static void Prefix(
            CharacterMutator __instance,
            AbilityInfo __0,
            ref AbilityMutator __1,
            float __2,
            UnityEngine.Vector3 __3,
            bool __4
        )
        {
            if ((Scenes.IsGameScene()) && (!__1.IsNullOrDestroyed()))
            {
                //Ability
                Ability ability = null;
                try
                {
                    ability = __1.getAbility();
                }
                catch
                {
                    Main.logger_instance?.Error("OnAbilityUse Prefix : Can't get Ability");
                }

                if (!ability.IsNullOrDestroyed())
                {
                    if (ModSettings.SkillsGeneral.RemoveChannelCost.Value)
                    {
                        ability.channelCost = 0f;
                    }
                    if (ModSettings.SkillsGeneral.RemoveManaCost.Value)
                    {
                        ability.manaCost = 0f;
                        ability.minimumManaCost = 0f;
                        ability.manaCostPerDistance = 0f;
                    }
                    if (ModSettings.SkillsGeneral.ManaRegenWhileChanneling.Value)
                    {
                        ability.noManaRegenWhileChanneling = false;
                    }
                    if (ModSettings.SkillsGeneral.ContinueWhenOutOfMana.Value)
                    {
                        ability.stopWhenOutOfMana = false;
                    }
                    if (
                        (ability.moveOrAttackCompatible)
                        && (ability.moveOrAttackFallback == Ability.MoveOrAttackFallback.Move)
                    )
                    {
                        if (ModSettings.SkillsMovement.NoTarget.Value)
                        {
                            ability.playerRequiresTarget = false;
                            ability.requiredEnemyTargetMustBeAccessible = false;
                        }
                        if (ModSettings.SkillsMovement.ImmuneDuringMovement.Value)
                        {
                            ability.immuneDuringMovement = true;
                            ability.cannotDieDuringMovement = true;
                        }
                        if (ModSettings.SkillsMovement.IgnoreSimplePath.Value)
                        {
                            ability.limitRangeForPlayers = false;
                            ability.requireSimplePath = false;
                        }
                    }
                }

                //Mutators
                Il2CppSystem.Type il2cpp_type = null;
                try
                {
                    il2cpp_type = __1.GetIl2CppType();
                }
                catch
                {
                    Main.logger_instance?.Error("OnAbilityUse Prefix : Can't get Mutator type");
                }

                if (!il2cpp_type.IsNullOrDestroyed())
                {
                    //Use Switch(il2cpp_type.ToString()) instead of if for better result (== is bad)

                    //ErasingStrike : Fix ManaCost
                    if (il2cpp_type.ToString() == "ErasingStrikeMutator")
                    {
                        if (ModSettings.SkillsGeneral.RemoveManaCost.Value)
                        {
                            var ab = __1.TryCast<ErasingStrikeMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                ab.addedManaCost = 0f;
                                ab.increasedManaCost = 0f;
                                if (!ab.abyssalEchoes.IsNullOrDestroyed())
                                {
                                    ab.abyssalEchoes.manaCost = 0f;
                                } //AbyssalEchoes
                                if (!ab.erasingStrikeHit.IsNullOrDestroyed())
                                {
                                    ab.erasingStrikeHit.manaCost = 0f;
                                } //Hit
                            }
                        }
                    }

                    //Holy Aura : Fix ManaCost
                    if (il2cpp_type.ToString() == "HolyAuraMutator")
                    {
                        if (ModSettings.SkillsGeneral.RemoveManaCost.Value)
                        {
                            var ab = __1.TryCast<HolyAuraMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                ab.ability.manaCost = 0f;
                                ab.ability.minimumManaCost = 0f;
                                ab.ability.manaCostPerDistance = 0f;
                            }
                        }
                    }

                    //Warpath : Fix Channel Cost
                    if (il2cpp_type.ToString() == "WarpathMutator")
                    {
                        if (ModSettings.SkillsGeneral.RemoveChannelCost.Value)
                        {
                            var ab = __1.TryCast<WarpathMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                ab.addedChannelCost = 0f;
                                ab.addedChannelCostPerSecond = 0f;
                            }
                        }
                    }

                    //Smite : Fix ManaCost
                    if (il2cpp_type.ToString() == "SmiteMutator")
                    {
                        if (ModSettings.SkillsGeneral.RemoveManaCost.Value)
                        {
                            var ab = __1.TryCast<SmiteMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                ab.addedManaCost = 0f;
                                ab.increasedManaCost = 0f;
                            }
                        }
                    }

                    //Judgement : Fix ManaCost
                    if (il2cpp_type.ToString() == "JudgementMutator")
                    {
                        if (ModSettings.SkillsGeneral.RemoveManaCost.Value)
                        {
                            var ab = __1.TryCast<JudgementMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                ab.addedManaCost = 0f;
                                ab.increasedManaCost = 0f;
                                ab.percentCurrentManaConsumed = 0f;
                            }
                        }
                    }
                    if (il2cpp_type.ToString() == "JudgementAoEMutator")
                    {
                        if (ModSettings.SkillsGeneral.RemoveManaCost.Value)
                        {
                            var ab = __1.TryCast<JudgementAoEMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                ab.increasedManaCost = 0f;
                            }
                        }
                    }

                    //Sigil of Hope : Fix ManaCost
                    if (il2cpp_type.ToString() == "SigilsOfHopeMutator")
                    {
                        if (ModSettings.SkillsGeneral.RemoveManaCost.Value)
                        {
                            var ab = __1.TryCast<SigilsOfHopeMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                ab.addedManaCost = 0f;
                                ab.increasedManaCost = 0f;
                            }
                        }
                    }

                    //Meteor : Fix ManaCost
                    if (il2cpp_type.ToString() == "MeteorMutator")
                    {
                        if (ModSettings.SkillsGeneral.RemoveManaCost.Value)
                        {
                            var ab = __1.TryCast<MeteorMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                ab.addedManaCost = 0f;
                            }
                        }
                    }
                    //Companions
                    else if (il2cpp_type.ToString() == "SummonWolfMutator")
                    {
                        if (ModSettings.SkillsCompanions.WolfSummonLimit.Enabled)
                        {
                            var ab = __1.TryCast<SummonWolfMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                ab.wolfLimit = (int)
                                    ModSettings.SkillsCompanions.WolfSummonLimit.Value;
                            }
                        }
                    }
                    else if (il2cpp_type.ToString() == "SummonScorpionMutator")
                    {
                        if (ModSettings.SkillsCompanions.ScorpionBabyQuantity.Enabled)
                        {
                            var ab = __1.TryCast<SummonScorpionMutator>();
                            if (!ab.IsNullOrDestroyed())
                            {
                                int babies = (int)
                                    ModSettings.SkillsCompanions.ScorpionBabyQuantity.Value;
                                ab.babyScorpionQuantity = babies;
                                ab.babyScorpionsToSpawnOnAbilityActivation = babies;
                                //ab.increasedBabySpawnRate = 1;
                            }
                        }
                    }
                    //Minions
                    else if (il2cpp_type.ToString() == "SummonSkeletonMutator")
                    {
                        var ab = __1.TryCast<SummonSkeletonMutator>();
                        if (!ab.IsNullOrDestroyed())
                        {
                            if (ModSettings.SkillsMinions.SkeletonFromPassives.Enabled)
                            {
                                ab.additionalSkeletonsFromPassives = (int)
                                    ModSettings.SkillsMinions.SkeletonFromPassives.Value;
                                ab.onlySummonOneWarrior = false;
                            }
                            if (ModSettings.SkillsMinions.SkeletonFromSkillTree.Enabled)
                            {
                                ab.additionalSkeletonsFromSkillTree = (int)
                                    ModSettings.SkillsMinions.SkeletonFromSkillTree.Value;
                                ab.onlySummonOneWarrior = false;
                            }
                            if (ModSettings.SkillsMinions.SkeletonPerCast.Enabled)
                            {
                                ab.additionalSkeletonsPerCast = (int)
                                    ModSettings.SkillsMinions.SkeletonPerCast.Value;
                            }
                            if (ModSettings.SkillsMinions.SkeletonResummonChance.Enabled)
                            {
                                ab.chanceToResummonOnDeath = ModSettings
                                    .SkillsMinions
                                    .SkeletonResummonChance
                                    .Value;
                            }
                            if (ModSettings.SkillsMinions.SkeletonForceArcher.Value)
                            {
                                ab.cannotSummonArchers = false;
                                ab.cannotSummonWarriors = true;
                                ab.canSummonRogues = false;

                                ab.forceBrawler = false;
                                ab.forceRogue = false;
                                ab.forceWarrior = false;
                                ab.forceArcher = true;
                            }
                            if (ModSettings.SkillsMinions.SkeletonForceBrawler.Value)
                            {
                                ab.cannotSummonArchers = true;
                                ab.cannotSummonWarriors = true;
                                ab.canSummonRogues = false;

                                ab.forceRogue = false;
                                ab.forceWarrior = false;
                                ab.forceArcher = false;
                                ab.forceBrawler = true;
                            }
                            if (ModSettings.SkillsMinions.SkeletonForceRogue.Value)
                            {
                                ab.cannotSummonArchers = true;
                                ab.cannotSummonWarriors = true;
                                ab.canSummonRogues = true;

                                ab.forceWarrior = false;
                                ab.forceArcher = false;
                                ab.forceBrawler = false;
                                ab.forceRogue = true;
                            }
                            if (ModSettings.SkillsMinions.SkeletonForceWarrior.Value)
                            {
                                ab.cannotSummonArchers = true;
                                ab.cannotSummonWarriors = false;
                                ab.canSummonRogues = false;

                                ab.forceArcher = false;
                                ab.forceBrawler = false;
                                ab.forceRogue = false;
                                ab.forceWarrior = true;
                            }
                        }
                    }
                    else if (il2cpp_type.ToString() == "SummonWraithMutator")
                    {
                        var ab = __1.TryCast<SummonWraithMutator>();
                        if (!ab.IsNullOrDestroyed())
                        {
                            if (ModSettings.SkillsMinions.WraithMaximum.Enabled)
                            {
                                ab.additionalMaxWraiths = (int)
                                    ModSettings.SkillsMinions.WraithMaximum.Value;
                            }
                            if (ModSettings.SkillsMinions.WraithDelayed.Enabled)
                            {
                                ab.delayedWraiths = (int)
                                    ModSettings.SkillsMinions.WraithDelayed.Value; //Wraiths per cast
                            }
                            if (ModSettings.SkillsMinions.WraithNoLimit.Value)
                            {
                                ab.limitedTo2Wraiths = false;
                            }
                            if (ModSettings.SkillsMinions.WraithNoDecay.Value)
                            {
                                ab.wraithsDoNotDecay = true;
                            }
                            if (ModSettings.SkillsMinions.WraithCastSpeed.Enabled)
                            {
                                ab.increasedCastSpeed = ModSettings
                                    .SkillsMinions
                                    .WraithCastSpeed
                                    .Value;
                            }
                        }
                    }
                    else if (il2cpp_type.ToString() == "SummonMageMutator")
                    {
                        var ab = __1.TryCast<SummonMageMutator>();
                        if (!ab.IsNullOrDestroyed())
                        {
                            if (ModSettings.SkillsMinions.MageFromPassives.Enabled)
                            {
                                ab.additionalSkeletonsFromPassives = (int)
                                    ModSettings.SkillsMinions.MageFromPassives.Value;
                            }
                            if (ModSettings.SkillsMinions.MageFromSkillTree.Enabled)
                            {
                                ab.additionalSkeletonsFromSkillTree = (int)
                                    ModSettings.SkillsMinions.MageFromSkillTree.Value;
                            }
                            if (ModSettings.SkillsMinions.MagePerCast.Enabled)
                            {
                                ab.additionalSkeletonsPerCast = (int)
                                    ModSettings.SkillsMinions.MagePerCast.Value;
                            }
                            if (ModSettings.SkillsMinions.MageNoSingleSummon.Value)
                            {
                                ab.singleSummon = false;
                            }
                            if (ModSettings.SkillsMinions.MageForceCryomancer.Value)
                            {
                                ab.forceDeathKnight = false;
                                ab.forcePyromancer = false;
                                ab.forceNoCryo = false;
                                ab.forceCryomancer = true;
                            }
                            if (ModSettings.SkillsMinions.MageForceDeathKnight.Value)
                            {
                                ab.forcePyromancer = false;
                                ab.forceCryomancer = false;
                                ab.forceDeathKnight = true;
                            }
                            if (ModSettings.SkillsMinions.MageForcePyromancer.Value)
                            {
                                ab.forceCryomancer = false;
                                ab.forceDeathKnight = false;
                                ab.forceNoPyro = false;
                                ab.forcePyromancer = true;
                            }
                            if (ModSettings.SkillsMinions.MageProjectileChance.Enabled)
                            {
                                ab.chanceForTwoExtraProjectiles = ModSettings
                                    .SkillsMinions
                                    .MageProjectileChance
                                    .Value;
                            }
                            if (ModSettings.SkillsMinions.MageDoubleProjectiles.Value)
                            {
                                ab.doubleProjectiles = true;
                            }
                            //ability_mutator.TryCast<SummonMageMutator>().additionalWarlords = 50;
                        }
                    }
                    else if (il2cpp_type.ToString() == "SummonBoneGolemMutator")
                    {
                        var ab = __1.TryCast<SummonBoneGolemMutator>();
                        if (!ab.IsNullOrDestroyed())
                        {
                            if (ModSettings.SkillsMinions.BoneGolemFireAura.Enabled)
                            {
                                ab.increasedFireAuraArea = ModSettings
                                    .SkillsMinions
                                    .BoneGolemFireAura
                                    .Value;
                            }
                            if (ModSettings.SkillsMinions.BoneGolemTwins.Value)
                            {
                                ab.twins = true;
                            }
                            if (ModSettings.SkillsMinions.BoneGolemArmorAura.Enabled)
                            {
                                ab.undeadArmorAura = ModSettings
                                    .SkillsMinions
                                    .BoneGolemArmorAura
                                    .Value;
                            }
                            if (ModSettings.SkillsMinions.BoneGolemMoveSpeedAura.Enabled)
                            {
                                ab.undeadMovespeedAura = ModSettings
                                    .SkillsMinions
                                    .BoneGolemMoveSpeedAura
                                    .Value;
                            }
                        }
                    }
                    else if (il2cpp_type.ToString() == "SummonVolatileZombieMutator")
                    {
                        var ab = __1.TryCast<SummonVolatileZombieMutator>();
                        if (!ab.IsNullOrDestroyed())
                        {
                            if (ModSettings.SkillsMinions.ZombieMinionDeathChance.Enabled)
                            {
                                ab.chanceToCastFromMinionDeath = ModSettings
                                    .SkillsMinions
                                    .ZombieMinionDeathChance
                                    .Value;
                            }
                            if (ModSettings.SkillsMinions.ZombieInfernalShadeChance.Enabled)
                            {
                                ab.chanceToCastInfernalShadeOnDeath = ModSettings
                                    .SkillsMinions
                                    .ZombieInfernalShadeChance
                                    .Value;
                            }
                            if (ModSettings.SkillsMinions.ZombieMarrowShardsChance.Enabled)
                            {
                                ab.chanceToCastMarrowShardsOnDeath = ModSettings
                                    .SkillsMinions
                                    .ZombieMarrowShardsChance
                                    .Value;
                            }
                        }
                    }
                    else if (il2cpp_type.ToString() == "DreadShadeMutator")
                    {
                        var ab = __1.TryCast<DreadShadeMutator>();
                        if (!ab.IsNullOrDestroyed())
                        {
                            if (ModSettings.SkillsMinions.DreadShadeNoLimit.Value)
                            {
                                ab.limitTo1DreadShade = false;
                            }
                            if (ModSettings.SkillsMinions.DreadShadeDuration.Enabled)
                            {
                                ab.increasedDuration = ModSettings
                                    .SkillsMinions
                                    .DreadShadeDuration
                                    .Value;
                            }
                            if (ModSettings.SkillsMinions.DreadShadeNoHealthDrain.Value)
                            {
                                ab.noHealthDrain = true;
                            }
                            if (ModSettings.SkillsMinions.DreadShadeMaximum.Enabled)
                            {
                                ab.addedMaxShades = (int)
                                    ModSettings.SkillsMinions.DreadShadeMaximum.Value;
                            }
                            if (ModSettings.SkillsMinions.DreadShadeDecay.Enabled)
                            {
                                ab.reducedDecayRate = ModSettings
                                    .SkillsMinions
                                    .DreadShadeDecay
                                    .Value;
                            }
                            if (ModSettings.SkillsMinions.DreadShadeRadius.Enabled)
                            {
                                ab.increasedRadius = ModSettings
                                    .SkillsMinions
                                    .DreadShadeRadius
                                    .Value;
                            }
                        }
                    }
                    else if (il2cpp_type.ToString() == "FlameWardMutator")
                    {
                        var ab = __1.TryCast<FlameWardMutator>();
                        if (!ab.IsNullOrDestroyed())
                        {
                            if (ModSettings.SkillsGeneral.RemoveManaCost.Value)
                            {
                                ab.addedManaCost = 0f;
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPostfix]
        static void PostFix(
            CharacterMutator __instance,
            AbilityInfo __0,
            ref AbilityMutator __1,
            float __2,
            UnityEngine.Vector3 __3,
            bool __4
        )
        {
            if ((Scenes.IsGameScene()) && (!__1.IsNullOrDestroyed()))
            {
                if (ModSettings.SkillsGeneral.RemoveCooldown.Value)
                {
                    __1.RemoveCooldown();
                }
            }
        }
    }

    //Fix ManaCost for ErasingStrike on Echo
    [HarmonyPatch(typeof(AbilityCoroutineHandler), "CreateVoidKnightEchoAfterDelay")]
    public class AbilityCoroutineHandler_CreateVoidKnightEchoAfterDelay
    {
        [HarmonyPrefix]
        //static void Prefix(float __0, Il2Cpp.Actor __1, UnityEngine.Vector3 __2, UnityEngine.Vector3 __3, Il2Cpp.AbilityAnimation __4, int __5, Il2Cpp.AbilityObjectConstructor __6, Il2Cpp.Ability __7, UnityEngine.Vector3 __8, UnityEngine.Vector3 __9, float __10, Il2Cpp.UseType __11, Il2CppSystem.Collections.Generic.List<Il2Cpp.Stats.Stat> __12, byte __13, bool __14, ref bool __15, ref int __16)
        static void Prefix(ref bool __15, ref int __16)
        {
            if (Scenes.IsGameScene())
            {
                if (ModSettings.SkillsGeneral.RemoveManaCost.Value)
                {
                    __15 = false; //consumesMana
                    //__16 = 0; //manaConsumed
                }
            }
        }
    }
}
