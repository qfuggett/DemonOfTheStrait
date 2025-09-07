using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Rigidbody rb;
    private bool isGrounded;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector2 movementInput = Keyboard.current.wKey.isPressed ? Vector2.up : 
                                Keyboard.current.sKey.isPressed? Vector2.down :
                                Vector2.zero;
        if (Keyboard.current.aKey.isPressed) movementInput.x -= 1;
        if (Keyboard.current.dKey.isPressed) movementInput.x += 1;

        // Calculate the movement vector based on the camera's direction.
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0; //move on horizontal plane
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * movementInput.y + cameraRight * movementInput.x).normalized;
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);
    }
    
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Check if the collision is with the ground layer or any other surface, use to eval if player is off the ground, if so diasble jumping
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}
