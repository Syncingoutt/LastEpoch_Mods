using System;
using LastEpoch_Hud.Scripts.Core.ModUI;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>World &gt; Camera HUD page. State resets on HUD re-bind (Build).</summary>
internal static class WorldCameraPage
{
    private static HudFormPage page;

    public static void Build(HudPageId pageId, GameObject parent, GameObject hud, Font font)
    {
        page = HudFormPage.Build(parent, hud, font, pageId);
        if (page == null)
            return;
        var card = page.AddCard("Camera", "Camera");
        page.AddButton(card, "Set", "Set", Hud_Manager.Content.Scenes.Camera.Set);
        page.AddButton(card, "Reset", "Reset", Hud_Manager.Content.Scenes.Camera.Reset);
        AddToggle(
            card,
            "Enable",
            "Enable Camera Override",
            () => Save_Manager.instance.data.Scenes.Camera.Enable_Mod,
            v => Save_Manager.instance.data.Scenes.Camera.Enable_Mod = v
        );
        AddSlider(
            card,
            "ZoomMinimum",
            "Zoom Minimum",
            Hud_Manager.Content.Scenes.Camera.zoom_minimum_slider,
            () => Save_Manager.instance.data.Scenes.Camera.Enable_ZoomMinimum,
            v => Save_Manager.instance.data.Scenes.Camera.Enable_ZoomMinimum = v,
            () => Save_Manager.instance.data.Scenes.Camera.ZoomMinimum,
            v => Save_Manager.instance.data.Scenes.Camera.ZoomMinimum = v
        );
        AddSlider(
            card,
            "ZoomPerScroll",
            "Zoom Per Scroll",
            Hud_Manager.Content.Scenes.Camera.zoom_per_scroll_slider,
            () => Save_Manager.instance.data.Scenes.Camera.Enable_ZoomPerScroll,
            v => Save_Manager.instance.data.Scenes.Camera.Enable_ZoomPerScroll = v,
            () => Save_Manager.instance.data.Scenes.Camera.ZoomPerScroll,
            v => Save_Manager.instance.data.Scenes.Camera.ZoomPerScroll = v
        );
        AddSlider(
            card,
            "ZoomSpeed",
            "Zoom Speed",
            Hud_Manager.Content.Scenes.Camera.zoom_speed_slider,
            () => Save_Manager.instance.data.Scenes.Camera.Enable_ZoomSpeed,
            v => Save_Manager.instance.data.Scenes.Camera.Enable_ZoomSpeed = v,
            () => Save_Manager.instance.data.Scenes.Camera.ZoomSpeed,
            v => Save_Manager.instance.data.Scenes.Camera.ZoomSpeed = v
        );
        AddSlider(
            card,
            "DefaultRotation",
            "Default Rotation",
            Hud_Manager.Content.Scenes.Camera.default_rotation_slider,
            () => Save_Manager.instance.data.Scenes.Camera.Enable_DefaultRotation,
            v => Save_Manager.instance.data.Scenes.Camera.Enable_DefaultRotation = v,
            () => Save_Manager.instance.data.Scenes.Camera.DefaultRotation,
            v => Save_Manager.instance.data.Scenes.Camera.DefaultRotation = v
        );
        AddSlider(
            card,
            "OffsetMinimum",
            "Offset Minimum",
            Hud_Manager.Content.Scenes.Camera.offset_minimum_slider,
            () => Save_Manager.instance.data.Scenes.Camera.Enable_OffsetMinimum,
            v => Save_Manager.instance.data.Scenes.Camera.Enable_OffsetMinimum = v,
            () => Save_Manager.instance.data.Scenes.Camera.OffsetMinimum,
            v => Save_Manager.instance.data.Scenes.Camera.OffsetMinimum = v
        );
        AddSlider(
            card,
            "OffsetMaximum",
            "Offset Maximum",
            Hud_Manager.Content.Scenes.Camera.offset_maximum_slider,
            () => Save_Manager.instance.data.Scenes.Camera.Enable_OffsetMaximum,
            v => Save_Manager.instance.data.Scenes.Camera.Enable_OffsetMaximum = v,
            () => Save_Manager.instance.data.Scenes.Camera.OffsetMaximum,
            v => Save_Manager.instance.data.Scenes.Camera.OffsetMaximum = v
        );
        AddSlider(
            card,
            "AngleMinimum",
            "Angle Minimum",
            Hud_Manager.Content.Scenes.Camera.angle_minimum_slider,
            () => Save_Manager.instance.data.Scenes.Camera.Enable_AngleMinimum,
            v => Save_Manager.instance.data.Scenes.Camera.Enable_AngleMinimum = v,
            () => Save_Manager.instance.data.Scenes.Camera.AngleMinimum,
            v => Save_Manager.instance.data.Scenes.Camera.AngleMinimum = v
        );
        AddSlider(
            card,
            "AngleMaximum",
            "Angle Maximum",
            Hud_Manager.Content.Scenes.Camera.angle_maximum_slider,
            () => Save_Manager.instance.data.Scenes.Camera.Enable_AngleMaximum,
            v => Save_Manager.instance.data.Scenes.Camera.Enable_AngleMaximum = v,
            () => Save_Manager.instance.data.Scenes.Camera.AngleMaximum,
            v => Save_Manager.instance.data.Scenes.Camera.AngleMaximum = v
        );
        AddToggle(
            card,
            "LoadOnStart",
            "Load On Start",
            () => Save_Manager.instance.data.Scenes.Camera.Enable_LoadOnStart,
            v => Save_Manager.instance.data.Scenes.Camera.Enable_LoadOnStart = v
        );
    }

    public static void Show() => page?.Show();

    public static void Hide() => page?.Hide();

    public static void Refresh() => page?.RefreshValues();

    private static void AddToggle(
        HudFormPage.Card card,
        string id,
        string label,
        Func<bool> read,
        Action<bool> write
    ) =>
        page.AddToggle(
            card,
            id,
            label,
            () => HasSave() && read(),
            v =>
            {
                if (HasSave())
                    write(v);
            }
        );

    private static void AddSlider(
        HudFormPage.Card card,
        string id,
        string label,
        Slider source,
        Func<bool> readEnabled,
        Action<bool> writeEnabled,
        Func<float> read,
        Action<float> write
    )
    {
        float min = source.IsNullOrDestroyed() ? 0f : source.minValue;
        float max = source.IsNullOrDestroyed() ? 100f : source.maxValue;
        bool whole = source.IsNullOrDestroyed() || source.wholeNumbers;
        page.AddToggleSlider(
            card,
            id,
            label,
            string.Empty,
            min,
            max,
            whole,
            () => HasSave() && readEnabled(),
            v =>
            {
                if (HasSave())
                    writeEnabled(v);
            },
            () => HasSave() ? read() : min,
            v =>
            {
                if (HasSave())
                    write(v);
            }
        );
    }

    private static bool HasSave() =>
        !Save_Manager.instance.IsNullOrDestroyed() && Save_Manager.instance.initialized;
}
