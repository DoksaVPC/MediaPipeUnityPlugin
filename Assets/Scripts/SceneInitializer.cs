using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mediapipe.Unity;

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
        private CameraSource cameraSource;
        [SerializeField] private ImageSource.SourceType defaultImageSource;
        [SerializeField] private InferenceMode preferableInferenceMode;
        [SerializeField] private AssetLoaderType assetLoaderType;

        private JewelProperties jewelProperties;
        public JewelProperties JewelProperties
        {
            get { return jewelProperties; }
        }

        public InferenceMode inferenceMode { get; private set; }
        public bool isFinished { get; private set; }

        IEnumerator Start()
        {
            GlobalConfigManager.SetFlags();

            ApplicationChrome.statusBarState = ApplicationChrome.States.Visible;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            cameraSource = imageSource.GetComponent<CameraSource>();

            //Debug.Log("Initializing AssetLoader...");
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
          yield break;
#endif
                    }
            }

            DecideInferenceMode();
            if (inferenceMode == InferenceMode.GPU)
            {
                //Debug.Log("Initializing GPU resources...");
                yield return GpuManager.Initialize();
            }

            //Debug.Log("Preparing ImageSource...");
            ImageSourceProvider.SwitchSource(imageSource.GetComponent<CameraSource>());
            DontDestroyOnLoad(imageSource);
            DontDestroyOnLoad(gameObject);
            isFinished = true;

            //Debug.Log("Loading hand tracking scene...");
            var sceneLoadReq = SceneManager.LoadSceneAsync(1);
            yield return new WaitUntil(() => sceneLoadReq.isDone);
        }

        void DecideInferenceMode()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
            if (preferableInferenceMode == InferenceMode.GPU)
            {
                //Debug.LogWarning("Current platform does not support GPU inference mode, so falling back to CPU mode");
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

        public void StartTryOn(JewelProperties properties)
        {
            jewelProperties = properties;
            SceneManager.LoadSceneAsync(2);
            StartCoroutine(cameraSource.InitializeCameraSource());

    }
    }
