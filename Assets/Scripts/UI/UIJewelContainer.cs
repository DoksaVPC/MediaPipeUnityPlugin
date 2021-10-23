using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIJewelContainer : MonoBehaviour
{
    [SerializeField]
    private GameObject UIJewelPrefab;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(UIJewelPrefab, gameObject.transform);
    }
}
