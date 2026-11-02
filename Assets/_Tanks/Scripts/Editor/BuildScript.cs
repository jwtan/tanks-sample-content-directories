using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildScript
{
    [MenuItem("Build/Build All")]
    public static void BuildAll()
    {
        BuildContentDirectories();
        BuildPlayer();
    }

    [MenuItem("Build/Build Player")]
    public static void BuildPlayer()
    {
        // Todo:
    }

    // Reference: https://docs.unity.com/en-us/engine/6000.6/manual/assets-and-media/assets-managing-runtime/content-directories/create.
    [MenuItem("Build/Build Content Directories")]
    public static void BuildContentDirectories()
    {
        var outputPath = ContentDirectoryManager.ContentDirectoryBuildPath + "/" + CoreRootAsset.DirectoryName;

        var rootAssetPaths = new[]
        {
            "Assets/_Tanks/Content/CoreRootAsset.asset"
        };

        BuildContentDirectory(outputPath, rootAssetPaths);
    }

    private static void BuildContentDirectory(string buildPath, string[] rootAssetPaths)
    {
        var parameters = new BuildContentDirectoryParameters
        {
            // Folder for the built content directory (created if it does not exist).
            // Typically should be outside the Assets folder.
            outputPath = buildPath,

            // Project paths to ScriptableObject root assets for this pack.
            // Unity includes each root and every asset they reference (direct, Loadable, scenes).
            rootAssetPaths = rootAssetPaths,

            // Optional build flags (for example BuildContentOptions.CleanBuildCache).
            // See BuildContentOptions in the Scripting Reference.
            options = BuildContentOptions.None,
        };

        // Uses the active build target from Build Profiles.
        BuildReport report = BuildPipeline.BuildContentDirectory(parameters);

        if (report.summary.result == BuildResult.Succeeded)
            Debug.Log($"Content directory built to: {parameters.outputPath}");
    }
}