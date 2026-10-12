using System;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Keybind;

/// <summary>KeyCode names from Unity.</summary>
internal sealed class UnityKeyNames : ILegacyKeyNames
{
    public bool TryGetName(int code, out string name)
    {
        name = null;
        if (!Enum.IsDefined(typeof(KeyCode), (KeyCode)code))
        {
            return false;
        }
        name = ((KeyCode)code).ToString();
        return true;
    }

    public bool TryGetCode(string name, out int code)
    {
        code = 0;
        if (!Enum.TryParse(name, true, out KeyCode key))
        {
            return false;
        }
        code = (int)key;
        return true;
    }
}
