using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Transform targetTransform;

    [SerializeField]
    Vector3 offset;

    [SerializeField]
    float yThreshold = 1f;


    [SerializeField]
    float ySmoothTime = 0.1f;
    [SerializeField]
    float xSmoothTime = 0.5f;

    [SerializeField]
    float snapDistance = 0.25f; 

    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        Camera mainCamera = GetComponent<Camera>();
        mainCamera.fieldOfView = 120f; // Set a very small field of view to start the tween from.
        mainCamera.DOFieldOfView(70, 0.5f);
    }

    private void LateUpdate()
    {
        Vector3 target = targetTransform.position + offset;

        Vector3 targetPosition = transform.position;
        float distanceToTarget = Mathf.Abs(target.x - transform.position.x);

        if (distanceToTarget > snapDistance)
        {
            targetPosition.x = Mathf.SmoothDamp(transform.position.x, target.x, ref velocity.x, xSmoothTime);
        }
        else
        {
            targetPosition.x = target.x;
        }


        if (Mathf.Abs(target.y - transform.position.y) > yThreshold)
        {
            targetPosition.y = Mathf.SmoothDamp(transform.position.y, target.y, ref velocity.y, ySmoothTime);
        }
        

        transform.position = targetPosition;


    }
}
