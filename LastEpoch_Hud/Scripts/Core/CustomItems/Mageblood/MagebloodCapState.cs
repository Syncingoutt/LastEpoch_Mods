using System;

namespace LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;

/// <summary>Arms the hit-scoped cap raise for the local player and remembers the cap to restore.</summary>
public sealed class MagebloodCapState
{
    private float _bonus;
    private float _saved;
    private bool _inside;

    public bool Armed => _bonus > 0f;

    /// <summary>Stores the effective bonus. True when the cap it implies changed.</summary>
    public bool TryArm(bool worn, float bonusPercent, float baseCap, out float from, out float to)
    {
        float previous = _bonus;
        _bonus = worn && float.IsFinite(bonusPercent) && bonusPercent > 0f ? bonusPercent : 0f;
        from = baseCap;
        to = baseCap;
        if (!float.IsFinite(baseCap))
        {
            return false;
        }

        from = MagebloodResistanceCap.Raise(baseCap, previous);
        to = MagebloodResistanceCap.Raise(baseCap, _bonus);
        return from != to;
    }

    /// <summary>True when the calc belongs to the owner and no raise is running.</summary>
    public bool Targets(IntPtr instance, IntPtr owner)
    {
        return Armed && !_inside && owner != IntPtr.Zero && instance == owner;
    }

    /// <summary>Starts one raise. True when <paramref name="raised"/> must be written.</summary>
    public bool TryEnter(float current, out float raised)
    {
        raised = current;
        if (_inside || !float.IsFinite(current))
        {
            return false;
        }

        float candidate = MagebloodResistanceCap.Raise(current, _bonus);
        if (candidate == current)
        {
            return false;
        }

        raised = candidate;
        _saved = current;
        _inside = true;
        return true;
    }

    /// <summary>Ends the raise. True when <paramref name="restore"/> must be written back.</summary>
    public bool TryExit(out float restore)
    {
        restore = 0f;
        if (!_inside)
        {
            return false;
        }

        restore = _saved;
        _inside = false;
        return true;
    }
}
