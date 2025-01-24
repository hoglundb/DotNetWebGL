using NaughtyAttributes;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonModelAssociation : MonoBehaviour
{
    [NonSerialized] public GameObject LoadedPrefab;
    public GameObject SpawnedModel => spawnedModel;

    [field: SerializeField] public GameObject Prefab { get; private set; }

    [SerializeField, ReadOnly] private GameObject spawnedModel;
    [SerializeField, ReadOnly] private Button selectModelButton;   
    [SerializeField, ReadOnly] private TextMeshProUGUI buttonText;

    private Action<ButtonModelAssociation> buttonClickedCallback;

    private void OnValidate()
    {
        selectModelButton = GetComponent<Button>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        enabled = false;

        if (buttonText) buttonText.text = "Loading...";

        if(selectModelButton)
        {
            selectModelButton.interactable = false;

            var colorBock = selectModelButton.colors;
            colorBock.selectedColor = Color.cyan;
            selectModelButton.colors = colorBock;
        }
    }

    private void OnEnable()
    {
        if (spawnedModel) spawnedModel.SetActive(true);
    }

    private void OnDisable()
    {
        if (spawnedModel) spawnedModel.SetActive(false);
    }

    private void Update()
    {
        // Keep the button in selected mode so it stays highlighted
        selectModelButton.Select();
    } 

    public void SpawnModel(Action<ButtonModelAssociation> onModelSelectedCallback, bool startOutSelected)
    {
        if (spawnedModel) return;

      
        spawnedModel = Instantiate(Prefab);
        spawnedModel.name = Prefab.name;

        enabled = startOutSelected;
        spawnedModel.SetActive(startOutSelected);

        selectModelButton.interactable = true;
        buttonText.text = spawnedModel.name;

        buttonClickedCallback = onModelSelectedCallback;
        selectModelButton.onClick.AddListener(() =>
        {
            if (!spawnedModel) return;
            enabled = true;
            buttonClickedCallback?.Invoke(this);            
        });

        if (startOutSelected) selectModelButton.Select();
    }  
}