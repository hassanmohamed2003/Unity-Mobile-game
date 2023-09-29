using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public CameraFollow cameraScript;
    
    [Range(0.0f, 1.0f)]
    public float CameraTargetHeight;
    public UnityEvent<Camera> CameraPositionUpdated;
    public UnityEvent CameraFinishedMovementEvent;    
    private Vector3 prevCameraWorldPos;
    private bool isMoving = false;

    void Start()
    {
        prevCameraWorldPos = Camera.main.transform.position;
    }

    void Update()
    {
        // If camera's position was updated
        if(Camera.main.transform.position != prevCameraWorldPos)
        {
            // Invoke the position update event
            CameraPositionUpdated.Invoke(Camera.main);
            prevCameraWorldPos = Camera.main.transform.position;
            isMoving = true;
        }
        else if(isMoving)
        {
            CameraFinishedMovementEvent.Invoke();
            isMoving = false;
        }        
    }

    public void UpdateCameraTargetPosition(Transform highestBlock)
    {
        Debug.Log("Updating camera position");
        if(highestBlock != null)
        {
            // Get the camera's world position at the specified target height
            Vector3 cameraWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(0, CameraTargetHeight, 0));

            // Check if the highest block is above the target height
            if( highestBlock.gameObject.TryGetComponent(out BuildingBlock block) && 
                (block.transform.position.y + block.ropeStartOffset) > cameraWorldPos.y)
            {
                // Calculate offset from bottom to highest block
                Vector3 bottomScreenWorldPos = Camera.main.ViewportToWorldPoint(new Vector3(0,0,0));
                float offset = block.transform.position.y + block.ropeStartOffset - bottomScreenWorldPos.y;

                // Add multiple of offset to camera's targetpos to update it
                cameraScript.targetPos.y = bottomScreenWorldPos.y + (0.5f/CameraTargetHeight * offset);
            }
        }
    }
}
