using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandBone : MonoBehaviour
{
    private Transform endLandmark;
    private Transform startLandmark;

    // Update is called once per frame
    void Update()
    {
        if (startLandmark != null)
        {
            transform.position = startLandmark.position;
        }
        if (endLandmark != null)
        {
            Vector3 offset = endLandmark.position - transform.position;
            float boneLength = offset.magnitude / 2;
            transform.up = offset;
            transform.localScale = new Vector3(transform.localScale.x, boneLength, transform.localScale.z);
        }
    }

    public void BindLandmarks(Transform startLandmarkTransform, Transform endLandmarkTransform)
    {
        startLandmark = startLandmarkTransform;
        endLandmark = endLandmarkTransform;
    }
}
