using System;
using LastEpoch_Hud.Scripts.Core.ModUI;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Keybind;

/// <summary>KeyCode of a single-key binding, re-parsed only when the binding reference changes.</summary>
internal sealed class KeyCodeCache
{
    private string _binding;
    private KeyCode _key = KeyCode.None;

    public KeyCode Get(string binding)
    {
        if (ReferenceEquals(binding, _binding))
        {
            return _key;
        }
        _binding = binding;
        _key = Parse(binding);
        return _key;
    }

    private static KeyCode Parse(string binding)
    {
        string name = KeybindKeyName.Single(binding);
        if (name == null)
        {
            return KeyCode.None;
        }
        if (!Enum.TryParse(name, out KeyCode key))
        {
            return KeyCode.None;
        }
        return Enum.IsDefined(typeof(KeyCode), key) ? key : KeyCode.None;
    }
}
