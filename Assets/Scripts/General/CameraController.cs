using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float cameraSpeed = 5f;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float zPosition = -10f;

    public Vector3 offset;



    private void LateUpdate()
    {
        
        // Clamp the camera position within the specified bounds.
        if (player != null)
        {
            Vector3 targetPosition = player.position + offset;

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
            targetPosition.z = zPosition;

            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraSpeed);
        }
       
        
    }
}