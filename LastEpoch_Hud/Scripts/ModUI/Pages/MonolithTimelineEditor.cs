using LastEpoch_Hud.Scripts.ModUI.Settings;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;
using UnityEngine.UI;
using Data = LastEpoch_Hud.Scripts.Hud_Manager.Content.Character.Data;

namespace LastEpoch_Hud.Scripts.ModUI.Pages;

/// <summary>
/// The bundled HUD uses manually positioned RectTransforms, not layout groups.
/// Give every moved control new anchors instead of retaining its old viewport geometry.
/// Build runs once per root; after the HUD (and root) is destroyed it rebuilds and clears the button and accent lists.
/// </summary>
internal static class MonolithTimelineEditor
{
    const float EditorHeight = 500f;
    static Color Gold => HudTheme.AccentBright;
    static Color PanelColor => HudTheme.InputBackground;
    static GameObject root;
    static Text selectedLabel;
    static Text availabilityLabel;
    static readonly System.Collections.Generic.List<Button> timelineButtons =
        new System.Collections.Generic.List<Button>();
    static readonly System.Collections.Generic.List<GameObject> timelineAccents =
        new System.Collections.Generic.List<GameObject>();

    public static void Build(GameObject content)
    {
        if (
            content.IsNullOrDestroyed()
            || Data.save_button.IsNullOrDestroyed()
            || Data.monolith_dropdown.IsNullOrDestroyed()
            || !root.IsNullOrDestroyed()
        )
        {
            return;
        }

        RectTransform contentRect = content.GetComponent<RectTransform>();
        if (contentRect.IsNullOrDestroyed())
        {
            return;
        }
        Text template = Data.save_button.GetComponentInChildren<Text>(true);
        if (template.IsNullOrDestroyed())
        {
            return;
        }

        // Leave the existing Monolith settings above the editor. Their children
        // are anchored at the top, so extending the scroll content preserves them.
        float top = contentRect.rect.height + 12f;
        root = CreateObject("TimelineEditor", content);
        Place(root, 0f, 1f, top, EditorHeight, 8f, 8f);
        LayoutElement layout = root.AddComponent<LayoutElement>();
        layout.ignoreLayout = true;
        contentRect.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            top + EditorHeight + 12f
        );

        GameObject list = CreatePanel("TimelineSelection", root, 0f, 0.34f);
        GameObject values = CreatePanel("TimelineValues", root, 0.34f, 0.75f);
        GameObject actions = CreatePanel("TimelineActions", root, 0.75f, 1f);
        CreateLabel(list, template, "Timelines", 8f, 32f);
        selectedLabel = CreateLabel(values, template, "Selected Timeline", 8f, 52f);
        CreateLabel(actions, template, "Actions", 8f, 32f);

        timelineButtons.Clear();
        timelineAccents.Clear();
        // The existing dropdown indices are the game's timeline IDs (1 through 10).
        for (int id = 1; id < Data.monolith_dropdown.options.Count; id++)
        {
            int timelineId = id;
            string name = Data.monolith_dropdown.options[id].text.Replace('_', ' ');
            Button button = CreateButton(list, template, name, 46f + (id - 1) * 44f, 40f);
            Hud_Manager.Events.Set_Button_Event(
                button,
                new System.Action(() => Select(timelineId))
            );
            timelineButtons.Add(button);
            timelineAccents.Add(CreateSelectionAccent(button.gameObject));
        }

        MoveValue(
            Data.monolith_stability_basic_go,
            Data.monolith_stability_basic_input,
            Data.monolith_stability_basic_slider,
            values,
            "Normal Stability",
            76f
        );
        MoveValue(
            Data.monolith_stability_empower_go,
            Data.monolith_stability_empower_input,
            Data.monolith_stability_empower_slider,
            values,
            "Empowered Stability",
            158f
        );
        MoveValue(
            Data.monolith_corruption_go,
            Data.monolith_corruption_input,
            Data.monolith_corruption_slider,
            values,
            "Corruption",
            240f
        );
        MoveValue(
            Data.monolith_gaze_go,
            Data.monolith_gaze_input,
            Data.monolith_gaze_slider,
            values,
            "Gazes",
            322f
        );
        availabilityLabel = CreateLabel(values, template, "", 408f, 80f);
        availabilityLabel.fontSize = 13;

        if (!Data.monolith_corruption_all_button.IsNullOrDestroyed())
        {
            GameObject buttonObject = Data.monolith_corruption_all_button.gameObject;
            buttonObject.transform.SetParent(actions.transform, false);
            StyleButton(Data.monolith_corruption_all_button);
            Place(buttonObject, 0f, 1f, 46f, 40f, 10f, 10f);
            Text label = buttonObject.GetComponentInChildren<Text>(true);
            if (!label.IsNullOrDestroyed())
            {
                Prefab.ApplyLabel(label, "Copy to All");
                label.resizeTextForBestFit = true;
                label.resizeTextMinSize = 11;
                label.resizeTextMaxSize = 16;
            }
        }
        Button reload = CreateButton(actions, template, "Reload Values", 98f, 40f);
        Hud_Manager.Events.Set_Button_Event(
            reload,
            new System.Action(Hud_Manager.Content.Character.Update_Monoliths_Data)
        );
        Text note = CreateLabel(
            actions,
            template,
            "Edits apply immediately.\n\nCopy to All applies the visible values to existing timelines of the same difficulty.",
            158f,
            190f
        );
        note.fontSize = 13;
        note.alignment = TextAnchor.UpperLeft;

        // Keep the dropdown as the existing backend's ID binding, hidden from view.
        if (!Data.monolith_selector_go.IsNullOrDestroyed())
        {
            Data.monolith_selector_go.SetActive(false);
        }
        int selected = Data.monolith_dropdown.value;
        if (selected <= 0 && !Refs_Manager.player_data.IsNullOrDestroyed())
        {
            foreach (var run in Refs_Manager.player_data.MonolithRuns)
            {
                if (run.TimelineID > 0 && run.TimelineID < Data.monolith_dropdown.options.Count)
                {
                    selected = run.TimelineID;
                    break;
                }
            }
        }
        Select(selected > 0 ? selected : 1);
    }

    static void Select(int id)
    {
        Data.monolith_dropdown.value = id;
        Hud_Manager.Content.Character.Update_Monoliths_Data();
    }

    public static void RefreshSelection()
    {
        if (root.IsNullOrDestroyed() || Data.monolith_dropdown.IsNullOrDestroyed())
        {
            return;
        }
        int selected = Data.monolith_dropdown.value;
        if (
            !selectedLabel.IsNullOrDestroyed()
            && selected > 0
            && selected < Data.monolith_dropdown.options.Count
        )
        {
            Prefab.ApplyLabel(
                selectedLabel,
                Data.monolith_dropdown.options[selected].text.Replace('_', ' ')
            );
        }
        for (int i = 0; i < timelineButtons.Count; i++)
        {
            Button button = timelineButtons[i];
            if (button.IsNullOrDestroyed())
            {
                continue;
            }
            bool isSelected = i + 1 == selected;
            Color normal = isSelected ? HudTheme.Selection : HudTheme.Surface;
            button.colors = HudTheme.ActionButtonColors(
                normal,
                isSelected ? HudTheme.Selection : HudTheme.SurfaceHover
            );
            var image = button.GetComponent<Image>();
            if (!image.IsNullOrDestroyed())
                image.color = HudTheme.SelectableTint;
            if (i < timelineAccents.Count && !timelineAccents[i].IsNullOrDestroyed())
                timelineAccents[i].SetActive(isSelected);
        }
        bool basic = IsVisible(Data.monolith_stability_basic_go);
        bool empowered = IsVisible(Data.monolith_stability_empower_go);
        if (!availabilityLabel.IsNullOrDestroyed())
        {
            Prefab.ApplyLabel(
                availabilityLabel,
                basic || empowered
                    ? "Only unlocked timeline values are shown."
                    : "This timeline has no saved run yet."
            );
        }
        if (!Data.monolith_corruption_all_button.IsNullOrDestroyed())
        {
            Data.monolith_corruption_all_button.interactable = basic || empowered;
        }
    }

    public static void AttachTo(GameObject parent)
    {
        if (root.IsNullOrDestroyed() || parent.IsNullOrDestroyed())
            return;
        root.transform.SetParent(parent.transform, false);
        var rect = root.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(0f, EditorHeight);
        var layout = root.GetComponent<LayoutElement>();
        if (layout.IsNullOrDestroyed())
            layout = root.AddComponent<LayoutElement>();
        layout.ignoreLayout = false;
        layout.minHeight = EditorHeight;
        layout.preferredHeight = EditorHeight;
        layout.flexibleHeight = 0f;
    }

    static bool IsVisible(GameObject row)
    {
        return !row.IsNullOrDestroyed() && row.activeSelf;
    }

    static GameObject CreateObject(string name, GameObject parent)
    {
        GameObject obj = new GameObject(name);
        obj.AddComponent<RectTransform>();
        obj.layer = parent.layer;
        obj.transform.SetParent(parent.transform, false);
        return obj;
    }

    static GameObject CreatePanel(string name, GameObject parent, float left, float right)
    {
        GameObject panel = CreateObject(name, parent);
        Place(panel, left, right, 0f, EditorHeight, 4f, 4f);
        Image image = panel.AddComponent<Image>();
        image.color = PanelColor;
        image.raycastTarget = false;
        Outline border = panel.AddComponent<Outline>();
        border.effectColor = Gold;
        border.effectDistance = new Vector2(1f, -1f);
        return panel;
    }

    static Text CreateLabel(GameObject parent, Text template, string text, float top, float height)
    {
        GameObject obj = CreateObject("Label", parent);
        Place(obj, 0f, 1f, top, height, 12f, 12f);
        Text label = obj.AddComponent<Text>();
        label.font = template.font;
        label.fontSize = 16;
        label.color = HudTheme.TextPrimary;
        label.alignment = TextAnchor.MiddleLeft;
        label.horizontalOverflow = HorizontalWrapMode.Wrap;
        label.verticalOverflow = VerticalWrapMode.Truncate;
        label.raycastTarget = false;
        if (!string.IsNullOrEmpty(text))
        {
            Prefab.ApplyLabel(label, text);
        }
        return label;
    }

    static Button CreateButton(
        GameObject parent,
        Text template,
        string text,
        float top,
        float height
    )
    {
        GameObject obj = CreateObject("Btn_" + text.Replace(' ', '_'), parent);
        Place(obj, 0f, 1f, top, height, 10f, 10f);
        Image image = obj.AddComponent<Image>();
        image.sprite = null;
        image.type = Image.Type.Simple;
        image.color = HudTheme.Surface;
        Button button = obj.AddComponent<Button>();
        button.targetGraphic = image;
        StyleButton(button);
        Text label = CreateLabel(obj, template, text, 0f, height);
        label.alignment = TextAnchor.MiddleCenter;
        label.resizeTextForBestFit = true;
        label.resizeTextMinSize = 11;
        label.resizeTextMaxSize = 16;
        return button;
    }

    static void MoveValue(
        GameObject row,
        Il2CppTMPro.TMP_InputField input,
        Slider slider,
        GameObject parent,
        string title,
        float top
    )
    {
        if (row.IsNullOrDestroyed())
        {
            return;
        }
        row.transform.SetParent(parent.transform, false);
        Place(row, 0f, 1f, top, 72f, 12f, 12f);
        GameObject labelObject = Prefab.Child(row, "Label");
        if (!labelObject.IsNullOrDestroyed())
        {
            Place(labelObject, 0f, 1f, 0f, 26f);
            Text label = labelObject.GetComponent<Text>();
            if (!label.IsNullOrDestroyed())
            {
                Prefab.ApplyLabel(label, title);
            }
        }
        if (!input.IsNullOrDestroyed())
        {
            Place(input.gameObject, 0f, 1f, 32f, 32f);
            StyleInput(input);
        }
        else if (!slider.IsNullOrDestroyed())
        {
            Place(slider.gameObject, 0f, 1f, 32f, 32f);
            HudStyler.ApplySlider(slider);
        }
    }

    static void StyleButton(Button button)
    {
        if (button.IsNullOrDestroyed())
            return;
        var image = button.GetComponent<Image>();
        if (!image.IsNullOrDestroyed())
        {
            image.sprite = null;
            image.type = Image.Type.Simple;
            image.color = HudTheme.Surface;
        }
        var oldOutline = button.GetComponent<Outline>();
        if (!oldOutline.IsNullOrDestroyed())
            oldOutline.enabled = false;
        HudStyler.AddPrimaryBorder(button.gameObject);
        button.targetGraphic = image;
        button.colors = HudTheme.ActionButtonColors(HudTheme.Surface, HudTheme.Selection);
        foreach (var text in button.GetComponentsInChildren<Text>(true))
            text.color = HudTheme.TextPrimary;
        foreach (var text in button.GetComponentsInChildren<Il2CppTMPro.TMP_Text>(true))
            text.color = HudTheme.TextPrimary;
    }

    static GameObject CreateSelectionAccent(GameObject button)
    {
        var accent = CreateObject("SelectedAccent", button);
        var rect = accent.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(4f, 0f);
        var image = accent.AddComponent<Image>();
        image.color = HudTheme.Accent;
        image.raycastTarget = false;
        accent.transform.SetAsLastSibling();
        accent.SetActive(false);
        return accent;
    }

    static void StyleInput(Il2CppTMPro.TMP_InputField input)
    {
        var background = input.GetComponent<Image>();
        if (!background.IsNullOrDestroyed())
        {
            background.sprite = null;
            background.type = Image.Type.Simple;
            background.color = HudTheme.ControlBox;
        }
        var oldOutline = input.GetComponent<Outline>();
        if (!oldOutline.IsNullOrDestroyed())
            oldOutline.enabled = false;
        HudStyler.AddPrimaryBorder(input.gameObject, 1f);
        input.colors = HudTheme.ButtonColors(HudTheme.ControlBox, HudTheme.Selection);
        if (!input.placeholder.IsNullOrDestroyed())
            input.placeholder.gameObject.SetActive(false);
        if (!input.textViewport.IsNullOrDestroyed())
        {
            input.textViewport.anchorMin = Vector2.zero;
            input.textViewport.anchorMax = Vector2.one;
            input.textViewport.offsetMin = new Vector2(8f, 0f);
            input.textViewport.offsetMax = new Vector2(-10f, 0f);
        }
        if (!input.textComponent.IsNullOrDestroyed())
        {
            input.textComponent.color = HudTheme.TextPrimary;
            input.textComponent.fontSize = HudTheme.ValueFontSize;
            input.textComponent.horizontalAlignment = Il2CppTMPro.HorizontalAlignmentOptions.Right;
            input.textComponent.verticalAlignment = Il2CppTMPro.VerticalAlignmentOptions.Middle;
        }
    }

    static void Place(
        GameObject obj,
        float left,
        float right,
        float top,
        float height,
        float leftInset = 0f,
        float rightInset = 0f
    )
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(left, 1f);
        rect.anchorMax = new Vector2(right, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(-leftInset - rightInset, height);
        rect.anchoredPosition = new Vector2((leftInset - rightInset) * 0.5f, -top);
        rect.localScale = Vector3.one;
        LayoutElement layout = obj.GetComponent<LayoutElement>();
        if (!layout.IsNullOrDestroyed())
        {
            layout.ignoreLayout = true;
            layout.flexibleHeight = 0f;
        }
    }
}
