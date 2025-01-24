#if UNITY_EDITOR
using NaughtyAttributes;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuilder : MonoBehaviour
{
   [Button]
    static void BuildAllAssetBundles()
    {
        string outputDirectory = "Assets/AssetBundles";

        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);

        BuildPipeline.BuildAssetBundles(outputDirectory, BuildAssetBundleOptions.None, BuildTarget.WebGL);

        Debug.Log("Asset Bundles built successfully!");
    }
}
#endif