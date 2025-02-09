using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Mediapipe.Unity {
  public abstract class Solution : MonoBehaviour {
    protected virtual string TAG { get { return this.GetType().Name; } }

    protected SceneInitializer bootstrap;
    protected bool isPaused;

    protected virtual IEnumerator Start() {
      var bootstrapObj = GameObject.Find("Bootstrap");

      if (bootstrapObj == null) {
        //Logger.LogError(TAG, "Bootstrap is not found. Please play 'Start Scene' first");
        yield break;
      }

      bootstrap = bootstrapObj.GetComponent<SceneInitializer>();
      yield return new WaitUntil(() => bootstrap.isFinished);

      Play();
    }

    /// <summary>
    ///   Start the main program from the beginning.
    /// </summary>
    public virtual void Play() {
      isPaused = false;
    }

    /// <summary>
    ///   Pause the main program.
    /// <summary>
    public virtual void Pause() {
      isPaused = true;
    }

    /// <summary>
    ///    Resume the main program.
    ///    If the main program has not begun, it'll do nothing.
    /// </summary>
    public virtual void Resume() {
      isPaused = false;
    }

    /// <summary>
    ///   Stops the main program.
    /// </summary>
    public virtual void Stop() {
      isPaused = true;
    }

    protected static void SetupScreen(RawImage screen, ImageSource imageSource) {
      screen.rectTransform.sizeDelta = new Vector2(imageSource.textureWidth, imageSource.textureHeight);
      screen.rectTransform.localEulerAngles = imageSource.rotation.Reverse().GetEulerAngles();
      if (imageSource.isVerticallyFlipped) {
        screen.uvRect = new UnityEngine.Rect(0, 1, 1, -1);
      }
    }

    protected static void SetupAnnotationController<T>(AnnotationController<T> annotationController, ImageSource imageSource, bool expectedToBeMirrored = false) where T : HierarchicalAnnotation {
      annotationController.isMirrored = expectedToBeMirrored ^ imageSource.isHorizontallyFlipped;
      annotationController.rotationAngle = imageSource.rotation.Reverse();
    }

    protected static void ReadFromImageSource(TextureFrame textureFrame, RunningMode runningMode, GraphRunner.ConfigType configType) {
      var sourceTexture = ImageSourceProvider.imageSource.GetCurrentTexture();

      // For some reason, when the image is coiped on GPU, latency tends to be high.
      // So even when OpenGL ES is available, use CPU to copy images.
      var textureType = sourceTexture.GetType();

      if (textureType == typeof(WebCamTexture)) {
        textureFrame.ReadTextureFromOnCPU((WebCamTexture)sourceTexture);
      } else if (textureType == typeof(Texture2D)) {
        textureFrame.ReadTextureFromOnCPU((Texture2D)sourceTexture);
      } else {
        textureFrame.ReadTextureFromOnCPU(sourceTexture);
      }
    }
  }
}
