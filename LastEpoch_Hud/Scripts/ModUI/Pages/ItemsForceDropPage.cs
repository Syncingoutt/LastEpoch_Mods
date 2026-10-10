using LastEpoch_Hud.Scripts.ModUI.ForceDrop;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;
using FD = LastEpoch_Hud.Scripts.Hud_Manager.Content.OdlForceDrop;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

// Items > Force Drop. The feature still reads the original prefab controls as
// its game-facing data model, while ForceDropBuilder supplies the replacement
// runtime view inside the redesigned HUD shell.
internal static class ItemsForceDropPage
{
    public static void Show()
    {
        if (FD.content_obj.IsNullOrDestroyed())
            return;

        FD.Set_Active(true);
        Hud_Manager.Content.Set_Active();

        // Avoid flashing the legacy controls while the catalog initializes on
        // the first frame. The builder restores them if replacement setup fails.
        if (!ForceDropBuilder.IsReady)
            foreach (var child in Functions.GetAllChild(FD.content_obj))
                child.SetActive(false);
    }

    public static void Hide()
    {
        FD.Set_Active(false);
        Hud_Manager.Content.Set_Active();
    }
}
