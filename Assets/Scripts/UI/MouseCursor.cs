using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    [Header("Scene Camera")]
    [SerializeField] private Camera sceneCamera;

    private Vector2 mousePosition;

    private void Start()
    {
        /* Makes the system mouse cursor invisible and confined within the screen when game is playing. */
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        /* Converts to the mouse position to a world point position in the scene. */
        mousePosition = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
    }
}
