using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LeTai.TrueShadow;

public class SizeButton : SwitchButton
{
    private SizeValues sizeValues = new SizeValues(0.85f, 1f, 1.15f);

    [SerializeField]
    private JewelSizes jewelSize;

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
        DisplayCurrentActive();
        DisplayOthersInactive();
    }
}
