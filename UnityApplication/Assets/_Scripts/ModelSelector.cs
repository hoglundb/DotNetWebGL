using System;
using System.Collections;
using UnityEngine;

public class ModelSelector : MonoBehaviour
{
    [SerializeField] private OrbitalCameraController orbitalCameraController;
    [SerializeField] private ButtonModelAssociation[] buttonModels;
    [SerializeField] private float loadDelay = 1f;

    private string baseUrl;
    public ModelLoader modelLoader = new();

    private void OnValidate()
        => buttonModels = GetComponentsInChildren<ButtonModelAssociation>(true);

    private void Awake()
    {
        baseUrl = GetBaseUrl();
        StartCoroutine(LoadAllModelsSequentially());
    }

    private IEnumerator LoadAllModelsSequentially()
    {
        var isFirst = true;
        foreach (var item in buttonModels)
        {
            yield return modelLoader.LoadModelAsync(item, baseUrl);

            if (item.LoadedPrefab != null)
            {
                item.SpawnModel(OnModelSelected, isFirst);
                if(isFirst) orbitalCameraController.SetTarget(item.SpawnedModel);
                isFirst = false;
            }
            else
            {
                Debug.LogError($"Failed to load prefab for {item.Prefab.name}. Check logs for ModelLoader errors.");
                yield break;
            }

            yield return new WaitForSeconds(loadDelay);
        }
    }

    private void OnModelSelected(ButtonModelAssociation selectedModel)
    {
        foreach (var item in buttonModels)
        {
            item.enabled = false;
        }

        if (selectedModel.SpawnedModel)
        {
            selectedModel.enabled = true;
            orbitalCameraController.SetTarget(selectedModel.SpawnedModel);
        }
    }

    private string GetBaseUrl()
    {
        // If in Unity Editor, models are loaded directly from the asset database
        if (Application.isEditor) return "";

        // If running on the web server, we load the asset bundles from the server domain.
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            var baseUri = new Uri(Application.absoluteURL);
            return baseUri.GetLeftPart(UriPartial.Authority) + "/assets/models/";
        }

        // If running locally using IIS Express.
        return "https://localhost:44367/assets/models/";
    }
}