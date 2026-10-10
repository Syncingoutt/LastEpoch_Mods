using System.Collections;
using LastEpoch_Hud.Scripts.ModUI.Shell;
using UnityEngine;
using UnityEngine.UI;
using Character = LastEpoch_Hud.Scripts.Hud_Manager.Content.Character;

namespace LastEpoch_Hud.Scripts.ModUI.Settings;

// Legacy action cleanup. Rebuilt pages own currency presentation, while the
// blessing group still reuses its native controls until that page is rebuilt.
internal static class CharacterActionControls
{
    public static void Bind(GameObject cheats, GameObject data)
    {
        HideCurrencyActions(
            new[]
            {
                Character.Cheats.add_runes_button,
                Character.Cheats.add_glyphs_button,
                Character.Cheats.add_shards_button,
                Character.Cheats.add_ancient_bones_button,
                Mods.Character.Character_MemoryAmber.AddButton,
                Character.Data.soul_add_button,
            }
        );
        Group(
            data,
            "BlessingActions",
            "Blessings",
            new[]
            {
                Character.Cheats.choose_blessings_button,
                Character.Cheats.discover_blessings_button,
                Character.Cheats.max_blessings_button,
                Character.Cheats.unlock_blessing_slots_button,
            }
        );
    }

    static void HideCurrencyActions(Button[] buttons)
    {
        foreach (var button in buttons)
        {
            if (button.IsNullOrDestroyed())
                continue;
            var parent = button.transform.parent;
            button.gameObject.SetActive(false);
            if (!parent.IsNullOrDestroyed() && parent.gameObject.name == "CharacterCurrencyActions")
                parent.gameObject.SetActive(false);
        }
    }

    static void Group(GameObject content, string name, string caption, Button[] buttons)
    {
        if (content.IsNullOrDestroyed() || !Prefab.Child(content, name).IsNullOrDestroyed())
            return;
        var sample = content.GetComponentInChildren<Text>(true);
        if (sample.IsNullOrDestroyed())
            return;
        int count = 0;
        foreach (var button in buttons)
            if (!button.IsNullOrDestroyed())
                count++;
        if (count == 0)
            return;
        const float headerHeight = 24;
        const float rowHeight = 20;
        const float rowSpacing = 2;
        float height = headerHeight + count * (rowHeight + rowSpacing);
        var section = Node(content, name);
        var rect = section.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(.5f, 1);
        rect.sizeDelta = new Vector2(0, height);
        var layout = section.AddComponent<LayoutElement>();
        layout.minHeight = layout.preferredHeight = height;
        layout.flexibleHeight = 0;
        var title = Node(section, "Title").AddComponent<Text>();
        title.font = sample.font;
        title.fontSize = 16;
        title.color = sample.color;
        title.alignment = TextAnchor.MiddleLeft;
        title.raycastTarget = false;
        Place(title.gameObject, 0, 1, 0, 20);
        LocaleRegistry.Apply(title, caption);
        var line = Node(section, "Separator").AddComponent<Image>();
        line.color = HudTheme.Accent;
        line.raycastTarget = false;
        Place(line.gameObject, 0, 1, 21, 1);
        int index = 0;
        foreach (var button in buttons)
        {
            if (button.IsNullOrDestroyed())
                continue;
            var oldParent = button.transform.parent;
            button.transform.SetParent(section.transform, false);
            Place(
                button.gameObject,
                0,
                1,
                headerHeight + index * (rowHeight + rowSpacing),
                rowHeight
            );
            var oldLayout = button.GetComponent<LayoutElement>();
            if (!oldLayout.IsNullOrDestroyed())
                oldLayout.ignoreLayout = true;
            foreach (var text in button.GetComponentsInChildren<Text>(true))
            {
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 10;
                text.resizeTextMaxSize = 14;
                text.horizontalOverflow = HorizontalWrapMode.Wrap;
                text.alignment = TextAnchor.MiddleCenter;
            }
            foreach (var text in button.GetComponentsInChildren<Il2CppTMPro.TMP_Text>(true))
            {
                text.enableAutoSizing = true;
                text.fontSizeMin = 10;
                text.fontSizeMax = 14;
            }
            button.gameObject.SetActive(true);
            // These runtime wrappers have no purpose after all buttons are moved.
            if (
                !oldParent.IsNullOrDestroyed()
                && oldParent.childCount == 0
                && (
                    oldParent.name == "CharacterCurrencyActions"
                    || oldParent.name == "Blessing discovery actions"
                )
            )
                oldParent.gameObject.SetActive(false);
            index++;
        }
        section.transform.SetAsLastSibling();
        MelonLoader.MelonCoroutines.Start(PositionWhenVisible(content, section, height));
    }

    static GameObject Node(GameObject parent, string name)
    {
        var node = new GameObject(name);
        node.layer = parent.layer;
        node.AddComponent<RectTransform>().SetParent(parent.transform, false);
        return node;
    }

    static void Place(GameObject node, float left, float right, float top, float height)
    {
        var rect = node.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(left, 1);
        rect.anchorMax = new Vector2(right, 1);
        rect.pivot = new Vector2(.5f, 1);
        rect.anchoredPosition = new Vector2(0, -top);
        rect.sizeDelta = new Vector2(0, height);
        rect.localScale = Vector3.one;
    }

    static IEnumerator PositionWhenVisible(GameObject content, GameObject section, float height)
    {
        while (!content.IsNullOrDestroyed() && !content.activeInHierarchy)
            yield return null;
        if (content.IsNullOrDestroyed() || section.IsNullOrDestroyed())
            yield break;
        yield return null;
        if (content.IsNullOrDestroyed() || section.IsNullOrDestroyed())
            yield break;
        Canvas.ForceUpdateCanvases();
        var parent = content.GetComponent<RectTransform>();
        if (content.GetComponent<VerticalLayoutGroup>().IsNullOrDestroyed())
        {
            float bottom = parent.rect.yMax;
            var corners = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<Vector3>(
                4
            );
            for (int i = 0; i < content.transform.childCount; i++)
            {
                var child = content.transform.GetChild(i).gameObject;
                if (child == section || !child.activeSelf)
                    continue;
                var childRect = child.GetComponent<RectTransform>();
                if (childRect.IsNullOrDestroyed())
                    continue;
                childRect.GetWorldCorners(corners);
                bottom = Mathf.Min(bottom, content.transform.InverseTransformPoint(corners[0]).y);
            }
            var rect = section.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, bottom - parent.rect.yMax - 8);
            parent.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                Mathf.Max(parent.rect.height, -rect.anchoredPosition.y + height)
            );
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
    }
}
