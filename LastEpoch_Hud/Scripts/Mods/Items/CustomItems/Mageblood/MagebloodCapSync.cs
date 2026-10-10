using System;
using Il2Cpp;
using LastEpoch_Hud.Scripts.Core.CustomItems.Mageblood;
using LastEpoch_Hud.Scripts.ModUI;

namespace LastEpoch_Hud.Scripts.Mods.Items.CustomItems.Mageblood;

/// <summary>Arms the Mageblood cap raise and applies it to the shared cap around player hits.</summary>
internal static class MagebloodCapSync
{
    private static readonly MagebloodCapState _state = new();

    public static void Sync(bool worn)
    {
        try
        {
            float bonus = MagebloodConfigLoader.Current.MaxResistances;
            if (
                _state.TryArm(
                    worn,
                    bonus,
                    ProtectionClass.resistanceCap,
                    out float from,
                    out float to
                )
            )
            {
                LogChange(from, to);
            }
        }
        catch (Exception ex)
        {
            ErrorLog.Report(ex, "MagebloodCapSync.Sync");
        }
    }

    public static bool Enter(ProtectionClass instance)
    {
        if (!_state.Armed)
        {
            return false;
        }

        try
        {
            if (!_state.Targets(instance.Pointer, OwnerPointer()))
            {
                return false;
            }
            if (!_state.TryEnter(ProtectionClass.resistanceCap, out float raised))
            {
                return false;
            }

            ProtectionClass.resistanceCap = raised;
            return true;
        }
        catch (Exception ex)
        {
            ErrorLog.Report(ex, "MagebloodCapSync.Enter");
            return false;
        }
    }

    public static void Exit(bool entered)
    {
        if (!entered)
        {
            return;
        }

        try
        {
            if (_state.TryExit(out float restore))
            {
                ProtectionClass.resistanceCap = restore;
            }
        }
        catch (Exception ex)
        {
            ErrorLog.Report(ex, "MagebloodCapSync.Exit");
        }
    }

    private static IntPtr OwnerPointer()
    {
        ProtectionClass player = Refs_Manager.player_protection_class;
        return player.IsNullOrDestroyed() ? IntPtr.Zero : player.Pointer;
    }

    private static void LogChange(float from, float to)
    {
        if (!ModSettings.Debug.Enabled.Value)
        {
            return;
        }

        Main.logger_instance?.Msg(MagebloodSyncLog.Cap(from, to));
    }
}
