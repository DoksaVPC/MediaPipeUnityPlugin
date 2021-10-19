using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    private float rotation = 0f;
    [SerializeField]
    private float rotationSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        rotation += Time.deltaTime * rotationSpeed;
        gameObject.transform.rotation = Quaternion.Euler(-30, rotation, 1);
    }
}
