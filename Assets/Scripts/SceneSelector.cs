using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    private void Awake()
    {
        ApplicationChrome.statusBarState = ApplicationChrome.States.Visible;
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    private int sceneIndex = 0;
    public int SceneIndex
    {
        get { return sceneIndex; }
    }
}
