using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ModelLoader
{
    public IEnumerator LoadModelAsync(ButtonModelAssociation modelAssociation, string baseUrl)
    {
        if (modelAssociation.SpawnedModel) yield break;

        if (string.IsNullOrEmpty(baseUrl))
        {
            modelAssociation.LoadedPrefab = modelAssociation.Prefab;
            yield break;
        }

        var url = $"{baseUrl}{modelAssociation.Prefab.name.ToLower()}";

        using var request = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var bundle = DownloadHandlerAssetBundle.GetContent(request);

            if (bundle == null)
            {
                Debug.LogError($"Failed to download AssetBundle from {url}: Bundle is null");
                yield break;
            }

            var prefab = bundle.LoadAsset<GameObject>(modelAssociation.Prefab.name);
            bundle.Unload(false);

            if (prefab != null) modelAssociation.LoadedPrefab = prefab;
            else Debug.LogError($"Prefab {modelAssociation.Prefab.name} not found in AssetBundle {url}");
        }
        else Debug.LogError($"Failed to load AssetBundle from {url}: {request.error}");
    }
}