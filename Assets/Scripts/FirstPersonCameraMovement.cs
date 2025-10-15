using UnityEngine;
using UnityEngine.InputSystem;
// using PixelCrushers.DialogueSystem;

public class CameraController : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 0.005f;
    private float xRotation = 0f;
    private float yRotation = 0f;
    private bool isEnabled = true;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerBody != null)
        {
            yRotation = playerBody.eulerAngles.y;
        }
    }

    // void OnEnable()
    // {
    //     DialogueManager.instance.conversationStarted += OnConversationStarted;
    //     DialogueManager.instance.conversationEnded += OnConversationEnded;
    // }

    // void OnDisable()
    // {
    //     if (DialogueManager.instance != null)
    //     {
    //         DialogueManager.instance.conversationStarted -= OnConversationStarted;
    //         DialogueManager.instance.conversationEnded -= OnConversationEnded;
    //     }
    // }

    // void OnConversationStarted(Transform actor)
    // {
    //     // Only pause camera for the "Stay" conversation
    //     if (DialogueManager.lastConversationStarted == "Stay")
    //     {
    //         isEnabled = false;
    //     }
    // }

    // void OnConversationEnded(Transform actor)
    // {
    //     // Only resume if it was the "Stay" conversation ending
    //     if (DialogueManager.lastConversationStarted == "Stay")
    //     {
    //         isEnabled = true;
    //     }
    // }

    void Update()
    {
        if (!isEnabled || Mouse.current == null)
        {
            return;
        }

        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * (mouseSensitivity * 0.01f);

        xRotation = Mathf.Clamp(xRotation - mouseDelta.y, -90f, 90f);
        yRotation += mouseDelta.x;

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        if (playerBody != null)
        {
            playerBody.rotation = Quaternion.Euler(0f, yRotation, 0f);
        }
    }
}