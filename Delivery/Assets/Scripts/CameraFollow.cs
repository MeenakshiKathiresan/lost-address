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
    float offsetY = 1f;

    [SerializeField]
    float thresholdY = 3f;


    [SerializeField]
    float ySmoothTimeUp = 0.35f;

    [SerializeField]
    float ySmoothTimeDown = 0.1f;
    [SerializeField]
    float xSmoothTime = 0.5f;


    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        //Camera mainCamera = GetComponent<Camera>();
        //mainCamera.fieldOfView = 120f; // Set a very small field of view to start the tween from.
        //mainCamera.DOFieldOfView(70, 0.5f);
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    void OnGameStateChange(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.InGame:
                
                Vector3 target = targetTransform.position + offset;
                Vector3 targetPosition = transform.position;

                targetPosition.x = target.x;
                targetPosition.y = target.y;

                transform.position = targetPosition;

                break;
        }
    }


    private void LateUpdate()
    {
        Vector3 target = targetTransform.position + offset;

        Vector3 targetPosition = transform.position;

       
        targetPosition.x = target.x;

        


        if (GameManager.instance.player.canClimb || GameManager.instance.player.isDownLadder || GameManager.instance.player.IsGrounded())
        {
    
            float ySmoothTime = ySmoothTimeUp;

            if (GameManager.instance.player.isDownLadder)
            {
                ySmoothTime = ySmoothTimeDown;
            }


            // follow player y only while climbing up or down and grounded
            if (GameManager.instance.player.IsGrounded())
            {
                targetPosition.y = Mathf.SmoothDamp(transform.position.y, target.y, ref velocity.y, ySmoothTime);
            }
            
        }
        

        transform.position = targetPosition;


    }
}
