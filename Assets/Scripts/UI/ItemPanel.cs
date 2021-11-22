using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
    [SerializeField]
    private Text itemNameText;
    [SerializeField]
    private Text itemLabelText;
    [SerializeField]
    private GameObject modelContainer;
    [SerializeField]
    private Button button;

    private JewelProperties jewelProperties;
    private SceneInitializer sceneInitializer;

    public void InitializeItem(string itemName, string itemLabel, GameObject model, JewelProperties properties, SceneInitializer initializer)
    {
        SetItemTexts(itemName, itemLabel);
        SetUIModel(model);
        sceneInitializer = initializer;
        jewelProperties = properties;
        button.onClick.AddListener(StartARScene);
    }

    private void SetItemTexts(string itemName, string itemLabel)
    {
        itemNameText.text = itemName;
        itemLabelText.text = itemLabel;
    }

    private void SetUIModel(GameObject model) {
        Instantiate(model, modelContainer.transform);
    }

    private void StartARScene()
    {
        sceneInitializer.StartTryOn(jewelProperties);
    }

}
