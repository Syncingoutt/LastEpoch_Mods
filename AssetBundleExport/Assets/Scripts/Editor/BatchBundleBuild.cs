using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Headless bundle build: Unity -batchmode -quit -projectPath AssetBundleExport
/// -executeMethod BatchBundleBuild.Build -bundleOut &lt;dir&gt; [-oldBundle &lt;path&gt;]
/// Sets up the Mageblood icons like Headhunter's, builds every bundle into bundleOut
/// and writes old/new asset name lists next to it for comparison.
/// </summary>
public static class BatchBundleBuild
{
    private const string BundleName = "lastepochmods";
    private const string TemplateIcon = "Assets/Headhunter/Texture2D/Icon.png";
    private const string MagebloodFolder = "Assets/Mageblood/Texture2D";
    private const string BeltIcon = MagebloodFolder + "/Icon.png";
    private const int BeltIconMaxSize = 256;

    public static void Build()
    {
        string outDir = Arg("-bundleOut");
        if (string.IsNullOrEmpty(outDir))
        {
            Fail("missing -bundleOut");
        }

        SetUpMagebloodIcons();
        Directory.CreateDirectory(outDir);
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(
            outDir,
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneWindows
        );
        if (manifest == null)
        {
            Fail("BuildAssetBundles returned null");
        }

        WriteNames(Path.Combine(outDir, BundleName), Path.Combine(outDir, "new-assets.txt"));
        string oldBundle = Arg("-oldBundle");
        if (!string.IsNullOrEmpty(oldBundle))
        {
            WriteNames(oldBundle, Path.Combine(outDir, "old-assets.txt"));
        }

        Debug.Log("BATCH_BUILD_OK");
    }

    private static void SetUpMagebloodIcons()
    {
        var template = (TextureImporter)AssetImporter.GetAtPath(TemplateIcon);
        var settings = new TextureImporterSettings();
        template.ReadTextureSettings(settings);
        TextureImporterPlatformSettings standalone = template.GetPlatformTextureSettings("Standalone");
        TextureImporterPlatformSettings fallback = template.GetDefaultPlatformTextureSettings();

        string[] icons = AssetDatabase
            .FindAssets("t:Texture2D", new[] { MagebloodFolder })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => p.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        foreach (string path in icons)
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.SetTextureSettings(settings);
            importer.SetPlatformTextureSettings(standalone);
            importer.SetPlatformTextureSettings(fallback);
            importer.maxTextureSize = path == BeltIcon ? BeltIconMaxSize : fallback.maxTextureSize;
            importer.assetBundleName = BundleName;
            importer.SaveAndReimport();
            Debug.Log("BATCH_ICON " + path);
        }
    }

    private static void WriteNames(string bundlePath, string listPath)
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
        if (bundle == null)
        {
            Fail("cannot load " + bundlePath);
        }

        string[] names = bundle.GetAllAssetNames().OrderBy(n => n, StringComparer.Ordinal).ToArray();
        bundle.Unload(true);
        File.WriteAllLines(listPath, names);
    }

    private static string Arg(string name)
    {
        string[] args = Environment.GetCommandLineArgs();
        int index = Array.IndexOf(args, name);
        return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
    }

    private static void Fail(string message)
    {
        Debug.LogError("BATCH_BUILD_FAIL " + message);
        EditorApplication.Exit(1);
    }
}
