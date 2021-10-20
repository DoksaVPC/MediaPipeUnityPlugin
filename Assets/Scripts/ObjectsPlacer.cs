using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    private GameObject jewelPrefab;
    [SerializeField]
    private JewelType jewelType;
    [SerializeField]
    private bool jewelFlips = false;

    private enum JewelType
    {
        Ring,
        Bracelet
    }

    private GameObject[] landmarkSpheres = new GameObject[21];
    private GameObject[] handBones = new GameObject[21];
    private GameObject jewel;

    private CameraSource cameraSource;
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

    private const float landmarkBaseSize = 0.85f;
    private const float zNegativeRecalibration = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        pixelWidth = cam.pixelWidth;
        pixelHeight = cam.pixelHeight;
        cameraSource = GameObject.FindGameObjectWithTag("ImageSource").GetComponent<CameraSource>();
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
        jewel = Instantiate(jewelPrefab);
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

                    landmarkRawPositions[i] = new Vector3(pixelWidth - (handLandmarks[0].Landmark[i].X * pixelWidth), pixelHeight - heightDelta / 2 - (handLandmarks[0].Landmark[i].Y * trueHeight), 10 + zDelta*10);
                    landmarkNormalizedValues[i] = new Vector3(handLandmarks[0].Landmark[i].X, handLandmarks[0].Landmark[i].Y * textureHeight / textureWidth, zDelta);
                }
            }
        }
    }

    private void Update()
    {
        float handSizeFactor = GetHandSize() * 2f;
        int handednessValue = 1;
        if (jewelFlips)
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
            landmarkSpheres[i].transform.localScale = Vector3.one * (handSizeFactor * landmarkBaseSize);
        }

        foreach(GameObject handBone in handBones)
        {
            handBone.transform.localScale = new Vector3(handSizeFactor * landmarkBaseSize * 0.9f, handBone.transform.localScale.y, handSizeFactor * landmarkBaseSize * 0.9f);
        }

        jewel.transform.localScale = new Vector3(-1, handednessValue, 1) * (handSizeFactor * landmarkBaseSize);
        PlaceJewel();

        if(textureWidth == 0 || textureHeight == 0)
        {
            if (cameraSource.textureWidth > 0 && cameraSource.textureHeight > 0)
            {
                SetDimensions();
            }
        } else
        {
            if (!timerStarted)
            {
                handSwitchingRemainingTime = handSwitchingMaxTime;
                timerStarted = true;
            }
        }

        if (!landmarksActive)
        {
            if (handRecognized)
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
        return Vector3.Distance(landmarkNormalizedValues[5], landmarkNormalizedValues[17]);
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
        
        if (jewelType == JewelType.Ring)
        {        
            Quaternion rotation = Quaternion.LookRotation(landmarkSpheres[14].transform.position - landmarkSpheres[13].transform.position, palmNormal);
            float distance = 0.6f;
            Vector3 ringCoordinate = (1 - distance) * landmarkSpheres[13].transform.position + distance * landmarkSpheres[14].transform.position;
            jewel.transform.rotation = rotation;
            jewel.transform.position = ringCoordinate;
        } else if (jewelType == JewelType.Bracelet)
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
}
