using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private float force = 50f;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private Vector2 moveInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        InputAction moveAction = playerInput.actions["Move"];

        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveInput = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //if (rb != null)
        //{
        //    Debug.Log("Current Velocity: " + rb.linearVelocity + " Magnitude: " + Mathf.Sqrt(Mathf.Pow(rb.linearVelocity.x, 2) + Mathf.Pow(rb.linearVelocity.y, 2) + Mathf.Pow(rb.linearVelocity.z, 2)) * 3.6);
        //}
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector3(moveInput.x, 0f, moveInput.y) * force);
    }
}
