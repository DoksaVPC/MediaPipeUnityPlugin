using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mediapipe.Unity;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    private int sceneIndex;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel")){
            if (sceneIndex > 1)
            {
                ImageSourceProvider.imageSource.Stop();
                SceneManager.LoadSceneAsync(sceneIndex - 1);
            }
            else
            {
                Application.Quit();
            }
            
        }
    }
}
