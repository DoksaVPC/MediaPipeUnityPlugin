using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinToSafeArea : MonoBehaviour
{
    private Rect lastSafeArea;
    [SerializeField]
    private RectTransform parentRectTransform;

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

        var left = safeAreaRect.xMin * scaleRatio;
        var right = -(Screen.width - safeAreaRect.xMax) * scaleRatio;
        var top = -safeAreaRect.yMin * scaleRatio;
        var bottom = (Screen.height - safeAreaRect.yMax) * scaleRatio;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.offsetMin = new Vector2(left, bottom);
        rectTransform.offsetMax = new Vector2(right, top);

        lastSafeArea = Screen.safeArea;
    }
}