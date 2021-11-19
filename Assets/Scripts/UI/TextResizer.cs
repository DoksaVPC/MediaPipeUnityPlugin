using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextResizer : MonoBehaviour
{
    [SerializeField]
    RectTransform rect;
    [SerializeField]
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text.preferredWidth);
    }

    // Update is called once per frame
    void Update()
    {
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text.preferredWidth);
    }
}
