using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIJewelCamera : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    private RenderTexture renderTexture;

    [SerializeField]
    private GameObject jewelContainer;
    private GameObject UIJewelPrefab;

    public RenderTexture RenderTex
    {
        get { return renderTexture; }
    }
    // Start is called before the first frame update
    void Awake()
    {
        renderTexture = new RenderTexture(820, 560, 16);
    }

    private void Start()
    {
        cam.targetTexture = renderTexture;
        Instantiate(UIJewelPrefab, jewelContainer.transform);
    }

    public void SetUIJewelPrefab(GameObject jewelPrefab)
    {
        UIJewelPrefab = jewelPrefab;
    }
}
