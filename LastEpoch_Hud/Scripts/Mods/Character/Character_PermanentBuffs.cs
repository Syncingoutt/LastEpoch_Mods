using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LastEpoch_Hud.Scripts.Mods.Character;

[RegisterTypeInIl2Cpp]
public class Character_PermanentBuffs : MonoBehaviour
{
    public static Character_PermanentBuffs instance { get; private set; }

    public Character_PermanentBuffs(System.IntPtr ptr)
        : base(ptr) { }

    public struct PermanentBuff
    {
        public string Name;
        public bool Toggle;
        public float Value;
        public SP Propertie;
        public Buff_Type Type;
        public AT Tags;
    }

    public enum Buff_Type
    {
        Add,
        Increase,
    }

    void Awake()
    {
        instance = this;
        SceneManager.add_sceneLoaded(new System.Action<Scene, LoadSceneMode>(OnSceneLoaded));
    }

    void Update()
    {
        SyncModEnable();

        if ((Mod_Enable) && (!force_disable))
        {
            if ((Buff_Enable) && (!Running))
            {
                StartBuffs();
            }
            else if (Running)
            {
                System.TimeSpan elaspedTime = System.DateTime.Now - StartTime;
                System.Double seconds = elaspedTime.TotalSeconds;
                if (seconds > (Buff_Duration - 1))
                {
                    Running = false;
                }
                else
                {
                    if ((PauseMenu) && (!HudIsOpen()))
                    {
                        Buff_Enable = InitBuffs();
                        Running = false;
                        if (!Buff_Enable)
                        {
                            RemoveBuffs();
                        }
                    }
                    PauseMenu = HudIsOpen();
                }
            }
            else
            {
                if ((PauseMenu) && (!HudIsOpen()))
                {
                    Buff_Enable = InitBuffs();
                }
                PauseMenu = HudIsOpen();
            }
        }
        else if ((Mod_Enable) && (force_disable))
        {
            Running = false;
            Buff_Enable = false;
            RemoveBuffs();
            Mod_Enable = false;
            force_disable = false;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Buff_Enable = InitBuffs();
        Running = false;
    }

    private void SyncModEnable()
    {
        if (!SaveManager.IsLoaded)
        {
            Mod_Enable = false;
            return;
        }
        if (ModSettings.PermanentBuffs.EnableMod.Value)
        {
            Mod_Enable = true;
            return;
        }
        if (Mod_Enable)
        {
            force_disable = true;
        }
    }

    bool HudIsOpen()
    {
        if (!Hud_Manager.hud_object.IsNullOrDestroyed())
        {
            return Hud_Manager.hud_object.active;
        }
        else
        {
            return false;
        }
    }

    private bool Mod_Enable = false;
    private bool force_disable = false;
    private bool Buff_Enable = false;
    private bool Running = false;
    private bool Starting = false;
    private System.Collections.Generic.List<PermanentBuff> Buffs;
    private System.DateTime StartTime;
    private readonly float Buff_Duration = 255f;
    private bool PauseMenu = false;

    private bool InitBuffs()
    {
        bool result = false;
        Buffs = new System.Collections.Generic.List<PermanentBuff>
        {
            new PermanentBuff
            {
                Name = "MoveSpeed_Buff",
                Value = ModSettings.PermanentBuffs.MoveSpeed.Value,
                Propertie = SP.Movespeed,
                Type = Buff_Type.Increase,
                Toggle = ModSettings.PermanentBuffs.MoveSpeed.Enabled,
            },
            new PermanentBuff
            {
                Name = "CastSpeed_Buff",
                Value = ModSettings.PermanentBuffs.CastSpeed.Value,
                Propertie = SP.CastSpeed,
                Type = Buff_Type.Increase,
                Toggle = ModSettings.PermanentBuffs.CastSpeed.Enabled,
            },
            new PermanentBuff
            {
                Name = "AttackSpeed_Buff",
                Value = ModSettings.PermanentBuffs.AttackSpeed.Value,
                Propertie = SP.AttackSpeed,
                Type = Buff_Type.Increase,
                Toggle = ModSettings.PermanentBuffs.AttackSpeed.Enabled,
            },
            new PermanentBuff
            {
                Name = "Damage_Buff",
                Value = ModSettings.PermanentBuffs.Damage.Value,
                Propertie = SP.Damage,
                Type = Buff_Type.Increase,
                Toggle = ModSettings.PermanentBuffs.Damage.Enabled,
            },
            new PermanentBuff
            {
                Name = "ManaRegen_Buff",
                Value = ModSettings.PermanentBuffs.ManaRegen.Value,
                Propertie = SP.ManaRegen,
                Type = Buff_Type.Increase,
                Toggle = ModSettings.PermanentBuffs.ManaRegen.Enabled,
            },
            new PermanentBuff
            {
                Name = "HealthRegen_Buff",
                Value = ModSettings.PermanentBuffs.HealthRegen.Value,
                Propertie = SP.HealthRegen,
                Type = Buff_Type.Increase,
                Toggle = ModSettings.PermanentBuffs.HealthRegen.Enabled,
            },
            new PermanentBuff
            {
                Name = "CriticalChance_Buff",
                Value = ModSettings.PermanentBuffs.CriticalChance.Value,
                Propertie = SP.CriticalChance,
                Type = Buff_Type.Add,
                Toggle = ModSettings.PermanentBuffs.CriticalChance.Enabled,
            },
            new PermanentBuff
            {
                Name = "CriticalMultiplier_Buff",
                Value = ModSettings.PermanentBuffs.CriticalMultiplier.Value,
                Propertie = SP.CriticalMultiplier,
                Type = Buff_Type.Add,
                Toggle = ModSettings.PermanentBuffs.CriticalMultiplier.Enabled,
            },
            new PermanentBuff
            {
                Name = "Strength_Buff",
                Value = ModSettings.PermanentBuffs.Strength.Value,
                Propertie = SP.Strength,
                Type = Buff_Type.Add,
                Toggle = ModSettings.PermanentBuffs.Strength.Enabled,
            },
            new PermanentBuff
            {
                Name = "Intelligence_Buff",
                Value = ModSettings.PermanentBuffs.Intelligence.Value,
                Propertie = SP.Intelligence,
                Type = Buff_Type.Add,
                Toggle = ModSettings.PermanentBuffs.Intelligence.Enabled,
            },
            new PermanentBuff
            {
                Name = "Dexterity_Buff",
                Value = ModSettings.PermanentBuffs.Dexterity.Value,
                Propertie = SP.Dexterity,
                Type = Buff_Type.Add,
                Toggle = ModSettings.PermanentBuffs.Dexterity.Enabled,
            },
            new PermanentBuff
            {
                Name = "Attunement_Buff",
                Value = ModSettings.PermanentBuffs.Attunement.Value,
                Propertie = SP.Attunement,
                Type = Buff_Type.Add,
                Toggle = ModSettings.PermanentBuffs.Attunement.Enabled,
            },
            new PermanentBuff
            {
                Name = "Vitality_Buff",
                Value = ModSettings.PermanentBuffs.Vitality.Value,
                Propertie = SP.Vitality,
                Type = Buff_Type.Add,
                Toggle = ModSettings.PermanentBuffs.Vitality.Enabled,
            },
            new PermanentBuff
            {
                Name = "AreaOfEffect_Buff",
                Value = ModSettings.PermanentBuffs.AreaOfEffect.Value,
                Propertie = SP.IncreasedAreaForAreaSkills,
                Type = Buff_Type.Add,
                Toggle = ModSettings.PermanentBuffs.AreaOfEffect.Enabled,
                Tags = AT.None,
            },
            new PermanentBuff
            {
                Name = "MeleeAreaOfEffect_Buff",
                Value = ModSettings.PermanentBuffs.AreaOfEffect.Value,
                Propertie = SP.IncreasedAreaForAreaSkills,
                Type = Buff_Type.Add,
                Toggle = ModSettings.PermanentBuffs.AreaOfEffect.Enabled,
                Tags = AT.Melee,
            },
        };

        foreach (PermanentBuff p in Buffs)
        {
            if (p.Toggle)
            {
                result = true;
                break;
            }
        }

        return result;
    }

    private void StartBuffs()
    {
        if (!Starting)
        {
            Starting = true;
            if (!Refs_Manager.player_actor.IsNullOrDestroyed())
            {
                System.Collections.Generic.List<string> player_buffs =
                    new System.Collections.Generic.List<string>();
                foreach (Buff player_buff in Refs_Manager.player_actor.statBuffs.buffs)
                {
                    player_buffs.Add(player_buff.name);
                }
                foreach (PermanentBuff permanent_buff in Buffs)
                {
                    if (player_buffs.Contains(permanent_buff.Name))
                    {
                        Refs_Manager.player_actor.statBuffs.removeBuffsWithName(
                            permanent_buff.Name
                        );
                    }
                    if (permanent_buff.Toggle)
                    {
                        float add = 0;
                        float increase = 0;
                        if (permanent_buff.Type == Buff_Type.Add)
                        {
                            add = permanent_buff.Value;
                        }
                        else
                        {
                            increase = permanent_buff.Value;
                        }
                        Refs_Manager.player_actor.statBuffs.addBuff(
                            Buff_Duration,
                            permanent_buff.Propertie,
                            add,
                            increase,
                            null,
                            permanent_buff.Tags,
                            0,
                            0,
                            permanent_buff.Name
                        );
                    }
                }
                StartTime = System.DateTime.Now;
                Running = true;
            }
            Starting = false;
        }
    }

    private void RemoveBuffs()
    {
        if (Buffs == null)
        {
            return;
        }
        if (!Refs_Manager.player_actor.IsNullOrDestroyed())
        {
            System.Collections.Generic.List<string> player_buffs =
                new System.Collections.Generic.List<string>();
            foreach (Buff player_buff in Refs_Manager.player_actor.statBuffs.buffs)
            {
                player_buffs.Add(player_buff.name);
            }
            foreach (PermanentBuff permanent_buff in Buffs)
            {
                if (player_buffs.Contains(permanent_buff.Name))
                {
                    Refs_Manager.player_actor.statBuffs.removeBuffsWithName(permanent_buff.Name);
                }
            }
        }
    }
}
