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
        // We can set to false later, or avoid locking the cursor in the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    void Update()
    {
        float mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity * Time.deltaTime;
        float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity * Time.deltaTime;

        // -= because a positive mouse Y movement means moving the mouse up, which should cause a negative rotation around the X-axis (looking up).
        xRotation -= mouseY;
        yRotation += mouseX;
        // Clamp rotation to prevent the camera from flipping
        xRotation = Mathf.Clamp(xRotation, -45f, 45f);
        yRotation = Mathf.Clamp(yRotation, -45f, 45f);

        // Apply the vertical rotation to the camera itself (the object this script is on).
        // We rotate around the X-axis to look up and down, rotate the Y-axis to look left and right
        // We can change this based on keyboard inputs instead of the mouse
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

    }
}
