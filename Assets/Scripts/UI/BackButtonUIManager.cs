using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject androidBackButton;
    [SerializeField]
    private GameObject iosBackButton;
    // Start is called before the first frame update
    void Awake()
    {
#if UNITY_ANDROID
        androidBackButton.SetActive(true);
#endif

#if UNITY_IOS
        iosBackButton.SetActive(true);
#endif
    }
}
