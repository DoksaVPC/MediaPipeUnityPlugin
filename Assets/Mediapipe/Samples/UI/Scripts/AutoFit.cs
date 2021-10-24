using UnityEngine;

namespace Mediapipe.Unity {
  public class AutoFit : MonoBehaviour {
    [System.Serializable]
    public enum FitMode {
      Expand,
      Shrink,
      FitWidth,
      FitHeight,
    }

    [SerializeField] FitMode fitMode;
        [SerializeField] private RectTransform headerRect;
        [SerializeField] private int headerHeight = 182;

    void LateUpdate() {
      var rectTransform = GetComponent<RectTransform>();
      if (rectTransform.rect.width == 0 || rectTransform.rect.height == 0) {
        return;
      }

      var parentRect = gameObject.transform.parent.gameObject.GetComponent<RectTransform>().rect;
      var (width, height) = GetBoundingBoxSize(rectTransform);

      var ratio = parentRect.width / width;
      var w = parentRect.width;
      var h = height * ratio;

      if (fitMode == FitMode.FitWidth || (fitMode == FitMode.Expand && h >= parentRect.height) || (fitMode == FitMode.Shrink && h <= parentRect.height)) {
                float deltaHeight = parentRect.height - h;
                rectTransform.pivot = new Vector2(0.5f - (deltaHeight / 2) / h, 0.5f);
        rectTransform.offsetMin *= ratio;
        rectTransform.offsetMax *= ratio;
                if (deltaHeight <= headerHeight)
                {
                    headerRect.offsetMin = Vector2.down * headerHeight;
                } else
                {
                    headerRect.offsetMin = Vector2.down * deltaHeight;
                }
        return;
      }

      ratio = parentRect.height / height;
      w = width * ratio;
      h = parentRect.height;

      rectTransform.offsetMin *= ratio;
      rectTransform.offsetMax *= ratio;
    }

    (float, float) GetBoundingBoxSize(RectTransform rectTransform) {
      var rect = rectTransform.rect;
      var center = rect.center;
      var topLeftRel = new Vector2(rect.xMin - center.x, rect.yMin - center.y);
      var topRightRel = new Vector2(rect.xMax - center.x, rect.yMin - center.y);
      var rotatedTopLeftRel = rectTransform.rotation * topLeftRel;
      var rotatedTopRightRel = rectTransform.rotation * topRightRel;
      var wMax = Mathf.Max(Mathf.Abs(rotatedTopLeftRel.x), Mathf.Abs(rotatedTopRightRel.x));
      var hMax = Mathf.Max(Mathf.Abs(rotatedTopLeftRel.y), Mathf.Abs(rotatedTopRightRel.y));
      return (2 * wMax, 2 * hMax);
    }
  }
}