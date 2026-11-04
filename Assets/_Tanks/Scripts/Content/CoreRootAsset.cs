using Unity.Loading;
using UnityEngine;

// Reference: https://docs.unity.com/en-us/engine/6000.6/manual/assets-and-media/assets-managing-runtime/content-directories/create.
[CreateAssetMenu(fileName = "CoreRootAsset", menuName = "Content/Core Root Asset")]
public class CoreRootAsset : ScriptableObject
{
    public static string DirectoryName = "Core";
    
    // Scene assets
    public LoadableSceneId[] scenes;
    
    // Tank assets
    public Loadable<GameObject> tank1Prefab;
    public Loadable<GameObject> tank2Prefab;
    public Loadable<GameObject> tank3Prefab;
    public Loadable<GameObject> tank4Prefab;
}