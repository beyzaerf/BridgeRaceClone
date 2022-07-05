using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    private float lerpValue = 3;

    private void LateUpdate()
    {
        Vector3 destination = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, destination, lerpValue);
    }
}
