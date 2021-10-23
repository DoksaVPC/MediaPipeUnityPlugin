using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JewelProperties
{
    [SerializeField]
    private GameObject jewelPrefab;
    [SerializeField]
    private JewelType jewelType;
    [SerializeField]
    private bool jewelFlips;

    public GameObject JewelPrefab
    {
        get { return jewelPrefab; }
    }
    public JewelType Type
    {
        get { return jewelType; }
    }
    public bool Filps
    {
        get
        {
            return jewelFlips;
        }
    }
    public enum JewelType
    {
        Ring,
        Bracelet
    }

    public JewelProperties(GameObject jewel, JewelType type, bool flips)
    {
        jewelPrefab = jewel;
        jewelType = type;
        jewelFlips = flips;
    }
}
