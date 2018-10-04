using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.forward = -cameraTransform.forward;
    }
}