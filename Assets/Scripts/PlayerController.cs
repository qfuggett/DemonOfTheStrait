using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Rigidbody rb;
    private bool isGrounded;
    private bool jumpQueued;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        Vector2 movementInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) movementInput.y += 1f;
        if (Keyboard.current.sKey.isPressed) movementInput.y -= 1f;
        if (Keyboard.current.aKey.isPressed) movementInput.x -= 1f;
        if (Keyboard.current.dKey.isPressed) movementInput.x += 1f;

        if (movementInput.sqrMagnitude > 1f)
        {
            movementInput.Normalize();
        }

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = (forward * movementInput.y + right * movementInput.x);
        Vector3 targetVelocity = moveDirection * moveSpeed;
        targetVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVelocity;

        if (jumpQueued)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpQueued = false;
            isGrounded = false;
        }
    }

    void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            jumpQueued = true;
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
