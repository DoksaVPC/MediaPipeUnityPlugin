using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelector : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    private int sceneIndex = 0;
    public int SceneIndex
    {
        get { return sceneIndex; }
    }

    public void ChangeScene(int index)
    {
        sceneIndex = index;
        SceneManager.LoadSceneAsync(1);
    }
}
