using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsRowContainer : MonoBehaviour
{
    [SerializeField]
    private Text rowTitleText;
    [SerializeField]
    private ScrollRectNested scrollRectNested;
    [SerializeField]
    private GameObject horizontalLayoutContainer;
    [SerializeField]
    private GameObject itemPanelPrefab;

    public void InitializeRow(string rowTitle, ScrollRect parentScrollRect)
    {
        SetRowTitle(rowTitle);
        LinkScrollRectToParent(parentScrollRect);
    }

    private void SetRowTitle(string title)
    {
        rowTitleText.text = title;
    }

    private void LinkScrollRectToParent(ScrollRect parent)
    {
        scrollRectNested.parentScrollRect = parent;
    }

    public void CreateItemPanel(string itemName, string itemLabel, GameObject UIJewelPrefab, JewelProperties properties, SceneInitializer initializer)
    {
        GameObject itemPanel = Instantiate(itemPanelPrefab, horizontalLayoutContainer.transform);
        itemPanel.GetComponent<ItemPanel>().InitializeItem(itemName, itemLabel, UIJewelPrefab, properties, initializer);
    }
}
