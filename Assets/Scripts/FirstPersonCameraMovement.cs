using UnityEngine;
using UnityEngine.InputSystem;
public class CameraController : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 50f;
    private float xRotation = 0f;
    private float yRotation = 0f;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerBody != null)
        {
            yRotation = playerBody.eulerAngles.y;
        }
    }

    void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * Time.deltaTime;

        // Positive Y mouse movement looks up which is a negative X-axis rotation.
        xRotation = Mathf.Clamp(xRotation - mouseDelta.y, -90f, 90f);
        yRotation += mouseDelta.x;

        // Apply pitch to the camera and yaw to the player body so the character can turn 360°.
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if (playerBody != null)
        {
            playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }
}
