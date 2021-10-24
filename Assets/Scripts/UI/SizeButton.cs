using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizeButton : MonoBehaviour
{
    [SerializeField]
    private ObjectsPlacer objectsPlacer;

    private SizeValues sizeValues = new SizeValues(0.85f, 1f, 1.15f);

    [SerializeField]
    private JewelSizes jewelSize;

    [SerializeField]
    private Sprite filledTexture;

    [SerializeField]
    private Button button;
    [SerializeField]
    private Text buttonText;

    [SerializeField]
    private Text[] otherButtonsTexts;

    [SerializeField]
    private bool isActive = false;

    private Color black = new Color(0.2f, 0.2f, 0.2f);
    private Color white = new Color(0.8f, 0.8f, 0.8f);

    private enum JewelSizes
    {
        S,
        M,
        L
    }

    public struct SizeValues
    {
        private float s;
        private float m;
        private float l;

        public float S
        {
            get { return s; }
        }
        public float M
        {
            get { return m; }
        }
        public float L
        {
            get { return l; }
        }

        public SizeValues(float s, float m, float l)
        {
            this.s = s;
            this.m = m;
            this.l = l;
        }
    }

    private void Start()
    {
        if (isActive)
        {
            //buttonRenderer.sprite = filledTexture;
            button.Select();
            buttonText.color = black;
            //buttonRenderer.size = new Vector2(140, 140);
        }
    }

    public void SetJewelSize()
    {
        if(jewelSize == JewelSizes.S)
        {
            objectsPlacer.SetJewelSize(sizeValues.S);
        } else if(jewelSize == JewelSizes.M)
        {
            objectsPlacer.SetJewelSize(sizeValues.M);
        }
        else
        {
            objectsPlacer.SetJewelSize(sizeValues.L);
        }
        buttonText.color = black;
        foreach(Text buttonText in otherButtonsTexts)
        {
            buttonText.color = white;
        }
    }

}
