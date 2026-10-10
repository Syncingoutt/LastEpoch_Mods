using LastEpoch_Hud.Scripts.Mods.Items.CustomItems.Mageblood.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>Items > Custom Items: one card per custom item.</summary>
internal static class Items_CustomItems
{
    public const string RootName = "Items_CustomItems";
    public const string PageId = "items.custom";

    private static HudFormPage _page;

    public static void Build(GameObject parent, GameObject hud, Font font)
    {
        _page = HudFormPage.Build(parent, hud, font, RootName);
        if (_page == null)
        {
            return;
        }

        MagebloodMenu.Build(_page);
        MagebloodFlasksCard.Build(_page);
    }

    public static void Show()
    {
        if (_page == null)
        {
            return;
        }

        MagebloodMenu.OnShow();
        MagebloodFlasksCard.OnShow();
        _page.Show();
        Refresh();
    }

    public static void Hide() => _page?.Hide();

    public static void Refresh()
    {
        if (_page == null)
        {
            return;
        }

        float now = Time.unscaledTime;
        bool menu = MagebloodMenu.Refresh(now);
        bool flasks = MagebloodFlasksCard.Refresh(now);
        if (!menu && !flasks)
        {
            return;
        }

        _page.RefreshValues();
    }
}
