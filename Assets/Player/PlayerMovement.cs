using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float initialSpeed = 50f;

    private float force = 50f;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private Vector2 moveInput;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        // Add a initial velocity to the player (km/h)
        rb.linearVelocity = transform.forward * (initialSpeed / 3.6f);

        InputAction moveAction = playerInput.actions["Move"];

        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveInput = Vector2.zero;
    }

    void FixedUpdate()
    {

        rb.AddForce(new Vector3(moveInput.x, 0f, moveInput.y) * force);

    }
}
