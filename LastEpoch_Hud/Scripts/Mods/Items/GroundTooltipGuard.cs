using System;
using HarmonyLib;
using Il2Cpp;
using Il2CppLE.UI.Tooltips;
using LastEpoch_Hud.Scripts.Core.Items;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LastEpoch_Hud.Scripts.Mods.Items;

// Preserve hovered ground-item content while allowing native cancellation and replacement.
internal static class GroundTooltipGuard
{
    [ThreadStatic]
    static int openingDepth;

    [ThreadStatic]
    static CloseAllScope closeAllScope;

    sealed class CloseAllScope
    {
        internal CloseAllScope Parent;
        internal UITooltipItem ProtectedItem;
    }

    static bool PointerOnOpener(TooltipItemManager manager)
    {
        var opener = manager.activeParameters.Opener;
        var highlighted = TooltipItem.highlightedTooltipItem;
        if (
            opener.IsNullOrDestroyed()
            || !opener.activeInHierarchy
            || highlighted.IsNullOrDestroyed()
            || !highlighted.isActiveAndEnabled
            || highlighted.slotType != TooltipItemManager.SlotType.GROUND
            || highlighted.button.IsNullOrDestroyed()
            || !highlighted.button.interactable
        )
            return false;
        var events = EventSystem.current;
        if (events.IsNullOrDestroyed())
            return false;
        var pointer = new PointerEventData(events) { position = Input.mousePosition };
        var hits = new Il2CppSystem.Collections.Generic.List<RaycastResult>();
        events.RaycastAll(pointer, hits);
        if (hits.Count == 0 || hits[0].gameObject.IsNullOrDestroyed())
            return false;
        var hit = hits[0].gameObject;
        return (hit == opener || hit.transform.IsChildOf(opener.transform))
            && (hit == highlighted.gameObject || hit.transform.IsChildOf(highlighted.transform));
    }

    [HarmonyPatch(
        typeof(TooltipItemManager),
        nameof(TooltipItemManager.OpenTooltip),
        new Type[]
        {
            typeof(ItemDataUnpacked),
            typeof(TooltipItemManager.SlotType),
            typeof(Vector2),
            typeof(Vector3),
            typeof(GameObject),
            typeof(Vector2),
        }
    )]
    static class Open
    {
        [HarmonyPrefix]
        static void Prefix(out bool __state)
        {
            openingDepth++;
            __state = true;
        }

        [HarmonyFinalizer]
        static Exception Finalizer(bool __state, Exception __exception)
        {
            if (__state)
                openingDepth--;
            return __exception;
        }
    }

    // Native CloseAllTooltips clears the visible view independently after closing its manager.
    [HarmonyPatch(typeof(TooltipSystem), nameof(TooltipSystem.CloseAllTooltips))]
    static class CloseAll
    {
        [HarmonyPrefix]
        static void Prefix(out CloseAllScope __state)
        {
            __state = new CloseAllScope { Parent = closeAllScope };
            closeAllScope = __state;
        }

        [HarmonyFinalizer]
        static Exception Finalizer(CloseAllScope __state, Exception __exception)
        {
            closeAllScope = __state?.Parent;
            return __exception;
        }
    }

    [HarmonyPatch(typeof(UITooltipItem), nameof(UITooltipItem.CloseTooltip))]
    static class CloseVisibleItem
    {
        [HarmonyPrefix]
        static bool Prefix(UITooltipItem __instance)
        {
            try
            {
                var protectedItem = closeAllScope?.ProtectedItem;
                return protectedItem.IsNullOrDestroyed() || protectedItem != __instance;
            }
            catch
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(TooltipItemManager), nameof(TooltipItemManager.CloseTooltip))]
    static class Close
    {
        [HarmonyPrefix]
        static bool Prefix(TooltipItemManager __instance)
        {
            try
            {
                bool ground =
                    __instance.active
                    && !__instance.activeParameters.IsNullOrDestroyed()
                    && __instance.activeParameters.SlotType == TooltipItemManager.SlotType.GROUND;
                if (!ground)
                    return true;
                bool keep = GroundTooltipRules.KeepHoveredGroundTooltip(
                    Scenes.IsGameScene() && Application.isFocused && __instance.isActiveAndEnabled,
                    ground,
                    !__instance.activeParameters.Item.IsNullOrDestroyed(),
                    PointerOnOpener(__instance),
                    openingDepth > 0,
                    Input.GetMouseButton(0) || Input.GetMouseButton(1),
                    Input.GetKey(KeyCode.Escape),
                    UIMouseListener.WorldActionsDisabled()
                );
                if (closeAllScope != null)
                    closeAllScope.ProtectedItem = keep ? __instance.itemTooltip : null;
                return !keep;
            }
            catch
            {
                return true;
            }
        }
    }
}
