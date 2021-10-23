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
    private RawImage rawImage;
    [SerializeField]
    private Button button;

    private JewelProperties jewelProperties;
    private ItemsList itemsManager;

    public void InitializeItem(string itemName, string itemLabel, RenderTexture texture, JewelProperties properties, ItemsList manager)
    {
        SetItemTexts(itemName, itemLabel);
        SetTexture(texture);
        itemsManager = manager;
        jewelProperties = properties;
        button.onClick.AddListener(SetJewelProperties);
    }

    private void SetItemTexts(string itemName, string itemLabel)
    {
        itemNameText.text = itemName;
        itemLabelText.text = itemLabel;
    }

    private void SetTexture(RenderTexture texture) {
        rawImage.texture = texture;
    }

    private void SetJewelProperties()
    {
        itemsManager.StartAR(jewelProperties);
    }

}
