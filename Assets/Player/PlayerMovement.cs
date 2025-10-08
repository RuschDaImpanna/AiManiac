using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float initialSpeed = 50f;

    private float force = 50f;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    private float speed;
    public float Speed { get { return speed; } }
    public bool showSpeed = false;
    private Vector3 previousVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        // Add a initial velocity to the player (km/h)
        rb.linearVelocity = transform.forward * (initialSpeed / 3.6f);

        InputAction moveAction = playerInput.actions["Move"];

        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveInput = Vector2.zero;

        rb.AddForce(new Vector3(0f, 0f, 1f) * force);
    }

    void FixedUpdate()
    {
        speed = rb.linearVelocity.magnitude;

        // Show speed and acceleration in console
        if (showSpeed)
        {
            float acceleration = ((rb.linearVelocity - previousVelocity) / Time.fixedDeltaTime).magnitude;
            Debug.Log("Speed (m/s): " + speed + " Acceleration (m/s^2): " + acceleration + " Speed (km/h): " +  speed * 3.6);
        }

        previousVelocity = rb.linearVelocity;
        rb.AddForce(new Vector3(moveInput.x, 0f, moveInput.y) * force);

    }
}
