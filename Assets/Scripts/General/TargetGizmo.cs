using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TargetGizmo : MonoBehaviour
{
    [SerializeField] private int targetNumber;
    [SerializeField] private Color gizmoColor;
    [SerializeField] private Vector3 textPosition;

    private void OnDrawGizmos()
    {
        Handles.Label(
            new Vector3(
                transform.position.x + textPosition.x,
                transform.position.y + textPosition.y,
                transform.position.z + textPosition.z),
           "Target " + targetNumber);

        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }  
}
