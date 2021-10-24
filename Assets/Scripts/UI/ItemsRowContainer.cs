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
    [SerializeField]
    private GameObject UI3DViewPrefab;

    private const int viewsDistance = 20;

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

    public void CreateItemPanel(string itemName, string itemLabel, GameObject UIJewelPrefab, int index, JewelProperties properties, SceneInitializer initializer)
    {
        GameObject itemPanel = Instantiate(itemPanelPrefab, horizontalLayoutContainer.transform);
        GameObject UI3Dview = Instantiate(UI3DViewPrefab, Vector3.right * index * viewsDistance, Quaternion.identity);
        UIJewelCamera jewelCamera = UI3Dview.GetComponent<UIJewelCamera>();
        jewelCamera.SetUIJewelPrefab(UIJewelPrefab);
        RenderTexture renderTex = jewelCamera.RenderTex;

        itemPanel.GetComponent<ItemPanel>().InitializeItem(itemName, itemLabel, renderTex, properties, initializer);

    }
}
