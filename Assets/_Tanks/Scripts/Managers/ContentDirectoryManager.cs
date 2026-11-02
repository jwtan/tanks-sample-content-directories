using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;

// Registers content directories by name and unregisters them on shutdown.
// Reference: https://docs.unity.com/en-us/engine/6000.6/manual/assets-and-media/assets-managing-runtime/content-directories/load.
public class ContentDirectoryManager
{
    // Build output location for the content directories, relative to the
    // project folder. It's outside the Assets folder, so Unity doesn't
    // import the built content.
    public const string ContentDirectoryBuildPath = "Builds/ContentDirectories";

    private readonly List<ContentDirectoryHandle> m_Handles = new();

    // During Play mode the content directories are read from the build
    // output location. In a Player they're read from the StreamingAssets
    // folder, where the Player build copied them.
    static string ContentDirectoryLoadPath
    {
        get
        {
#if UNITY_EDITOR
            return ContentDirectoryBuildPath;
#elif UNITY_WEBGL
            // Preloaded StreamingAssets files are in the Emscripten virtual
            // file system at this fixed path.
            return "/vfs_streamingassets";
#else
            return Application.streamingAssetsPath;
#endif
        }
    }

    public void Register(string directoryName)
    {
        var path = ContentDirectoryLoadPath + "/" + directoryName;

        m_Handles.Add(ContentLoadManager.RegisterContentDirectory(path));
    }

    public void UnregisterAll()
    {
        for (int i = m_Handles.Count - 1; i >= 0; i--)
        {
            ContentLoadManager.UnregisterContentDirectory(m_Handles[i]);
        }

        m_Handles.Clear();
    }
}