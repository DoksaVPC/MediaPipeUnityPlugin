using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mediapipe;
using Mediapipe.Unity;
using Mediapipe.Unity.HandTracking;

public class ObjectsPlacer : MonoBehaviour
{
    [SerializeField]
    private HandTrackingGraph graph;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private GameObject landmarkSpherePrefab;
    [SerializeField]
    private GameObject bonePrefab;
    [SerializeField]
    private JewelProperties jewelProperties;

    [SerializeField]
    private RectTransform[] tutorialLandmarkPositionsBracelet;
    [SerializeField]
    private RectTransform[] tutorialLandmarkPositionsRing;
    [SerializeField]
    private Sprite handIndicatorRingSprite;
    private RectTransform[] tutorialLandmarkPositions;
    [SerializeField]
    private GameObject tutorialPanel;
    [SerializeField]
    private Image tutorialImage;
    [SerializeField]
    private GameObject sizesPanel;

    private GameObject[] landmarkSpheres = new GameObject[21];
    private GameObject[] handBones = new GameObject[21];
    private GameObject jewel;

    private CameraSource cameraSource;
    private SceneInitializer sceneInitializer;
    private float textureWidth;
    private float textureHeight;
    private float aspectRatio;
    private float heightDelta;
    private float trueHeight;
    private float pixelWidth;
    private float pixelHeight;

    private Vector3[] landmarkNormalizedValues = new Vector3[21];
    private Vector3[] landmarkRawPositions = new Vector3[21];

    private bool landmarksActive = false;

    private bool handRecognized = false;
    private string currentHand;
    private string lastDetectedHand;
    [SerializeField]
    private float handSwitchingMaxTime = 1f;
    private float handSwitchingRemainingTime;
    private bool timerStarted = false;

    private const float landmarkBaseSize = 0.0008f;
    private const float zNegativeRecalibration = 0.25f;
    private float jewelSize;



    // Start is called before the first frame update
    void Start()
    {
        pixelWidth = cam.pixelWidth;
        pixelHeight = cam.pixelHeight;
        cameraSource = GameObject.FindGameObjectWithTag("ImageSource").GetComponent<CameraSource>();
        sceneInitializer = GameObject.FindGameObjectWithTag("Global Resource").GetComponent<SceneInitializer>();
        jewelProperties = sceneInitializer.JewelProperties;
        if (jewelProperties.Type == JewelProperties.JewelType.Ring)
        {
            tutorialImage.sprite = handIndicatorRingSprite;
            tutorialLandmarkPositions = tutorialLandmarkPositionsRing;
        } else
        {
            tutorialLandmarkPositions = tutorialLandmarkPositionsBracelet;
        }
        SetJewelSize(1f);
        for (int i = 0; i < landmarkSpheres.Length; i++)
        {
            landmarkSpheres[i] = Instantiate(landmarkSpherePrefab);
            handBones[i] = Instantiate(bonePrefab);
            if (i > 0)
            {
                if (i == 5)
                {
                    handBones[i].GetComponent<HandBone>().BindLandmarks(landmarkSpheres[i].transform, landmarkSpheres[0].transform);
                }
                else if (i == 9 || i == 13 || i == 17)
                {
                    handBones[i].GetComponent<HandBone>().BindLandmarks(landmarkSpheres[i].transform, landmarkSpheres[i - 4].transform);
                    if(i == 17)
                    {
                        handBones[0].GetComponent<HandBone>().BindLandmarks(landmarkSpheres[0].transform, landmarkSpheres[17].transform);
                    }
                }
                else
                {
                    handBones[i].GetComponent<HandBone>().BindLandmarks(landmarkSpheres[i].transform, landmarkSpheres[i - 1].transform);
                }
            }
            handBones[i].SetActive(false);
            landmarkSpheres[i].SetActive(false);
        }
        jewel = Instantiate(jewelProperties.JewelPrefab);
        jewel.SetActive(false);

        graph.OnHandLandmarksOutput.AddListener(OnLandmarksOutput);
        graph.OnHandednessOutput.AddListener(OnHandednessOutput);
    }

    private void OnHandednessOutput(List<ClassificationList> handedness)
    {
        if (handedness != null && handedness[0] != null && handedness[0].Classification[0] != null)
        {
            Debug.Log(handedness[0].Classification[0].Label);
            lastDetectedHand = handedness[0].Classification[0].Label;
        }
    }

    private void OnLandmarksOutput(List<NormalizedLandmarkList> handLandmarks)
    {
        if (handLandmarks != null && handLandmarks[0] != null && pixelWidth > 0)
        {
            if (textureWidth > 0 && textureHeight > 0)
            {
                for (int i = 0; i < landmarkRawPositions.Length; i++)
                {
                    float zDelta = 0;
                    if (i != 0)
                    {
                        zDelta = handLandmarks[0].Landmark[i].Z;
                        if (zDelta < 0)
                        {
                            zDelta *= zNegativeRecalibration;
                        }
                    }

                    landmarkRawPositions[i] = new Vector3(pixelWidth - (handLandmarks[0].Landmark[i].X * pixelWidth), pixelHeight - heightDelta - (handLandmarks[0].Landmark[i].Y * trueHeight), 10 + zDelta*10);
                    landmarkNormalizedValues[i] = new Vector3(handLandmarks[0].Landmark[i].X, handLandmarks[0].Landmark[i].Y, zDelta);
                }
            }
        }
    }

    private void Update()
    {

        if (tutorialPanel.activeInHierarchy)
        {
            CheckForCorrectPosition();
        }
        float handSizeFactor = GetHandSize() * 2f;
        int handednessValue = 1;
        if (jewelProperties.Filps)
        {
            if (currentHand == "Left")
            {
                handednessValue = 1;
            }
            else
            {
                handednessValue = -1;
            }
        }

        for (int i = 0; i < landmarkSpheres.Length; i++)
        {
            landmarkSpheres[i].transform.position = cam.ScreenToWorldPoint(landmarkRawPositions[i]);
            landmarkSpheres[i].transform.localScale = Vector3.one * (handSizeFactor * landmarkBaseSize * jewelSize);
        }

        foreach (GameObject handBone in handBones)
        {
            handBone.transform.localScale = new Vector3(handSizeFactor * landmarkBaseSize * 0.9f * jewelSize, handBone.transform.localScale.y, handSizeFactor * landmarkBaseSize * 0.9f * jewelSize);
        }

        if (jewel != null)
        {
            jewel.transform.localScale = new Vector3(-1, handednessValue, 1) * (handSizeFactor * landmarkBaseSize * jewelSize);
            PlaceJewel();
        }

        if (textureWidth == 0 || textureHeight == 0)
        {
            if (cameraSource.textureWidth > 0 && cameraSource.textureHeight > 0)
            {
                SetDimensions();
            }
        }
        else
        {
            if (!timerStarted)
            {
                handSwitchingRemainingTime = handSwitchingMaxTime;
                timerStarted = true;
            }
        }

        if (!landmarksActive)
        {
            if (handRecognized && !tutorialPanel.activeInHierarchy)
            {
                ToggleLandmarks();
            }
        }

        if (timerStarted)
        {
            CheckHandedness();
        }
    }

    private void SetDimensions()
    {
        textureWidth = cameraSource.textureWidth;
        textureHeight = cameraSource.textureHeight;
        aspectRatio = textureWidth / textureHeight;
        trueHeight = pixelWidth * aspectRatio;
        heightDelta = pixelHeight - trueHeight;
    }

    private float GetHandSize()
    {
        return Vector3.Distance(landmarkRawPositions[5], landmarkRawPositions[17]);
    }

    private void ToggleLandmarks()
    {
        foreach(GameObject landmarkSphere in landmarkSpheres)
        {
            if (landmarkSphere.activeInHierarchy)
            {
                landmarkSphere.SetActive(false);
                if (landmarksActive)
                {
                    landmarksActive = false;
                }
            } else
            {
                landmarkSphere.SetActive(true);
                if (!landmarksActive)
                {
                    landmarksActive = true;
                }
            }
        }
        foreach(GameObject handbone in handBones)
        {
            if (handbone.activeInHierarchy)
            {
                handbone.SetActive(false);
            }
            else
            {
                handbone.SetActive(true);
            }
        }
        if (jewel.activeInHierarchy)
        {
            jewel.SetActive(false);
        } else
        {
            jewel.SetActive(true);
        }
    }

    private void PlaceJewel()
    {
        Vector3 wristIndex = landmarkSpheres[17].transform.position - landmarkSpheres[0].transform.position;
        Vector3 wristPinky = landmarkSpheres[5].transform.position - landmarkSpheres[0].transform.position;
        Vector3 palmNormal;
        if (currentHand == lastDetectedHand)
        {
            palmNormal = Vector3.Cross(wristIndex, wristPinky);
        } else
        {
            palmNormal = Vector3.Cross(wristIndex, wristPinky) * -1;
        }
        
        if (jewelProperties.Type == JewelProperties.JewelType.Ring)
        {        
            Quaternion rotation = Quaternion.LookRotation(landmarkSpheres[14].transform.position - landmarkSpheres[13].transform.position, palmNormal);
            float distance = 0.45f;
            Vector3 ringCoordinate = (1 - distance) * landmarkSpheres[13].transform.position + distance * landmarkSpheres[14].transform.position;
            jewel.transform.rotation = rotation;
            jewel.transform.position = ringCoordinate;
        } else if (jewelProperties.Type == JewelProperties.JewelType.Bracelet)
        {
            Vector3 forwardVector = landmarkSpheres[9].transform.position - landmarkSpheres[0].transform.position;
            Quaternion rotation = Quaternion.LookRotation(forwardVector, palmNormal);
            jewel.transform.rotation = rotation;
            jewel.transform.position = landmarkSpheres[0].transform.position;
        }
        
    }

    private void CheckHandedness()
    {
        if (lastDetectedHand != currentHand || lastDetectedHand == null)
        {
            handSwitchingRemainingTime -= Time.deltaTime;
        } else
        {
            handSwitchingRemainingTime = handSwitchingMaxTime;
        }

        if (handSwitchingRemainingTime <= 0)
        {
            SwitchHand(lastDetectedHand);
        }
    }

    private void SwitchHand(string handLabel)
    {
        currentHand = handLabel;
        if (!handRecognized)
        {
            handRecognized = true;
        }
    }

    public void ChangeJewel(JewelProperties properties)
    {
        jewelProperties = properties;
        if (jewel != null)
        {
            Destroy(jewel);
            jewel = Instantiate(properties.JewelPrefab);
        }
    }

    public void SetJewelSize(float size)
    {
        jewelSize = size;
    }

    private void CheckForCorrectPosition()
    {
        float minDistance = pixelWidth / 13;
        Debug.Log("min distance: " + minDistance);
        Vector2 indexPos = cam.WorldToScreenPoint(landmarkSpheres[5].transform.position);      
        Vector2 pinkyPos = cam.WorldToScreenPoint(landmarkSpheres[17].transform.position);     
        Vector2 correctIndexPos = tutorialLandmarkPositions[0].position;
        Vector2 correctPinkyPos = tutorialLandmarkPositions[1].position;
        Debug.Log("real index: " + indexPos.x + ", " + indexPos.y);
        Debug.Log("correct index: " + correctIndexPos.x + ", " + correctIndexPos.y);
        Debug.Log("real pinky: " + pinkyPos.x + ", " + pinkyPos.y);
        Debug.Log("correct pinky: " + correctPinkyPos.x + ", " + correctPinkyPos.y);
        if (Vector2.Distance(indexPos, correctIndexPos) <= minDistance && Vector2.Distance(pinkyPos, correctPinkyPos) <= minDistance)
        {
            tutorialPanel.SetActive(false);
            sizesPanel.SetActive(true);
        }
    }
}
