using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Mediapipe.Unity
{
    public class SceneInitializer : MonoBehaviour
    {
        [System.Serializable]
        public enum AssetLoaderType
        {
            StreamingAssets,
            AssetBundle,
            Local,
        }

        [SerializeField] private GameObject imageSource;
        [SerializeField] private ImageSource.SourceType defaultImageSource;
        [SerializeField] private InferenceMode preferableInferenceMode;
        [SerializeField] private AssetLoaderType assetLoaderType;

        public InferenceMode inferenceMode { get; private set; }
        public bool isFinished { get; private set; }

        private SceneSelector sceneSelector;

        IEnumerator Start()
        {
            GlobalConfigManager.SetFlags();

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            sceneSelector = GameObject.FindGameObjectWithTag("SceneSelector").GetComponent<SceneSelector>();

            Debug.Log("Initializing AssetLoader...");
            switch (assetLoaderType)
            {
                case AssetLoaderType.AssetBundle:
                    {
                        AssetLoader.Provide(new AssetBundleResourceManager(Path.Combine(Application.streamingAssetsPath, "mediapipe")));
                        break;
                    }
                case AssetLoaderType.StreamingAssets:
                    {
                        AssetLoader.Provide(new StreamingAssetsResourceManager());
                        break;
                    }
                default:
                    {
#if UNITY_EDITOR
                        AssetLoader.Provide(new LocalResourceManager());
                        break;
#else
          Logger.LogError("LocalResourceManager is only supported on UnityEditor");
          yield break;
#endif
                    }
            }

            DecideInferenceMode();
            if (inferenceMode == InferenceMode.GPU)
            {
                Debug.Log("Initializing GPU resources...");
                yield return GpuManager.Initialize();
            }

            Debug.Log("Preparing ImageSource...");
            ImageSourceProvider.SwitchSource(defaultImageSource);
            DontDestroyOnLoad(imageSource);

            DontDestroyOnLoad(gameObject);
            isFinished = true;

            Debug.Log("Loading hand tracking scene...");
            var sceneLoadReq = SceneManager.LoadSceneAsync(sceneSelector.SceneIndex);
            yield return new WaitUntil(() => sceneLoadReq.isDone);
        }

        private void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                ImageSourceProvider.imageSource.Stop();
                Destroy(imageSource);
                Destroy(sceneSelector);
                GpuManager.Shutdown();
                Destroy(gameObject);
                SceneManager.LoadSceneAsync(0);
            }
        }

        void DecideInferenceMode()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
            if (preferableInferenceMode == InferenceMode.GPU)
            {
                Debug.LogWarning("Current platform does not support GPU inference mode, so falling back to CPU mode");
            }
            inferenceMode = InferenceMode.CPU;
#else
      inferenceMode = preferableInferenceMode;
#endif
        }

        void OnApplicationQuit()
        {
            GpuManager.Shutdown();
        }

        private void OnDisable()
        {
            GpuManager.Shutdown();
        }
    }
}
