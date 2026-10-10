using System;
using Il2CppLE.Factions;
using LastEpoch_Hud.Scripts.ModUI.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace LastEpoch_Hud.Scripts.Mods.Character;

internal static class Character_MemoryAmber
{
    const int Amount = 10000;
    const string RowName = "CharacterCurrencyActions";
    const string Caption = "Add 10,000 Memory Amber";

    public static Button AddButton { get; private set; }

    public static void BuildButton(Button ancientBones)
    {
        if (ancientBones.IsNullOrDestroyed())
            return;
        var original = ancientBones.gameObject;
        var parent = original.transform.parent;
        if (parent.IsNullOrDestroyed() || parent.gameObject.name == RowName)
            return;
        var sourceRect = original.GetComponent<RectTransform>();
        if (sourceRect.IsNullOrDestroyed())
            return;
        var row = new GameObject(RowName);
        var rowRect = row.AddComponent<RectTransform>();
        rowRect.SetParent(parent, false);
        rowRect.SetSiblingIndex(original.transform.GetSiblingIndex());
        rowRect.anchorMin = sourceRect.anchorMin;
        rowRect.anchorMax = sourceRect.anchorMax;
        rowRect.pivot = sourceRect.pivot;
        rowRect.anchoredPosition = sourceRect.anchoredPosition;
        rowRect.sizeDelta = sourceRect.sizeDelta;
        var oldLayout = original.GetComponent<LayoutElement>();
        var layout = row.AddComponent<LayoutElement>();
        if (!oldLayout.IsNullOrDestroyed())
        {
            layout.ignoreLayout = oldLayout.ignoreLayout;
            layout.minWidth = oldLayout.minWidth;
            layout.preferredWidth = oldLayout.preferredWidth;
            layout.flexibleWidth = oldLayout.flexibleWidth;
            layout.flexibleHeight = oldLayout.flexibleHeight;
        }
        layout.minHeight = Mathf.Max(40, oldLayout.IsNullOrDestroyed() ? 0 : oldLayout.minHeight);
        layout.preferredHeight = Mathf.Max(
            40,
            oldLayout.IsNullOrDestroyed() ? 0 : oldLayout.preferredHeight
        );
        if (parent.gameObject.GetComponent<VerticalLayoutGroup>().IsNullOrDestroyed())
            rowRect.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Vertical,
                Mathf.Max(40, sourceRect.rect.height)
            );

        var clone = UnityEngine.Object.Instantiate(original, row.transform, false);
        clone.name = "Btn_Character_Cheats_AddMemoryAmber";
        var button = clone.GetComponent<Button>();
        AddButton = button;
        // Discard cloned runtime actions before binding the currency grant.
        button.onClick.RemoveAllListeners();
        Hud_Manager.Events.Set_Button_Event(button, new Action(Add10000));
        button.interactable = true;
        foreach (var label in clone.GetComponentsInChildren<Text>(true))
        {
            label.fontSize = Mathf.Min(label.fontSize, 14);
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            LocaleRegistry.Apply(label, Caption);
        }
        foreach (var label in clone.GetComponentsInChildren<Il2CppTMPro.TMP_Text>(true))
        {
            label.enableAutoSizing = true;
            label.fontSizeMin = 10;
            LocaleRegistry.Apply(label, Caption);
        }
        foreach (var label in original.GetComponentsInChildren<Text>(true))
        {
            label.fontSize = Mathf.Min(label.fontSize, 14);
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
        }
        original.transform.SetParent(row.transform, false);
        Place(original, 0, .49f);
        Place(clone, .51f, 1);
        clone.SetActive(true);
    }

    static void Place(GameObject go, float left, float right)
    {
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(left, 0);
        rect.anchorMax = new Vector2(right, 1);
        rect.offsetMin = rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        var layout = go.GetComponent<LayoutElement>();
        if (!layout.IsNullOrDestroyed())
            layout.ignoreLayout = true;
    }

    public static void Add10000()
    {
        if (!Scenes.IsGameScene() || Refs_Manager.player_actor.IsNullOrDestroyed())
            return;
        try
        {
            var tracker = Refs_Manager.player_actor.gameObject.GetComponent<FactionTracker>();
            if (
                tracker.IsNullOrDestroyed()
                || tracker.factions == null
                || !tracker.factions.TryGetValue(FactionID.TheWeaver, out Faction faction)
                || faction.IsNullOrDestroyed()
            )
            {
                Main.logger_instance?.Warning("Memory Amber: Weaver faction unavailable.");
                return;
            }
            if (!faction.IsMember)
            {
                Main.logger_instance?.Warning(
                    "Memory Amber: join the Woven faction before adding amber."
                );
                return;
            }
            int amount = (int)
                Math.Min(Amount, Math.Max(0L, (long)Faction.MaxFavor - faction.Favor));
            if (amount == 0)
            {
                Main.logger_instance?.Warning(
                    "Memory Amber: currency balance is already at its limit."
                );
                return;
            }
            // Memory Amber is Weaver Favor. Update native currency/events without
            // multiplying this grant or awarding faction reputation.
            using (Mods.UI.SessionGainCounters.SuppressManualGrants())
                faction.GainFavor(amount, true, true);
            faction.SaveAndSync(false);
            Main.logger_instance?.Msg("Memory Amber: added " + amount + ".");
        }
        catch (Exception ex)
        {
            Main.logger_instance?.Error("Add Memory Amber failed: " + ex.Message);
        }
    }
}
