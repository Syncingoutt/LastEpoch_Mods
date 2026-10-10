using System;
using System.Collections.Generic;
using LastEpoch_Hud.Scripts.ModUI.Pages;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.ModUI;

// Runtime composition layer for the HUD. It deliberately moves (rather than
// clones) the existing prefab panels so every legacy reference and event binding
// continues to point at the same functional controls.
public static class HudLayout
{
    private sealed class NavigationButton
    {
        public Button Button;
        public Image Background;
        public GameObject Accent;
    }

    private sealed class NavigationSection
    {
        public HudSectionDefinition Definition;
        public Text Indicator;
        public readonly List<GameObject> PageRows = new();
        public bool Expanded;
    }

    private sealed class RowContainer
    {
        public GameObject Root;
        public readonly List<GameObject> Children = new();
        public readonly Dictionary<GameObject, bool> InitiallyActive = new();
    }

    private sealed class PanelState
    {
        public readonly string Id;
        public readonly GameObject Root;
        public readonly Dictionary<GameObject, bool> DirectChildren = new();
        public readonly List<RowContainer> RowContainers = new();
        private readonly List<(Text Text, string Caption)> titleTexts = new();

        public PanelState(string id, GameObject root)
        {
            Id = id;
            Root = root;
            CaptureDirectChildren();
            CaptureRowContainers();
            CaptureTitleTexts();
        }

        public void Restore()
        {
            foreach (var child in DirectChildren)
                if (!child.Key.IsNullOrDestroyed())
                    child.Key.SetActive(child.Value);
            foreach (var container in RowContainers)
            foreach (var child in container.Children)
                if (!child.IsNullOrDestroyed())
                    child.SetActive(
                        container.InitiallyActive[child]
                            && !child.name.StartsWith("Border", StringComparison.Ordinal)
                    );
            foreach (var title in titleTexts)
                if (!title.Text.IsNullOrDestroyed())
                    LocaleRegistry.Apply(title.Text, title.Caption);
        }

        public void ApplyVisibleChildren(string[] names)
        {
            if (names == null)
                return;
            var visible = new HashSet<string>(names, StringComparer.Ordinal);
            foreach (var child in DirectChildren)
                if (!child.Key.IsNullOrDestroyed())
                    child.Key.SetActive(child.Value && visible.Contains(child.Key.name));
        }

        public void ApplyVisibleRows(string[] names)
        {
            if (names == null)
                return;
            var visible = new HashSet<string>(names, StringComparer.Ordinal);
            foreach (var container in RowContainers)
            {
                bool previousRowVisible = false;
                foreach (var child in container.Children)
                {
                    bool show;
                    if (child.name.StartsWith("Border", StringComparison.Ordinal))
                        show = false;
                    else
                    {
                        show = visible.Contains(child.name);
                        previousRowVisible = show;
                    }
                    child.SetActive(container.InitiallyActive[child] && show);
                }
            }
        }

        public float MeasureAndArrange()
        {
            if (Id == HudPanelIds.ForceDrop)
                return 720f;
            if (Id == HudPanelIds.WorldMisc)
                return 620f;

            float top = 12f;
            for (int i = 0; i < Root.transform.childCount; i++)
            {
                var child = Root.transform.GetChild(i).gameObject;
                if (!child.activeSelf)
                    continue;
                float height = 0f;
                if (child.name.Contains("Title", StringComparison.Ordinal))
                    height = HudTheme.CardTitleHeight;
                else if (!child.GetComponent<ScrollRect>().IsNullOrDestroyed())
                    height = MeasureRows(child);
                else if (
                    !child.GetComponent<Button>().IsNullOrDestroyed()
                    || !child.GetComponentInChildren<Button>(true).IsNullOrDestroyed()
                )
                    height = HudTheme.ButtonRowHeight + 10f;
                if (height <= 0f)
                    continue;
                PlaceDirectChild(child.GetComponent<RectTransform>(), top, height);
                top += height;
            }
            return Mathf.Max(top + 12f, 140f);
        }

        public void ApplyTitle(string caption)
        {
            if (string.IsNullOrEmpty(caption))
                return;
            foreach (var title in titleTexts)
                if (!title.Text.IsNullOrDestroyed())
                    LocaleRegistry.Apply(title.Text, caption);
        }

        private void CaptureDirectChildren()
        {
            for (int i = 0; i < Root.transform.childCount; i++)
            {
                var child = Root.transform.GetChild(i).gameObject;
                DirectChildren[child] = child.activeSelf;
            }
        }

        private void CaptureRowContainers()
        {
            Prefab.ForEachDescendant(
                Root,
                candidate =>
                {
                    if (candidate.name != "Content" || candidate.transform.parent == null)
                        return;
                    if (candidate.transform.parent.gameObject.name != "Viewport")
                        return;
                    var rows = new RowContainer { Root = candidate };
                    for (int i = 0; i < candidate.transform.childCount; i++)
                    {
                        var child = candidate.transform.GetChild(i).gameObject;
                        rows.Children.Add(child);
                        rows.InitiallyActive[child] = child.activeSelf;
                    }
                    if (rows.Children.Count > 0)
                        RowContainers.Add(rows);
                }
            );
        }

        private void CaptureTitleTexts()
        {
            var title = Prefab.Child(Root, "Title");
            if (title.IsNullOrDestroyed())
                return;
            foreach (var text in title.GetComponentsInChildren<Text>(true))
                titleTexts.Add((text, text.text));
        }

        private float MeasureRows(GameObject scope)
        {
            float height = 16f;
            foreach (var container in RowContainers)
            {
                if (!container.Root.transform.IsChildOf(scope.transform))
                    continue;
                foreach (var child in container.Children)
                {
                    if (
                        !child.activeSelf
                        || child.name.StartsWith("Border", StringComparison.Ordinal)
                    )
                        continue;
                    var element = child.GetComponent<LayoutElement>();
                    height += element.IsNullOrDestroyed()
                        ? HudTheme.RowHeight
                        : element.preferredHeight;
                }
            }

            var scroll = scope.GetComponent<ScrollRect>();
            if (!scroll.IsNullOrDestroyed())
            {
                scroll.horizontal = false;
                scroll.vertical = false;
                if (!scroll.viewport.IsNullOrDestroyed())
                {
                    scroll.viewport.anchorMin = Vector2.zero;
                    scroll.viewport.anchorMax = Vector2.one;
                    scroll.viewport.offsetMin = Vector2.zero;
                    scroll.viewport.offsetMax = Vector2.zero;
                }
                if (!scroll.horizontalScrollbar.IsNullOrDestroyed())
                    scroll.horizontalScrollbar.gameObject.SetActive(false);
                if (!scroll.verticalScrollbar.IsNullOrDestroyed())
                    scroll.verticalScrollbar.gameObject.SetActive(false);
                // The page owns scrolling now. Disabling nested ScrollRects lets
                // wheel/drag events bubble to the single outer scroller.
                scroll.enabled = false;
            }
            return Mathf.Max(height, 62f);
        }

        private static void PlaceDirectChild(RectTransform rect, float top, float height)
        {
            if (rect.IsNullOrDestroyed())
                return;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -top);
            rect.sizeDelta = new Vector2(-24f, height);
        }
    }

    private static readonly Dictionary<string, PanelState> panels = new();
    private static readonly Dictionary<string, NavigationButton> pageButtons = new();
    private static readonly List<NavigationSection> navigationSections = new();
    private static GameObject boundHud;
    private static GameObject window;
    private static GameObject stage;
    private static ScrollRect mainScroll;
    private static Font font;
    private static HudPageDefinition activePage;
    private static GameObject settingsPanel;
    private static Slider settingsFontSlider;
    private static Text settingsFontValue;
    private static Button lightModeButton;
    private static Button darkModeButton;

    public static void Initialize(GameObject hud)
    {
        if (hud.IsNullOrDestroyed() || boundHud == hud)
            return;

        boundHud = hud;
        panels.Clear();
        pageButtons.Clear();
        navigationSections.Clear();
        HudSearch.Reset();
        activePage = null;
        HudTheme.LoadPreferences();
        HudTheme.ResetFontBaselines();
        font = FindFont(hud);

        var content = Prefab.Child(hud, "Content");
        var menu = Prefab.Child(hud, "Menu");
        if (content.IsNullOrDestroyed() || menu.IsNullOrDestroyed())
        {
            Main.logger_instance?.Warning("HudLayout: missing Content or Menu root");
            return;
        }

        window = BuildWindow(hud);
        menu.transform.SetParent(window.transform, false);
        content.transform.SetParent(window.transform, false);
        BuildHeader(window);
        // Build and validate the replacement navigation first. Content rebuilding
        // is deliberately deferred so this pass cannot fall back to the old menu.
        BuildNavigation(menu);
        PlaceLegacyContent(content);
        HideLegacyMultiplierRows(content);
        HideLegacyBuffPanel(content);
        Utilities_Character.Build(window, hud, font);
        Utilities_Multipliers.Build(window, hud, font);
        Utilities_Buffs.Build(window, hud, font);
        Utilities_QOL.Build(window, hud, font);
        Utilities_Currency.Build(window, font);
        Items_Drop.Build(window, hud, font);
        Items_CraftingSlot.Build(window, hud, font);
        Items_CustomItems.Build(window, hud, font);
        World_Difficulty.Build(window, hud, font);
        World_Monoliths.Build(window, hud, font);
        World_Misc.Build(window, hud, font);
        World_Camera.Build(window, hud, font);
        Skills_Minions.Build(window, hud, font);
        Skills_Companions.Build(window, hud, font);
        Skills_Summon.Build(window, hud, font);
        Skills_QOL.Build(window, hud, font);

        var defaultPage = HudNavigation.Sections[0].Pages[0];
        ExpandSection(HudNavigation.Sections[0].Id, true);
        ActivateNavigationOnly(defaultPage);
        HudTheme.NormalizeSelectableGraphics(window);
        HudTheme.ApplyFontScale(window);
        RefreshSettingsControls();
        Main.logger_instance?.Msg("HudLayout: replacement navigation initialized");
    }

    private static GameObject BuildWindow(GameObject hud)
    {
        var backdrop = Node(hud, "LEHUD_Backdrop");
        Stretch(backdrop.GetComponent<RectTransform>());
        var image = backdrop.AddComponent<Image>();
        image.color = HudTheme.Backdrop;
        image.raycastTarget = false;
        backdrop.transform.SetAsFirstSibling();

        var windowObject = Node(hud, "LEHUD_Window");
        var windowRect = windowObject.GetComponent<RectTransform>();
        windowRect.anchorMin = HudTheme.WindowAnchorMin;
        windowRect.anchorMax = HudTheme.WindowAnchorMax;
        windowRect.offsetMin = Vector2.zero;
        windowRect.offsetMax = Vector2.zero;
        var windowImage = windowObject.AddComponent<Image>();
        windowImage.color = HudTheme.Background;
        var outline = windowObject.AddComponent<Outline>();
        outline.effectColor = HudTheme.Border;
        outline.effectDistance = new Vector2(1f, -1f);
        return windowObject;
    }

    private static void BuildHeader(GameObject hud)
    {
        var header = Node(hud, "LEHUD_Header");
        var rect = header.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(0f, HudTheme.HeaderHeight);
        var background = header.AddComponent<Image>();
        background.color = HudTheme.Header;
        background.raycastTarget = false;

        var brand = TextNode(header, "Brand", "LE HUD MOD", HudTheme.BrandFontSize);
        var brandRect = brand.GetComponent<RectTransform>();
        brandRect.anchorMin = Vector2.zero;
        brandRect.anchorMax = Vector2.one;
        brandRect.offsetMin = new Vector2(26f, 0f);
        brandRect.offsetMax = new Vector2(-HudTheme.HeaderHeight, 0f);
        brand.alignment = TextAnchor.MiddleLeft;
        brand.color = HudTheme.TextPrimary;

        var closeObject = Node(header, "Close");
        var closeRect = closeObject.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1f, 0.5f);
        closeRect.anchorMax = new Vector2(1f, 0.5f);
        closeRect.pivot = new Vector2(1f, 0.5f);
        closeRect.anchoredPosition = new Vector2(-24f, 0f);
        closeRect.sizeDelta = new Vector2(48f, 48f);
        var closeImage = closeObject.AddComponent<Image>();
        closeImage.color = HudTheme.Surface;
        var outline = closeObject.AddComponent<Outline>();
        outline.effectColor = HudTheme.Border;
        outline.effectDistance = new Vector2(1f, -1f);
        var close = closeObject.AddComponent<Button>();
        close.targetGraphic = closeImage;
        close.colors = HudTheme.ButtonColors(HudTheme.Surface, HudTheme.SurfaceHover);
        Prefab.BindButton(close, new Action(Close));
        var closeText = TextNode(closeObject, "Label", "X", 25);
        Stretch(closeText.GetComponent<RectTransform>());
        closeText.alignment = TextAnchor.MiddleCenter;
        closeText.color = HudTheme.TextPrimary;

        var settingsObject = Node(header, "Settings");
        var settingsRect = settingsObject.GetComponent<RectTransform>();
        settingsRect.anchorMin = new Vector2(1f, 0.5f);
        settingsRect.anchorMax = new Vector2(1f, 0.5f);
        settingsRect.pivot = new Vector2(1f, 0.5f);
        settingsRect.anchoredPosition = new Vector2(-84f, 0f);
        settingsRect.sizeDelta = new Vector2(48f, 48f);
        var settingsImage = settingsObject.AddComponent<Image>();
        settingsImage.color = HudTheme.Surface;
        HudStyler.AddPrimaryBorder(settingsObject, 1f);
        var settingsButton = settingsObject.AddComponent<Button>();
        settingsButton.targetGraphic = settingsImage;
        settingsButton.colors = HudTheme.ButtonColors(HudTheme.Surface, HudTheme.Selection);
        ButtonHook.Register(settingsButton, ToggleSettings);
        var settingsIcon = TextNode(settingsObject, "Label", "⚙", 27);
        Stretch(settingsIcon.GetComponent<RectTransform>());
        settingsIcon.alignment = TextAnchor.MiddleCenter;
        settingsIcon.color = HudTheme.TextPrimary;

        var line = Node(header, "Border");
        var lineRect = line.GetComponent<RectTransform>();
        lineRect.anchorMin = new Vector2(0f, 0f);
        lineRect.anchorMax = new Vector2(1f, 0f);
        lineRect.pivot = new Vector2(0.5f, 0f);
        lineRect.anchoredPosition = Vector2.zero;
        lineRect.sizeDelta = new Vector2(0f, HudTheme.BorderWidth);
        var lineImage = line.AddComponent<Image>();
        lineImage.color = HudTheme.Border;
        lineImage.raycastTarget = false;

        BuildSettingsPanel(hud);
        HudSearchBar.Build(header, hud, boundHud, font, ActivateSearchMatch);
    }

    private static void BuildSettingsPanel(GameObject parent)
    {
        settingsPanel = Node(parent, "LEHUD_SettingsPanel");
        var rect = settingsPanel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-24f, -HudTheme.HeaderHeight - 12f);
        rect.sizeDelta = new Vector2(420f, 250f);
        var image = settingsPanel.AddComponent<Image>();
        image.color = HudTheme.Surface;
        HudStyler.AddPrimaryBorder(settingsPanel);

        var title = TextNode(settingsPanel, "Title", "Settings", HudTheme.CardTitleFontSize);
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.offsetMin = new Vector2(22f, -54f);
        titleRect.offsetMax = new Vector2(-22f, -8f);
        title.alignment = TextAnchor.MiddleLeft;

        var divider = Node(settingsPanel, "TitleDivider");
        var dividerRect = divider.GetComponent<RectTransform>();
        dividerRect.anchorMin = new Vector2(0f, 1f);
        dividerRect.anchorMax = new Vector2(1f, 1f);
        dividerRect.pivot = new Vector2(0.5f, 1f);
        dividerRect.anchoredPosition = new Vector2(0f, -58f);
        dividerRect.sizeDelta = new Vector2(-44f, HudTheme.BorderWidth);
        var dividerImage = divider.AddComponent<Image>();
        dividerImage.color = HudTheme.CardDivider;
        dividerImage.raycastTarget = false;

        var fontLabel = TextNode(
            settingsPanel,
            "FontSizeLabel",
            "Font Size",
            HudTheme.BodyFontSize
        );
        var fontLabelRect = fontLabel.GetComponent<RectTransform>();
        fontLabelRect.anchorMin = new Vector2(0f, 1f);
        fontLabelRect.anchorMax = new Vector2(1f, 1f);
        fontLabelRect.pivot = new Vector2(0.5f, 1f);
        fontLabelRect.offsetMin = new Vector2(22f, -98f);
        fontLabelRect.offsetMax = new Vector2(-110f, -66f);
        fontLabel.alignment = TextAnchor.MiddleLeft;

        settingsFontValue = TextNode(
            settingsPanel,
            "FontSizeValue",
            "100%",
            HudTheme.ValueFontSize
        );
        var valueRect = settingsFontValue.GetComponent<RectTransform>();
        valueRect.anchorMin = new Vector2(1f, 1f);
        valueRect.anchorMax = new Vector2(1f, 1f);
        valueRect.pivot = new Vector2(1f, 1f);
        valueRect.anchoredPosition = new Vector2(-22f, -66f);
        valueRect.sizeDelta = new Vector2(82f, 32f);
        settingsFontValue.alignment = TextAnchor.MiddleRight;

        settingsFontSlider = BuildSettingsSlider(settingsPanel);

        var modeLabel = TextNode(settingsPanel, "ModeLabel", "Appearance", HudTheme.BodyFontSize);
        var modeRect = modeLabel.GetComponent<RectTransform>();
        modeRect.anchorMin = new Vector2(0f, 1f);
        modeRect.anchorMax = new Vector2(1f, 1f);
        modeRect.pivot = new Vector2(0.5f, 1f);
        modeRect.offsetMin = new Vector2(22f, -166f);
        modeRect.offsetMax = new Vector2(-22f, -134f);
        modeLabel.alignment = TextAnchor.MiddleLeft;

        lightModeButton = BuildSettingsChoice(
            settingsPanel,
            "LightMode",
            "Light Mode",
            22f,
            6f,
            () => SetLightMode(true)
        );
        darkModeButton = BuildSettingsChoice(
            settingsPanel,
            "DarkMode",
            "Dark Mode",
            6f,
            22f,
            () => SetLightMode(false)
        );
        settingsPanel.SetActive(false);
    }

    private static Slider BuildSettingsSlider(GameObject parent)
    {
        var sliderObject = Node(parent, "FontSizeSlider");
        var rect = sliderObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -104f);
        rect.sizeDelta = new Vector2(-44f, 28f);

        var track = Node(sliderObject, "Track");
        var trackRect = track.GetComponent<RectTransform>();
        trackRect.anchorMin = new Vector2(0f, 0.5f);
        trackRect.anchorMax = new Vector2(1f, 0.5f);
        trackRect.sizeDelta = new Vector2(-8f, HudTheme.SliderTrackHeight);
        track.AddComponent<Image>().color = HudTheme.ControlTrack;

        var handleArea = Node(sliderObject, "Handle Slide Area");
        Stretch(handleArea.GetComponent<RectTransform>());
        var handle = Node(handleArea, "Handle");
        var handleRect = handle.GetComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0f, 0.5f);
        handleRect.anchorMax = new Vector2(0f, 0.5f);
        handleRect.pivot = new Vector2(0.5f, 0.5f);
        handleRect.sizeDelta = new Vector2(HudTheme.SliderHandleSize, HudTheme.SliderHandleSize);
        var handleImage = handle.AddComponent<Image>();
        handleImage.color = HudTheme.ControlHandle;

        var slider = sliderObject.AddComponent<Slider>();
        slider.minValue = 80f;
        slider.maxValue = 140f;
        slider.wholeNumbers = true;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        slider.value = Mathf.Round(HudTheme.FontScale * 100f);
        slider.colors = HudTheme.ButtonColors(HudTheme.ControlHandle, HudTheme.ControlHandle);
        SliderHook.Register(
            slider,
            value =>
            {
                float rounded = Mathf.Round(value / 5f) * 5f;
                if (Mathf.Abs(slider.value - rounded) > 0.01f)
                {
                    slider.SetValueWithoutNotify(rounded);
                }
                HudTheme.SetFontScale(window, rounded / 100f);
                RefreshSettingsControls();
            }
        );
        return slider;
    }

    private static Button BuildSettingsChoice(
        GameObject parent,
        string name,
        string caption,
        float leftInset,
        float rightInset,
        Action click
    )
    {
        var obj = Node(parent, name);
        var rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(name == "LightMode" ? 0f : 0.5f, 1f);
        rect.anchorMax = new Vector2(name == "LightMode" ? 0.5f : 1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2((leftInset - rightInset) * 0.5f, -180f);
        rect.sizeDelta = new Vector2(-leftInset - rightInset - 6f, 48f);
        var image = obj.AddComponent<Image>();
        image.color = HudTheme.Surface;
        HudStyler.AddPrimaryBorder(obj);
        var button = obj.AddComponent<Button>();
        button.targetGraphic = image;
        button.colors = HudTheme.ActionButtonColors(HudTheme.Surface, HudTheme.Selection);
        ButtonHook.Register(button, click);
        var label = TextNode(obj, "Label", caption, HudTheme.ValueFontSize);
        Stretch(label.GetComponent<RectTransform>());
        label.alignment = TextAnchor.MiddleCenter;
        return button;
    }

    private static void ToggleSettings()
    {
        if (settingsPanel.IsNullOrDestroyed())
            return;
        bool show = !settingsPanel.activeSelf;
        settingsPanel.SetActive(show);
        if (show)
        {
            settingsPanel.transform.SetAsLastSibling();
            RefreshSettingsControls();
        }
    }

    private static void SetLightMode(bool enabled)
    {
        HudTheme.SetLightMode(window, enabled);
        if (activePage != null)
            SetSelected(activePage.Id);
        MonolithTimelineEditor.RefreshSelection();
        RefreshSettingsControls();
    }

    private static void RefreshSettingsControls()
    {
        if (!settingsFontSlider.IsNullOrDestroyed())
            settingsFontSlider.SetValueWithoutNotify(Mathf.Round(HudTheme.FontScale * 100f));
        if (!settingsFontValue.IsNullOrDestroyed())
            settingsFontValue.text = Mathf.RoundToInt(HudTheme.FontScale * 100f) + "%";
        StyleSettingsChoice(darkModeButton, !HudTheme.LightMode);
        StyleSettingsChoice(lightModeButton, HudTheme.LightMode);
    }

    private static void StyleSettingsChoice(Button button, bool selected)
    {
        if (button.IsNullOrDestroyed())
            return;
        Color normal = selected ? HudTheme.Selection : HudTheme.Surface;
        var image = button.GetComponent<Image>();
        if (!image.IsNullOrDestroyed())
            image.color = Color.white;
        button.colors = HudTheme.ActionButtonColors(
            normal,
            selected ? HudTheme.Selection : HudTheme.SurfaceHover
        );
    }

    private static void BuildStage(GameObject content)
    {
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = new Vector2(HudTheme.SidebarWidth, 0f);
        contentRect.offsetMax = new Vector2(0f, -HudTheme.HeaderHeight);
        var contentBackground = content.GetComponent<Image>();
        if (contentBackground.IsNullOrDestroyed())
            contentBackground = content.AddComponent<Image>();
        contentBackground.color = HudTheme.Background;

        var scrollObject = Node(content, "LEHUD_MainScroll");
        var scrollRect = scrollObject.GetComponent<RectTransform>();
        Stretch(scrollRect);
        scrollRect.offsetMin = new Vector2(HudTheme.ContentPadding, HudTheme.ContentPadding);
        scrollRect.offsetMax = new Vector2(-HudTheme.ContentPadding, -HudTheme.ContentPadding);
        mainScroll = scrollObject.AddComponent<ScrollRect>();
        mainScroll.horizontal = false;
        mainScroll.vertical = true;
        mainScroll.movementType = ScrollRect.MovementType.Clamped;
        mainScroll.scrollSensitivity = 34f;

        var viewport = Node(scrollObject, "Viewport");
        var viewportRect = viewport.GetComponent<RectTransform>();
        Stretch(viewportRect);
        viewport.AddComponent<RectMask2D>();

        stage = Node(viewport, "LEHUD_ContentStage");
        var stageRect = stage.GetComponent<RectTransform>();
        stageRect.anchorMin = new Vector2(0f, 1f);
        stageRect.anchorMax = new Vector2(1f, 1f);
        stageRect.pivot = new Vector2(0.5f, 1f);
        stageRect.anchoredPosition = Vector2.zero;
        stageRect.sizeDelta = Vector2.zero;
        mainScroll.viewport = viewportRect;
        mainScroll.content = stageRect;
    }

    private static void PlaceLegacyContent(GameObject content)
    {
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = new Vector2(HudTheme.SidebarWidth, 0f);
        contentRect.offsetMax = new Vector2(0f, -HudTheme.HeaderHeight);
        var background = content.GetComponent<Image>();
        if (!background.IsNullOrDestroyed())
            background.color = HudTheme.Background;
    }

    private static void ResolvePanels(GameObject content)
    {
        foreach (var source in HudNavigation.PanelSources)
        {
            var panel = Prefab.ChildPath(content, source.Path);
            if (panel.IsNullOrDestroyed())
            {
                Main.logger_instance?.Warning(
                    "HudLayout: panel '" + source.Id + "' not found at " + source.Path
                );
                continue;
            }
            panel.transform.SetParent(stage.transform, false);
            var state = new PanelState(source.Id, panel);
            panels[source.Id] = state;
            HudStyler.ApplyPanel(panel);
            panel.SetActive(false);
        }
    }

    private static void BuildNavigation(GameObject menu)
    {
        var menuRect = menu.GetComponent<RectTransform>();
        menuRect.anchorMin = Vector2.zero;
        menuRect.anchorMax = new Vector2(0f, 1f);
        menuRect.pivot = new Vector2(0f, 0.5f);
        menuRect.offsetMin = Vector2.zero;
        menuRect.offsetMax = new Vector2(HudTheme.SidebarWidth, -HudTheme.HeaderHeight);

        var oldContent = Prefab.Child(menu, "Content");
        if (!oldContent.IsNullOrDestroyed())
            oldContent.SetActive(false);

        var background = menu.GetComponent<Image>();
        if (background.IsNullOrDestroyed())
            background = menu.AddComponent<Image>();
        background.color = HudTheme.Sidebar;

        var rightBorder = Node(menu, "LEHUD_RightBorder");
        var borderRect = rightBorder.GetComponent<RectTransform>();
        borderRect.anchorMin = new Vector2(1f, 0f);
        borderRect.anchorMax = Vector2.one;
        borderRect.pivot = new Vector2(1f, 0.5f);
        borderRect.anchoredPosition = Vector2.zero;
        borderRect.sizeDelta = new Vector2(HudTheme.BorderWidth, 0f);
        var border = rightBorder.AddComponent<Image>();
        border.color = HudTheme.Border;
        border.raycastTarget = false;

        var list = Node(menu, "LEHUD_Navigation");
        var listRect = list.GetComponent<RectTransform>();
        listRect.anchorMin = new Vector2(0f, 1f);
        listRect.anchorMax = new Vector2(1f, 1f);
        listRect.pivot = new Vector2(0.5f, 1f);
        listRect.anchoredPosition = Vector2.zero;
        listRect.sizeDelta = Vector2.zero;
        listRect.offsetMin = new Vector2(0f, 0f);
        listRect.offsetMax = new Vector2(0f, 0f);

        var layout = list.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(0, 0, 0, 0);
        layout.spacing = 0f;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        var fitter = list.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

        foreach (var definition in HudNavigation.Sections)
        {
            var state = new NavigationSection { Definition = definition };
            navigationSections.Add(state);
            var sectionButton = CreateNavigationButton(
                list,
                "Section_" + definition.Id,
                definition.Label,
                HudTheme.SectionHeight,
                false,
                out var indicator,
                out _
            );
            state.Indicator = indicator;
            if (definition.Accordion)
                Prefab.BindButton(sectionButton, new Action(() => ToggleSection(state)));
            else
            {
                Prefab.BindButton(
                    sectionButton,
                    new Action(() => ActivateNavigationOnly(definition.Pages[0]))
                );
                pageButtons[definition.Pages[0].Id] = new NavigationButton
                {
                    Button = sectionButton,
                    Background = sectionButton.GetComponent<Image>(),
                };
            }

            foreach (var page in definition.Pages)
            {
                if (!definition.Accordion)
                    continue;
                var pageButton = CreateNavigationButton(
                    list,
                    "Page_" + page.Id,
                    page.Label,
                    HudTheme.PageHeight,
                    true,
                    out _,
                    out var accent
                );
                Prefab.BindButton(pageButton, new Action(() => ActivateNavigationOnly(page)));
                state.PageRows.Add(pageButton.gameObject);
                pageButtons[page.Id] = new NavigationButton
                {
                    Button = pageButton,
                    Background = Prefab
                        .Child(pageButton.gameObject, "ButtonSurface")
                        .GetComponent<Image>(),
                    Accent = accent,
                };
                pageButton.gameObject.SetActive(false);
            }
        }
        // Navigation rows span the full sidebar and otherwise cover this edge.
        rightBorder.transform.SetAsLastSibling();
    }

    private static Button CreateNavigationButton(
        GameObject parent,
        string name,
        string caption,
        float height,
        bool child,
        out Text indicator,
        out GameObject accent
    )
    {
        var row = Node(parent, name);
        var image = row.AddComponent<Image>();
        image.color = HudTheme.Surface;
        var button = row.AddComponent<Button>();
        var element = row.AddComponent<LayoutElement>();
        element.minHeight = height;
        element.preferredHeight = height;

        Image buttonSurface = image;
        if (child)
        {
            var surfaceObject = Node(row, "ButtonSurface");
            var surfaceRect = surfaceObject.GetComponent<RectTransform>();
            surfaceRect.anchorMin = Vector2.zero;
            surfaceRect.anchorMax = Vector2.one;
            surfaceRect.offsetMin = new Vector2(HudTheme.NavigationIndent - 10f, 0f);
            surfaceRect.offsetMax = Vector2.zero;
            buttonSurface = surfaceObject.AddComponent<Image>();
            buttonSurface.color = HudTheme.Surface;
        }
        button.targetGraphic = buttonSurface;
        button.colors = HudTheme.ButtonColors(HudTheme.Surface, HudTheme.SurfaceHover);

        var label = TextNode(
            row,
            "Label",
            caption,
            child ? HudTheme.PageFontSize : HudTheme.SectionFontSize
        );
        var labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(child ? HudTheme.NavigationIndent : 18f, 0f);
        labelRect.offsetMax = new Vector2(-46f, 0f);
        label.alignment = TextAnchor.MiddleLeft;
        label.color = child ? HudTheme.TextPrimary : HudTheme.TextPrimary;

        indicator = null;
        if (!child)
        {
            indicator = TextNode(row, "Indicator", string.Empty, HudTheme.SectionFontSize);
            var indicatorRect = indicator.GetComponent<RectTransform>();
            indicatorRect.anchorMin = new Vector2(1f, 0f);
            indicatorRect.anchorMax = Vector2.one;
            indicatorRect.pivot = new Vector2(1f, 0.5f);
            indicatorRect.offsetMin = new Vector2(-44f, 0f);
            indicatorRect.offsetMax = new Vector2(-14f, 0f);
            indicator.alignment = TextAnchor.MiddleCenter;
            indicator.color = HudTheme.TextPrimary;
        }

        accent = null;
        if (child)
        {
            var gutter = Node(row, "SubmenuGutter");
            var gutterRect = gutter.GetComponent<RectTransform>();
            gutterRect.anchorMin = Vector2.zero;
            gutterRect.anchorMax = new Vector2(0f, 1f);
            gutterRect.pivot = new Vector2(0f, 0.5f);
            gutterRect.sizeDelta = new Vector2(HudTheme.BorderWidth, 0f);
            gutterRect.anchoredPosition = new Vector2(HudTheme.NavigationIndent - 12f, 0f);
            var gutterImage = gutter.AddComponent<Image>();
            gutterImage.color = HudTheme.Border;
            gutterImage.raycastTarget = false;

            accent = Node(row, "SelectedAccent");
            var accentRect = accent.GetComponent<RectTransform>();
            accentRect.anchorMin = Vector2.zero;
            accentRect.anchorMax = new Vector2(0f, 1f);
            accentRect.pivot = new Vector2(0f, 0.5f);
            accentRect.sizeDelta = new Vector2(4f, 0f);
            accentRect.anchoredPosition = new Vector2(HudTheme.NavigationIndent - 12f, 0f);
            var accentImage = accent.AddComponent<Image>();
            accentImage.color = HudTheme.Accent;
            accentImage.raycastTarget = false;
            accent.SetActive(false);
        }

        var divider = Node(row, "Divider");
        var dividerRect = divider.GetComponent<RectTransform>();
        dividerRect.anchorMin = Vector2.zero;
        dividerRect.anchorMax = new Vector2(1f, 0f);
        dividerRect.pivot = new Vector2(0.5f, 0f);
        dividerRect.offsetMin = new Vector2(child ? HudTheme.NavigationIndent - 10f : 0f, 0f);
        dividerRect.offsetMax = Vector2.zero;
        dividerRect.sizeDelta = new Vector2(dividerRect.sizeDelta.x, HudTheme.BorderWidth);
        var dividerImage = divider.AddComponent<Image>();
        dividerImage.color = child ? HudTheme.Border : HudTheme.AccentMuted;
        dividerImage.raycastTarget = false;
        return button;
    }

    private static void ExpandSection(string id, bool expanded)
    {
        foreach (var section in navigationSections)
        {
            if (!section.Definition.Accordion)
                continue;
            bool show = section.Definition.Id == id && expanded;
            if (!expanded && section.Definition.Id != id)
                show = section.Expanded;
            section.Expanded = show;
            if (!section.Indicator.IsNullOrDestroyed())
                section.Indicator.text = show ? "-" : "+";
            foreach (var row in section.PageRows)
                if (!row.IsNullOrDestroyed())
                    row.SetActive(show);
        }
    }

    private static void ToggleSection(NavigationSection section)
    {
        if (section == null || !section.Definition.Accordion)
            return;
        bool expand = !section.Expanded;
        ExpandSection(section.Definition.Id, expand);
        if (expand && section.Definition.Pages.Length > 0)
            ActivateNavigationOnly(section.Definition.Pages[0]);
    }

    private static void ActivateNavigationOnly(HudPageDefinition page, bool preserveSearch = false)
    {
        if (page == null)
            return;
        if (!preserveSearch)
        {
            HudSearch.ClearAll();
            HudSearchBar.Clear();
        }
        Utilities_Character.Hide();
        Utilities_Multipliers.Hide();
        Utilities_Buffs.Hide();
        Utilities_QOL.Hide();
        Utilities_Currency.Hide();
        Items_Drop.Hide();
        Items_ForceDrop.Hide();
        Items_CraftingSlot.Hide();
        Items_CustomItems.Hide();
        World_Difficulty.Hide();
        World_Monoliths.Hide();
        World_Misc.Hide();
        World_Camera.Hide();
        Skills_Minions.Hide();
        Skills_Companions.Hide();
        Skills_Summon.Hide();
        Skills_QOL.Hide();
        if (page.Id == "character.main")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            Utilities_Character.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "character.multipliers")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            Utilities_Multipliers.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "character.buffs")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            Utilities_Buffs.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "character.qol")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            Utilities_QOL.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "character.currency")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            Utilities_Currency.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "items.drop")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            Items_Drop.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "items.force-drop")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            Items_ForceDrop.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "items.crafting")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            Items_CraftingSlot.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == Items_CustomItems.PageId)
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            Items_CustomItems.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "world.difficulty")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            World_Difficulty.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "world.monoliths")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            World_Monoliths.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "world.misc")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            World_Misc.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "world.camera")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            World_Camera.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "skills.minions")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            Skills_Minions.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "skills.companions")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            Skills_Companions.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "skills.summon")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            Skills_Summon.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }
        if (page.Id == "skills.qol")
        {
            SetLegacyAreas(Array.Empty<HudArea>());
            Skills_QOL.Show();
            SetSelected(page.Id);
            activePage = page;
            return;
        }

        // Until each page body is rebuilt, show one intact legacy content page
        // behind the new navigation instead of overlapping several old layouts.
        SetLegacyAreas(page.Areas.Length > 0 ? new[] { page.Areas[0] } : Array.Empty<HudArea>());
        SetSelected(page.Id);
        activePage = page;
    }

    private static void ActivateSearchMatch(HudSearchMatch match, string query)
    {
        if (
            match == null
            || string.IsNullOrWhiteSpace(query)
            || !HudNavigation.TryGetPage(match.PageId, out var section, out var page)
        )
            return;
        HudSearch.ClearAll();
        ExpandSection(section.Id, true);
        ActivateNavigationOnly(page, true);
        match.Page.ApplySearch(query);
    }

    public static void RefreshActivePage()
    {
        if (activePage?.Id == Items_CustomItems.PageId)
        {
            Items_CustomItems.Refresh();
            return;
        }

        if (activePage?.Id == "character.main")
            Utilities_Character.Refresh();
        else if (activePage?.Id == "character.qol")
            Utilities_QOL.Refresh();
        else if (activePage?.Id == "items.drop")
            Items_Drop.Refresh();
        else if (activePage?.Id == "items.crafting")
            Items_CraftingSlot.Refresh();
        else if (activePage?.Id == "world.difficulty")
            World_Difficulty.Refresh();
        else if (activePage?.Id == "world.monoliths")
            World_Monoliths.Refresh();
        else if (activePage?.Id == "world.misc")
            World_Misc.Refresh();
        else if (activePage?.Id == "world.camera")
            World_Camera.Refresh();
        else if (activePage?.Id == "skills.minions")
            Skills_Minions.Refresh();
        else if (activePage?.Id == "skills.companions")
            Skills_Companions.Refresh();
        else if (activePage?.Id == "skills.summon")
            Skills_Summon.Refresh();
        else if (activePage?.Id == "skills.qol")
            Skills_QOL.Refresh();
    }

    private static void HideLegacyMultiplierRows(GameObject content)
    {
        var legacyRows = Prefab.ChildPath(
            content,
            "Character_Content/Character_Cheats/Character_Cheats_Content/Viewport/Content"
        );
        if (legacyRows.IsNullOrDestroyed())
            return;

        var names = new HashSet<string>(StringComparer.Ordinal)
        {
            "DensityMultiplier",
            "ExperienceMultiplier",
            "AbilityMultiplier",
            "FavorMultiplier",
            "MemoryAmberMultiplier",
            "ItemDropMultiplier",
            "ItemDropChance",
            "GoldDropMultiplier",
            "GoldDropChance",
        };
        bool removedPreviousRow = false;
        for (int i = 0; i < legacyRows.transform.childCount; i++)
        {
            var child = legacyRows.transform.GetChild(i).gameObject;
            if (names.Contains(child.name))
            {
                child.SetActive(false);
                removedPreviousRow = true;
            }
            else if (
                removedPreviousRow && child.name.StartsWith("Border", StringComparison.Ordinal)
            )
            {
                child.SetActive(false);
                removedPreviousRow = false;
            }
            else
            {
                removedPreviousRow = false;
            }
        }
    }

    private static void HideLegacyBuffPanel(GameObject content)
    {
        var legacyPanel = Prefab.ChildPath(content, "Character_Content/Character_Buffs");
        if (!legacyPanel.IsNullOrDestroyed())
            legacyPanel.SetActive(false);
    }

    private static void Activate(HudPageDefinition page)
    {
        if (page == null)
            return;

        SetLegacyAreas(page.Areas);
        foreach (var panel in panels.Values)
        {
            panel.Root.SetActive(false);
            panel.Restore();
        }

        var activePanels = new List<PanelState>();
        foreach (var use in page.Panels)
        {
            if (!panels.TryGetValue(use.Panel, out var panel))
                continue;
            panel.Restore();
            panel.ApplyVisibleChildren(use.VisibleChildren);
            panel.ApplyVisibleRows(use.VisibleRows);
            panel.ApplyTitle(use.Title);
            panel.Root.SetActive(true);
            if (!activePanels.Contains(panel))
                activePanels.Add(panel);
        }

        LayoutPanels(activePanels);
        SetSelected(page.Id);
        activePage = page;
    }

    private static void SetLegacyAreas(HudArea[] areas)
    {
        var selected = new HashSet<HudArea>(areas);
        Hud_Manager.Content.Character.Set_Active(selected.Contains(HudArea.Character));
        Hud_Manager.Content.Items.Set_Active(selected.Contains(HudArea.Items));
        Hud_Manager.Content.Scenes.Set_Active(selected.Contains(HudArea.World));
        Hud_Manager.Content.Skills.Set_Active(selected.Contains(HudArea.Skills));
        Hud_Manager.Content.OdlForceDrop.Set_Active(selected.Contains(HudArea.ForceDrop));
        Hud_Manager.Content.Headhunter.Set_Active(false);
        Hud_Manager.Content.Set_Active();
    }

    private static void LayoutPanels(List<PanelState> activePanels)
    {
        float top = 0f;
        for (int i = 0; i < activePanels.Count; i++)
        {
            var panel = activePanels[i];
            var rect = panel.Root.GetComponent<RectTransform>();
            if (rect.IsNullOrDestroyed())
                continue;
            float height = panel.MeasureAndArrange();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -top);
            rect.sizeDelta = new Vector2(0f, height);
            top += height + HudTheme.CardGap;
        }
        var stageRect = stage.GetComponent<RectTransform>();
        stageRect.sizeDelta = new Vector2(
            0f,
            Mathf.Max(0f, top - (activePanels.Count > 0 ? HudTheme.CardGap : 0f))
        );
        Canvas.ForceUpdateCanvases();
        if (!mainScroll.IsNullOrDestroyed())
            mainScroll.verticalNormalizedPosition = 1f;
    }

    private static void SetSelected(string pageId)
    {
        foreach (var pair in pageButtons)
        {
            bool selected = pair.Key == pageId;
            pair.Value.Background.color = Color.white;
            pair.Value.Button.colors = HudTheme.ButtonColors(
                selected ? HudTheme.Selection : HudTheme.Surface,
                selected ? HudTheme.Selection : HudTheme.SurfaceHover
            );
            if (!pair.Value.Accent.IsNullOrDestroyed())
                pair.Value.Accent.SetActive(selected);
        }
    }

    private static void Close()
    {
        if (Hud_Manager.mod_menu_open)
            Hud_Manager.mod_menu_open = false;
        else
            Hud_Manager.Hud_Base.Resume_Click();
    }

    private static Font FindFont(GameObject hud)
    {
        foreach (var text in hud.GetComponentsInChildren<Text>(true))
            if (!text.font.IsNullOrDestroyed())
                return text.font;
        return null;
    }

    private static GameObject Node(GameObject parent, string name)
    {
        var node = new GameObject(name);
        node.layer = parent.layer;
        node.AddComponent<RectTransform>().SetParent(parent.transform, false);
        return node;
    }

    private static Text TextNode(GameObject parent, string name, string caption, int size)
    {
        var node = Node(parent, name);
        var text = node.AddComponent<Text>();
        text.font = font;
        text.fontSize = size;
        text.fontStyle = FontStyle.Normal;
        text.color = HudTheme.TextPrimary;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        text.raycastTarget = false;
        LocaleRegistry.Apply(text, caption);
        return text;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
