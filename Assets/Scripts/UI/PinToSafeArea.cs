using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PinToSafeArea : MonoBehaviour
{
    private Rect lastSafeArea;
    [SerializeField]
    private RectTransform parentRectTransform;
    [SerializeField]
    private bool isSelectionScreen;
    [SerializeField]
    private VerticalLayoutGroup verticalLayoutGroup;
    private const int defaultPadding = -600;

    private void Update()
    {
        if (lastSafeArea != Screen.safeArea)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        Rect safeAreaRect = Screen.safeArea;
        float scaleRatio = parentRectTransform.rect.width / Screen.width;

        if (!isSelectionScreen)
        {
            var left = 0;
            var right = 0;
            var top = -safeAreaRect.yMin * scaleRatio;
            var bottom = 0;

            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.offsetMin = new Vector2(left, bottom);
            rectTransform.offsetMax = new Vector2(right, top);
        } else
        {
            verticalLayoutGroup.padding.top = defaultPadding + (int)(safeAreaRect.yMin * scaleRatio);
        }   

        lastSafeArea = Screen.safeArea;
    }
}