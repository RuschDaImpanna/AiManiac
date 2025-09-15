using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float force = 50f;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    public bool showSpeed = false;
    private Vector3 previousVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        InputAction moveAction = playerInput.actions["Move"];

        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveInput = Vector2.zero;

        rb.AddForce(new Vector3(0f, 0f, 1f) * force);
    }

    void FixedUpdate()
    {
        // Show speed and acceleration in console
        if (showSpeed)
        {
            float speed = rb.linearVelocity.magnitude;
            float acceleration = ((rb.linearVelocity - previousVelocity) / Time.fixedDeltaTime).magnitude;
            Debug.Log("Speed (m/s): " + speed + " Acceleration (m/s^2): " + acceleration + " Speed (km/h): " +  speed * 3.6);
        }
        previousVelocity = rb.linearVelocity;
        rb.AddForce(new Vector3(moveInput.x, 0f, moveInput.y) * force);

    }
}
