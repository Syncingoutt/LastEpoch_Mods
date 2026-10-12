using System.Collections.Generic;
using System.IO;
using System.Linq;
using LastEpoch_Hud.Scripts.Core.ModUI.LegacySettings;
using LastEpoch_Hud.Scripts.Core.ModUI.Settings;
using LastEpoch_Hud.Scripts.ModUI.Keybind;
using MelonLoader;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace LastEpoch_Hud.Scripts.ModUI.Settings;

// Persists settings to SaveModUI.json. Groups handle their own serialization.
[RegisterTypeInIl2Cpp]
public class SaveManager : MonoBehaviour
{
    public SaveManager(System.IntPtr ptr)
        : base(ptr) { }

    public static SaveManager instance;
    public volatile bool initialized;

    private const string LegacyFileName = "Save.json";
    private const string filename = "SaveModUI.json";
    private const string CorruptFileName = "SaveModUI.corrupt.json";
    private const float SaveInterval = 1f;
    private float saveTimer;
    private bool profilingAttachAttempted;
    private bool _writesBlocked;
    private string _legacyText;
    private bool _legacyReadFailed;

    /// <summary>Settings loaded this process.</summary>
    public static bool IsLoaded => !instance.IsNullOrDestroyed() && instance.initialized;

    void Awake()
    {
        instance = this;
        LegacyTextState legacyState = LegacyTextReader.TryRead(
            ModPaths.FilePath(LegacyFileName),
            out _legacyText,
            out string error
        );
        _legacyReadFailed = legacyState == LegacyTextState.Failed;
        if (_legacyReadFailed)
        {
            ErrorLog.Report(error, "ModUI SaveManager.ReadLegacy");
        }
    }

    void Start()
    {
        // Load before UI binding/capture; settings and Unity input validation
        // must not race a background loader.
        try
        {
            Load();
        }
        catch (System.Exception ex)
        {
            ErrorLog.Report(ex, "ModUI SaveManager.Load");
        }
    }

    void Update()
    {
        LocaleRegistry.TickIfLocaleChanged();
        if (KeybindCapture.Active)
        {
            KeybindCapture.Tick();
        }

        if (initialized && !profilingAttachAttempted)
        {
            profilingAttachAttempted = true;
            try
            {
                Mods.Diagnostics.DiagnosticsDumper.AttachIfEnabled();
            }
            catch (System.Exception ex)
            {
                Main.logger_instance?.Warning("[Profiling] attach failed: " + ex.Message);
            }
        }

        if (!initialized || _writesBlocked)
        {
            return;
        }
        if (!ModSettings.Dirty)
        {
            return;
        }
        // Time.unscaledDeltaTime: advances even when the game is paused (HUD open = timeScale=0)
        saveTimer += Time.unscaledDeltaTime;
        if (saveTimer < SaveInterval)
        {
            return;
        }
        saveTimer = 0f;
        ModSettings.Trace("SaveManager.Update flushing (debounce hit)");
        if (TrySave())
        {
            ModSettings.ClearDirty();
        }
    }

    void OnApplicationQuit()
    {
        if (!initialized || !ModSettings.Dirty)
        {
            return;
        }
        if (TrySave())
        {
            ModSettings.ClearDirty();
        }
    }

    private void Load()
    {
        string fullPath = ModPaths.FilePath(filename);
        SettingsFileState state = SettingsFile.TryRead(
            fullPath,
            out JObject fileRoot,
            out string readError
        );
        if (readError != null)
        {
            ErrorLog.Report(readError, "ModUI SaveManager.Read");
        }
        // Taken before the import: imported groups are missing on disk and must be saved.
        bool hasMissingEntries = state == SettingsFileState.Loaded && AnyMissingEntries(fileRoot);
        JObject root = fileRoot ?? new JObject();
        LegacyImportOutcome outcome = ImportLegacy(root);
        WarnUnknownMapTargets();
        bool allGroupsLoaded = LoadGroups(root);
        SettingsLoadPlan plan = SettingsLoadPlanner.Plan(
            state,
            allGroupsLoaded,
            hasMissingEntries,
            outcome == LegacyImportOutcome.Imported
        );
        // Without the legacy values, saving would replace them with defaults for good.
        _writesBlocked = outcome == LegacyImportOutcome.ReadFailed;

        Main.logger_instance?.Msg(
            "ModUI SaveManager: Initialized with "
                + ModSettings.AllGroups.Count
                + " group(s), file "
                + state
        );

        // Fresh or corrupt files should never silently enable XP capping.
        // A schema top-up of a healthy file keeps the player's choice.
        if (plan.ResetCapLevelToZone)
        {
            ModSettings.Difficulty.CapLevelToZone.Value = false;
        }

        bool conflictFixed = FixBindingConflict();
        if (plan.BackupBeforeWrite)
        {
            BackupCorruptFile(fullPath);
        }
        initialized = true;

        if (!plan.WriteNow && !conflictFixed)
        {
            return;
        }
        if (TrySave())
        {
            Main.logger_instance?.Msg("ModUI SaveManager: Wrote fresh schema to " + filename);
        }
    }

    private LegacyImportOutcome ImportLegacy(JObject root)
    {
        List<LegacyImportProblem> problems = new();
        LegacyImportOutcome outcome = LegacySettingsMerge.ImportMissing(
            _legacyText,
            _legacyReadFailed,
            root,
            LegacySettingsMap.All,
            new UnityKeyNames(),
            problems
        );
        _legacyText = null;
        ReportImport(outcome, problems);
        return outcome;
    }

    private static void ReportImport(
        LegacyImportOutcome outcome,
        List<LegacyImportProblem> problems
    )
    {
        if (outcome == LegacyImportOutcome.ReadFailed)
        {
            ErrorLog.Report(
                problems[0].Message + " Settings will not be saved this session.",
                "ModUI SaveManager.ImportLegacy"
            );
            return;
        }
        if (outcome == LegacyImportOutcome.FileFailed)
        {
            ErrorLog.Report(problems[0].Message, "ModUI SaveManager.ImportLegacy");
            return;
        }
        if (outcome == LegacyImportOutcome.Imported)
        {
            Main.logger_instance?.Msg(
                "ModUI SaveManager: Imported legacy settings from " + LegacyFileName
            );
        }
        if (problems.Count == 0)
        {
            return;
        }
        Main.logger_instance?.Warning(
            "ModUI SaveManager: Skipped unreadable legacy fields: "
                + string.Join(", ", problems.Select(problem => problem.Path))
        );
    }

    private static void WarnUnknownMapTargets()
    {
        IReadOnlyList<string> unknown = LegacyMapCheck.UnknownTargets(
            LegacySettingsMap.All,
            IsKnownSetting
        );
        if (unknown.Count == 0)
        {
            return;
        }
        Main.logger_instance?.Warning(
            "ModUI SaveManager: Legacy map targets no setting has: " + string.Join(", ", unknown)
        );
    }

    private static bool IsKnownSetting(string group, string key)
    {
        foreach (SettingsGroup candidate in ModSettings.AllGroups)
        {
            if (candidate.Name == group)
            {
                return candidate.HasEntry(key);
            }
        }
        return false;
    }

    // False if any group threw; the others still load.
    private static bool LoadGroups(JObject root)
    {
        bool allLoaded = true;
        foreach (SettingsGroup group in ModSettings.AllGroups)
        {
            try
            {
                group.Load(root);
            }
            catch (System.Exception ex)
            {
                ErrorLog.Report(ex, "ModUI SaveManager.LoadGroup " + group.Name);
                allLoaded = false;
            }
        }
        return allLoaded;
    }

    // A group or per-group entry missing on disk means newly added keys need writing.
    private static bool AnyMissingEntries(JObject root)
    {
        foreach (SettingsGroup group in ModSettings.AllGroups)
        {
            if (group.HasMissingEntries(root))
            {
                return true;
            }
        }
        return false;
    }

    private bool FixBindingConflict()
    {
        if (
            !KeybindMatcher.Conflicts(
                ModSettings.SafeTeleport.Key.Value,
                ModSettings.SkillsAutoCast.ModifierKey.Value
            )
        )
        {
            return false;
        }

        ModSettings.SafeTeleport.Key.Value = "";
        Main.logger_instance?.Warning(
            "Saved bindings conflict: Safe Teleport unbound; AutoCast binding preserved."
        );
        return true;
    }

    private void BackupCorruptFile(string fullPath)
    {
        if (!File.Exists(fullPath))
        {
            return;
        }
        try
        {
            File.Copy(fullPath, ModPaths.FilePath(CorruptFileName), overwrite: true);
        }
        catch (System.Exception ex)
        {
            ErrorLog.Report(ex, "ModUI SaveManager.Backup: writes blocked this session");
            _writesBlocked = true;
        }
    }

    internal static void FlushKeybind()
    {
        if (instance == null || !instance.initialized)
        {
            return;
        }
        if (instance.TrySave())
        {
            ModSettings.ClearDirty();
        }
    }

    private bool TrySave()
    {
        if (_writesBlocked)
        {
            return false;
        }
        try
        {
            var root = new JObject();
            foreach (SettingsGroup group in ModSettings.AllGroups)
            {
                group.Save(root);
            }

            Directory.CreateDirectory(ModPaths.Folder);
            if (
                !SettingsFile.WriteAtomic(
                    ModPaths.FilePath(filename),
                    root.ToString(Formatting.Indented),
                    out string writeError
                )
            )
            {
                ErrorLog.Report(writeError, "ModUI SaveManager.Save");
                return false;
            }
            ModSettings.Trace("SaveManager.Save wrote " + filename);
            return true;
        }
        catch (System.Exception ex)
        {
            ErrorLog.Report(ex, "ModUI SaveManager.Save");
            return false;
        }
    }

    public static void BindHud(GameObject hud_object)
    {
        BindRoot(BindingRoots.HudRootName, hud_object, RootKind.HudWithTabs);
    }

    public static void BindRoot(string name, GameObject root, RootKind kind = RootKind.Generic)
    {
        if (instance == null || root.IsNullOrDestroyed())
            return;
        if (string.IsNullOrEmpty(name))
            return;

        if (BindingRoots.IsSameRoot(name, root))
        {
            Main.logger_instance?.Msg(
                "ModUI SaveManager: BindRoot " + name + " skipped (same root already bound)"
            );
            return;
        }

        LocaleRegistry.SweepDead();
        BindingRoots.Register(name, root, kind);

        if (kind == RootKind.HudWithTabs)
            BindHudInternal(root);
        else
            BindGenericRoot(name, root);

        Main.logger_instance?.Msg(
            "ModUI SaveManager: BindRoot "
                + name
                + " (kind="
                + kind
                + ", groups="
                + ModSettings.AllGroups.Count
                + ")"
        );
    }

    private static void BindHudInternal(GameObject hud_object)
    {
        var contentRoot = Functions.GetChild(hud_object, "Content");
        var menu = Functions.GetChild(hud_object, "Menu");
        var menuContent = menu.IsNullOrDestroyed() ? null : Functions.GetChild(menu, "Content");
        if (contentRoot.IsNullOrDestroyed() || menuContent.IsNullOrDestroyed())
        {
            Main.logger_instance?.Warning(
                "ModUI SaveManager: BindHudInternal couldn't resolve Content or Menu/Content"
            );
            return;
        }

        TabManager.Init(contentRoot, menuContent);
        BindNonTabGroups(contentRoot);
    }

    private static void BindNonTabGroups(GameObject contentRoot)
    {
        foreach (var group in ModSettings.AllGroups)
        {
            if (group.RootName != BindingRoots.HudRootName)
                continue;
            if (group.HasTab || group.ContentObjectName == null)
                continue;
            try
            {
                var contentObj = Prefab.Child(contentRoot, group.ContentObjectName);
                if (contentObj != null)
                    group.ResolveAndBind(contentObj);
            }
            catch (System.Exception ex)
            {
                Main.logger_instance?.Error(
                    "ModUI SaveManager: Failed to bind group '" + group.Name + "': " + ex.Message
                );
            }
        }
    }

    private static void BindGenericRoot(string rootName, GameObject root)
    {
        foreach (var group in ModSettings.AllGroups)
        {
            if (group.RootName != rootName)
                continue;
            try
            {
                var contentObj =
                    group.ContentObjectName != null
                        ? Prefab.Child(root, group.ContentObjectName)
                        : root;
                if (contentObj != null)
                    group.ResolveAndBind(contentObj);
                else
                    Main.logger_instance?.Warning(
                        "ModUI SaveManager: BindGenericRoot '"
                            + rootName
                            + "' missing content '"
                            + group.ContentObjectName
                            + "' for group '"
                            + group.Name
                            + "'"
                    );
            }
            catch (System.Exception ex)
            {
                Main.logger_instance?.Error(
                    "ModUI SaveManager: Failed to bind group '"
                        + group.Name
                        + "' to root '"
                        + rootName
                        + "': "
                        + ex.Message
                );
            }
        }
    }
}
