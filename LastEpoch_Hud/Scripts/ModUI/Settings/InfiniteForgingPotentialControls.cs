using HarmonyLib;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.ModUI.Settings;

internal static class InfiniteForgingPotentialControls
{
    public static void Bind(GameObject content, GameObject viewport)
    {
        if (
            viewport.IsNullOrDestroyed()
            || Prefab.Child(viewport, "InfiniteForgingPotential") != null
        )
            return;
        var panel = Prefab.Child(viewport, "ForginPotencial");
        if (panel.IsNullOrDestroyed())
            return;
        var original = Prefab.Child(panel, "Toggle_Items_Craft_ForginPotencial");
        if (original.IsNullOrDestroyed())
            return;
        var row = Node(viewport, "InfiniteForgingPotential", 0, 1, 1, 1);
        var rowRect = row.GetComponent<RectTransform>();
        rowRect.pivot = new Vector2(.5f, 1);
        var panelRect = panel.GetComponent<RectTransform>();
        rowRect.anchorMin = new Vector2(panelRect.anchorMin.x, 1);
        rowRect.anchorMax = new Vector2(panelRect.anchorMax.x, 1);
        rowRect.sizeDelta = new Vector2(panelRect.sizeDelta.x, 68);
        rowRect.anchoredPosition = new Vector2(panelRect.anchoredPosition.x, 0);
        row.AddComponent<LayoutElement>().preferredHeight = 68;
        // Reuse the native prefab's checkbox geometry, sprites, font and transitions.
        var control = UnityEngine.Object.Instantiate(original, row.transform);
        control.name = "Toggle_InfiniteForgingPotential";
        var rect = control.GetComponent<RectTransform>();
        var originalRect = original.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(originalRect.anchorMin.x, .5f);
        rect.anchorMax = new Vector2(originalRect.anchorMax.x, 1);
        rect.offsetMin = new Vector2(originalRect.offsetMin.x, 0);
        rect.offsetMax = new Vector2(originalRect.offsetMax.x, 0);
        var toggle = control.GetComponent<Toggle>();
        if (toggle.IsNullOrDestroyed())
        {
            UnityEngine.Object.Destroy(row);
            return;
        }
        toggle.group = null;
        toggle.interactable = true;
        toggle.onValueChanged.RemoveAllListeners();
        var value = Prefab.Child(control, "Value");
        if (!value.IsNullOrDestroyed())
            value.SetActive(false);
        var labelObject = Prefab.Child(control, "Label");
        var label = labelObject.IsNullOrDestroyed() ? null : labelObject.GetComponent<Text>();
        if (!label.IsNullOrDestroyed())
            Prefab.ApplyLabel(label, "Infinite Forging Potential");

        // Keep the row transparent and reuse the label's gold for the divider.
        var line = Node(row, "Divider", 0, 0, 1, 0).AddComponent<Image>();
        line.rectTransform.sizeDelta = new Vector2(0, 1);
        line.color = label.IsNullOrDestroyed() ? HudTheme.Accent : label.color;
        line.raycastTarget = false;
        var buttonObject = Node(row, "Btn_Craft_DeselectAll", .76f, .12f, .99f, .88f);
        var buttonImage = buttonObject.AddComponent<Image>();
        buttonImage.color = HudTheme.InputBackground;
        var button = buttonObject.AddComponent<Button>();
        button.targetGraphic = buttonImage;
        // Explicit Text: arbitrary HUD buttons can have no Text or use TMP.
        var buttonText = Node(buttonObject, "Label", .02f, 0, .98f, 1).AddComponent<Text>();
        if (!label.IsNullOrDestroyed())
        {
            buttonText.font = label.font;
            buttonText.fontSize = label.fontSize;
            buttonText.color = label.color;
        }
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.raycastTarget = false;
        Prefab.ApplyLabel(buttonText, "Deselect All");
        var buttonLine = Node(buttonObject, "Divider", 0, 0, 1, 0).AddComponent<Image>();
        buttonLine.rectTransform.sizeDelta = new Vector2(0, 1);
        buttonLine.color = line.color;
        buttonLine.raycastTarget = false;
        ModSettings.InfiniteForgingPotential.Enabled.Changed += enabled =>
        {
            if (!toggle.IsNullOrDestroyed())
                toggle.SetIsOnWithoutNotify(enabled);
        };
        toggle.SetIsOnWithoutNotify(ModSettings.InfiniteForgingPotential.Enabled.Value);
        AddAdvancedToggle(
            row,
            original,
            "Toggle_AdvancedForge_T7",
            "Craft Affixes to T7",
            .00f,
            .25f,
            ModSettings.AdvancedForge.AllowT7Crafting.Value
        );
        AddAdvancedToggle(
            row,
            original,
            "Toggle_AdvancedForge_MaxRoll",
            "Max Crafted Roll",
            .25f,
            .50f,
            ModSettings.AdvancedForge.AffixRoll.Enabled
        );
        AddAdvancedToggle(
            row,
            original,
            "Toggle_AdvancedForge_Hope",
            "Guarantee Hope",
            .50f,
            .75f,
            ModSettings.AdvancedForge.GuaranteedGlyphOfHope.Value
        );
        AddAdvancedToggle(
            row,
            original,
            "Toggle_AdvancedForge_Despair",
            "Guarantee Despair",
            .75f,
            1.00f,
            ModSettings.AdvancedForge.GuaranteedGlyphOfDespair.Value
        );
        MelonLoader.MelonCoroutines.Start(PositionRow(viewport, row));
        Main.logger_instance?.Msg("Infinite Forging Potential checkbox bound in Items > Crafting.");
    }

    // Match the existing HUD's native event hooks; managed listeners do not
    // reliably receive IL2CPP control events in this build.
    [HarmonyPatch(typeof(Toggle), "OnPointerClick")]
    internal static class InfiniteClick
    {
        [HarmonyPostfix]
        static void Postfix(Toggle __instance)
        {
            if (__instance.IsNullOrDestroyed() || !__instance.interactable)
                return;
            switch (__instance.gameObject.name)
            {
                case "Toggle_InfiniteForgingPotential":
                    ModSettings.InfiniteForgingPotential.Enabled.Set(__instance.isOn);
                    Main.logger_instance?.Msg(
                        "Infinite Forging Potential: " + (__instance.isOn ? "enabled" : "disabled")
                    );
                    break;
                case "Toggle_AdvancedForge_T7":
                    ModSettings.AdvancedForge.AllowT7Crafting.Set(__instance.isOn);
                    break;
                case "Toggle_AdvancedForge_MaxRoll":
                    ModSettings.AdvancedForge.AffixRoll.SetEnabled(__instance.isOn);
                    if (__instance.isOn)
                        ModSettings.AdvancedForge.AffixRoll.SetValue(255f);
                    break;
                case "Toggle_AdvancedForge_Hope":
                    ModSettings.AdvancedForge.GuaranteedGlyphOfHope.Set(__instance.isOn);
                    break;
                case "Toggle_AdvancedForge_Despair":
                    ModSettings.AdvancedForge.GuaranteedGlyphOfDespair.Set(__instance.isOn);
                    break;
            }
        }
    }

    [HarmonyPatch(typeof(Button), "Press")]
    internal static class DeselectClick
    {
        [HarmonyPostfix]
        static void Postfix(Button __instance)
        {
            if (
                __instance.IsNullOrDestroyed()
                || !__instance.interactable
                || __instance.gameObject.name != "Btn_Craft_DeselectAll"
            )
                return;
            var row = __instance.transform.parent;
            if (row.IsNullOrDestroyed() || row.parent.IsNullOrDestroyed())
                return;
            DeselectAll(row.parent.gameObject);
        }
    }

    static void DeselectAll(GameObject viewport)
    {
        // Legacy crafting settings update on pointer clicks, not value-change events.
        // Clear their saved flags explicitly; merely unchecking the visuals is insufficient.
        if (!Scripts.Save_Manager.instance.IsNullOrDestroyed())
        {
            var settings = Scripts.Save_Manager.instance.data.Items.CraftingSlot;
            settings.Enable_ForginPotencial = false;
            settings.Enable_Implicit_0 = false;
            settings.Enable_Implicit_1 = false;
            settings.Enable_Implicit_2 = false;
            settings.Enable_Seal_Tier = false;
            settings.Enable_Seal_Value = false;
            settings.Enable_Affix_0_Tier = false;
            settings.Enable_Affix_1_Tier = false;
            settings.Enable_Affix_2_Tier = false;
            settings.Enable_Affix_3_Tier = false;
            settings.Enable_Affix_0_Value = false;
            settings.Enable_Affix_1_Value = false;
            settings.Enable_Affix_2_Value = false;
            settings.Enable_Affix_3_Value = false;
            settings.Enable_UniqueMod_0 = false;
            settings.Enable_UniqueMod_1 = false;
            settings.Enable_UniqueMod_2 = false;
            settings.Enable_UniqueMod_3 = false;
            settings.Enable_UniqueMod_4 = false;
            settings.Enable_UniqueMod_5 = false;
            settings.Enable_UniqueMod_6 = false;
            settings.Enable_UniqueMod_7 = false;
            settings.Enable_LegendaryPotencial = false;
            settings.Enable_WeaverWill = false;
            Scripts.Save_Manager.instance.data.Items.CraftingSlot = settings;
            Scripts.Save_Manager.instance.Save();
        }
        var toggles = viewport.GetComponentsInChildren<Toggle>(true);
        foreach (var toggle in toggles)
        {
            if (
                toggle.IsNullOrDestroyed()
                || toggle.gameObject.name == "Toggle_InfiniteForgingPotential"
                || toggle.gameObject.name.StartsWith("Toggle_AdvancedForge_")
            )
                continue;
            toggle.SetIsOnWithoutNotify(false);
        }
    }

    static System.Collections.IEnumerator PositionRow(GameObject viewport, GameObject row)
    {
        while (!viewport.IsNullOrDestroyed() && !viewport.activeInHierarchy)
            yield return null;
        if (viewport.IsNullOrDestroyed() || row.IsNullOrDestroyed())
            yield break;
        yield return null;
        Canvas.ForceUpdateCanvases();
        var contentRect = viewport.GetComponent<RectTransform>();
        if (contentRect.IsNullOrDestroyed())
            yield break;
        if (!viewport.GetComponent<VerticalLayoutGroup>().IsNullOrDestroyed())
        {
            row.transform.SetAsFirstSibling();
            LayoutRebuilder.MarkLayoutForRebuild(contentRect);
            yield break;
        }
        // Preserve existing row geometry before enlarging the scroll content.
        var rects = new System.Collections.Generic.List<RectTransform>();
        var centers = new System.Collections.Generic.List<Vector2>();
        var sizes = new System.Collections.Generic.List<Vector2>();
        for (int i = 0; i < viewport.transform.childCount; i++)
        {
            var child = viewport.transform.GetChild(i).GetComponent<RectTransform>();
            if (child.IsNullOrDestroyed() || child.gameObject == row)
                continue;
            rects.Add(child);
            centers.Add(
                new Vector2(child.localPosition.x, child.localPosition.y) - contentRect.rect.center
            );
            sizes.Add(child.rect.size);
        }
        float height = contentRect.rect.height;
        contentRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height + 72);
        for (int i = 0; i < rects.Count; i++)
        {
            var child = rects[i];
            child.anchorMin = child.anchorMax = new Vector2(.5f, 1);
            child.sizeDelta = sizes[i];
            // The old pivot's position, measured from the old content top.
            child.anchoredPosition = new Vector2(centers[i].x, centers[i].y - height / 2 - 72);
        }
        var rowRect = row.GetComponent<RectTransform>();
        rowRect.anchoredPosition = new Vector2(rowRect.anchoredPosition.x, 0);
    }

    static void AddAdvancedToggle(
        GameObject row,
        GameObject template,
        string name,
        string text,
        float left,
        float right,
        bool initial
    )
    {
        var control = UnityEngine.Object.Instantiate(template, row.transform);
        control.name = name;
        var rect = control.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(left, 0f);
        rect.anchorMax = new Vector2(right, .46f);
        rect.offsetMin = new Vector2(2, 0);
        rect.offsetMax = new Vector2(-2, 0);

        var toggle = control.GetComponent<Toggle>();
        if (toggle.IsNullOrDestroyed())
        {
            UnityEngine.Object.Destroy(control);
            return;
        }
        toggle.group = null;
        toggle.interactable = true;
        toggle.onValueChanged.RemoveAllListeners();
        toggle.SetIsOnWithoutNotify(initial);

        var value = Prefab.Child(control, "Value");
        if (!value.IsNullOrDestroyed())
            value.SetActive(false);
        var labelObject = Prefab.Child(control, "Label");
        var label = labelObject.IsNullOrDestroyed() ? null : labelObject.GetComponent<Text>();
        if (!label.IsNullOrDestroyed())
        {
            Prefab.ApplyLabel(label, text);
            label.fontSize = Mathf.Max(9, label.fontSize - 1);
        }
    }

    static GameObject Node(
        GameObject parent,
        string name,
        float left,
        float bottom,
        float right,
        float top
    )
    {
        var go = new GameObject(name);
        go.layer = parent.layer;
        var rect = go.AddComponent<RectTransform>();
        rect.SetParent(parent.transform, false);
        rect.anchorMin = new Vector2(left, bottom);
        rect.anchorMax = new Vector2(right, top);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return go;
    }
}
