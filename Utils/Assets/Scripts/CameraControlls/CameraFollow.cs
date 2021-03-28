using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private float smoothFactor = 1f;
    [SerializeField]
    private Vector3 offset;

    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;

    private void FixedUpdate()
    {
        desiredPosition = target.position + desiredPosition;
        smoothedPosition = Vector3.Slerp(transform.position, desiredPosition, smoothFactor * Time.deltaTime);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }

}
