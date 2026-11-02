using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

// Copies the built content directories into the Player build.
// Reference: https://docs.unity.com/en-us/engine/6000.6/manual/assets-and-media/assets-managing-runtime/content-directories/load.
class ContentDirectoryDeployment : BuildPlayerProcessor
{
    // A Web build preloads the files listed by every manifest in this
    // folder into its virtual file system.
    const string k_WebPreloadFolder =
        "Library/PlayerDataCache/WebGLPreloadedStreamingAssets";

    const string k_WebPreloadManifest =
        k_WebPreloadFolder + "/content-directories.manifest";

    public override void PrepareForBuild(
        BuildPlayerContext buildPlayerContext)
    {
        string buildsPath = ContentDirectoryManager.ContentDirectoryBuildPath;

        // Skip if no content directories exist. This allows other builds
        // in the same project to proceed without content directories.
        if (!Directory.Exists(buildsPath))
            return;

        string[] directories = Directory.GetDirectories(buildsPath);
        if (directories.Length == 0)
            return;

        // Unity copies the contents of buildsPath into the root of the
        // StreamingAssets folder, so each content directory keeps its own
        // folder name.
        buildPlayerContext.AddAdditionalPathToStreamingAssets(buildsPath);

        // Preserve the types used by the content so that managed code
        // stripping does not remove them.
        foreach (string directory in directories)
        {
            if (BuildHistory.TryGetBuildSummaryForOutputPath(
                    directory, out BuildReportSummary summary))
            {
                if (BuildHistory.TryGetBuildReportDirectory(
                        summary.BuildSessionGUID, out string reportDirectory))
                {
                    buildPlayerContext.AddPreviousBuildReportDirectory(
                        reportDirectory);
                }
            }
        }

        if (buildPlayerContext.BuildPlayerOptions.target ==
            BuildTarget.WebGL)
        {
            WriteWebPreloadManifest(buildsPath, directories);
        }
        else if (File.Exists(k_WebPreloadManifest))
        {
            // Remove the manifest from an earlier Web build, so that it
            // can't preload files in a later one.
            File.Delete(k_WebPreloadManifest);
        }
    }

    // The Web platform serves the StreamingAssets folder over HTTP, which
    // a browser can't read synchronously. RegisterContentDirectory needs
    // synchronous access, so list each file for the build to preload.
    static void WriteWebPreloadManifest(string buildsPath,
        string[] directories)
    {
        Directory.CreateDirectory(k_WebPreloadFolder);

        using (StreamWriter writer =
               new StreamWriter(k_WebPreloadManifest, false))
        {
            foreach (string directory in directories)
            {
                string[] files = Directory.GetFiles(
                    directory, "*", SearchOption.AllDirectories);

                string directoryName = Path.GetFileName(directory);
                foreach (string file in files)
                {
                    string relativePath = directoryName + "/" +
                                          Path.GetRelativePath(directory, file);
                    writer.WriteLine(relativePath.Replace('\\', '/'));
                }
            }
        }
    }
}