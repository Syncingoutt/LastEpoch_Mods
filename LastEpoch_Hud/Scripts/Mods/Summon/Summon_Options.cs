using System;
using System.Collections.Generic;
using Il2Cpp;
using LastEpoch_Hud.Scripts.ModUI;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using MelonLoader;
using UnityEngine;
using UnityEngine.AI;

namespace LastEpoch_Hud.Scripts.Mods.Summon;

[RegisterTypeInIl2Cpp]
public class Summon_Options : MonoBehaviour
{
    private readonly Dictionary<BaseHealth, bool> _health = new Dictionary<BaseHealth, bool>();
    private readonly Dictionary<NavMeshAgent, float> _radii = new Dictionary<NavMeshAgent, float>();

    public Summon_Options(IntPtr ptr)
        : base(ptr) { }

    private void RestoreHealth()
    {
        foreach (KeyValuePair<BaseHealth, bool> entry in _health)
        {
            if (!entry.Key.IsNullOrDestroyed())
            {
                entry.Key.damageable = entry.Value;
            }
        }
        _health.Clear();
    }

    private void RestoreRadii()
    {
        foreach (KeyValuePair<NavMeshAgent, float> entry in _radii)
        {
            if (!entry.Key.IsNullOrDestroyed())
            {
                entry.Key.radius = entry.Value;
            }
        }
        _radii.Clear();
    }

    private void OnDisable()
    {
        RestoreHealth();
        RestoreRadii();
    }

    private void Update()
    {
        bool ready =
            Scenes.IsGameScene()
            && SaveManager.IsLoaded
            && !Refs_Manager.player_actor.IsNullOrDestroyed();
        if (!ready)
        {
            OnDisable();
            return;
        }
        bool godMode = ModSettings.Summon.GodMode.Value;
        bool dontCollide = ModSettings.Summon.DontCollide.Value;
        bool forever = ModSettings.Summon.Forever.Value;
        if (!godMode)
        {
            RestoreHealth();
        }
        if (!dontCollide)
        {
            RestoreRadii();
        }
        if (!godMode && !dontCollide && !forever)
        {
            return;
        }
        SummonTracker tracker;
        if (
            !Refs_Manager.player_actor.TryGetExistingSummonTracker(out tracker)
            || tracker.IsNullOrDestroyed()
        )
        {
            return;
        }
        if (tracker.summons == null)
        {
            return;
        }
        foreach (Summoned summoned in tracker.summons)
        {
            if (summoned.IsNullOrDestroyed() || summoned.gameObject.IsNullOrDestroyed())
            {
                continue;
            }
            if (godMode)
            {
                UnitHealth unit = summoned.gameObject.GetComponent<UnitHealth>();
                BaseHealth target = unit.IsNullOrDestroyed() ? null : unit.TryCast<BaseHealth>();
                if (!target.IsNullOrDestroyed())
                {
                    if (!_health.ContainsKey(target))
                    {
                        _health.Add(target, target.damageable);
                    }
                    target.damageable = false;
                }
            }
            if (dontCollide)
            {
                NavMeshAgent agent = summoned.gameObject.GetComponent<NavMeshAgent>();
                if (!agent.IsNullOrDestroyed())
                {
                    if (!_radii.ContainsKey(agent))
                    {
                        _radii.Add(agent, agent.radius);
                    }
                    agent.radius = 0f;
                }
            }
            if (forever)
            {
                UnsummonAfterDelay delay = summoned.gameObject.GetComponent<UnsummonAfterDelay>();
                // Match the original Forever option: remove the lifetime component.
                // Existing summons stay permanent after this option is switched off.
                if (!delay.IsNullOrDestroyed())
                {
                    UnityEngine.Object.Destroy(delay);
                }
            }
        }
    }
}
