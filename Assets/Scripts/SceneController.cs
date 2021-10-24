using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mediapipe.Unity;
using Mediapipe.Unity.HandTracking;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    private int sceneIndex;
    [SerializeField]
    private HandTrackingSolution solution;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel")){
            if (sceneIndex > 1)
            {
                GoBack();
            }
            else
            {
                Application.Quit();
            }
            
        }
    }

    public void GoBack()
    {
        solution.Stop();
        SceneManager.LoadSceneAsync(sceneIndex - 1);
    }
}
