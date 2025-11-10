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
        playerInput = GetComponent<PlayerInput>();
        
        InputAction moveAction = playerInput.actions["Move"];

        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveInput = Vector2.zero;

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on this GameObject!");
        }

        // Add a initial velocity to the player (km/h)
        Vector3 angles = rb.rotation.eulerAngles;
        float xAngleRad = angles.x * Mathf.PI / 180;
        Vector3 speed = new Vector3(0f, -initialSpeed * Mathf.Sin(xAngleRad) / 3.6f, initialSpeed * Mathf.Cos(xAngleRad) / 3.6f);
        Debug.Log(Mathf.Sin(50f * xAngleRad) / 3.6f);
        rb.linearVelocity = speed;
        //Debug.Log("Initial velocity and velocity magnitued set to: " + new Vector3(rb.linearVelocity.x * 3.6f, rb.linearVelocity.y * 3.6f, rb.linearVelocity.z * 3.6f) + " " + rb.linearVelocity.magnitude * 3.6f + " for angle: " + angles.x);
        //Debug.Log("Speed: " + new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z));
        //Debug.Log("Speed: " + speed.magnitude * 3.6f + " km/h");
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
