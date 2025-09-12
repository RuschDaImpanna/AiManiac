using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float force = 50f;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private Vector2 moveInput;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        InputAction moveAction = playerInput.actions["Move"];

        moveAction.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => moveInput = Vector2.zero;
    }

    void FixedUpdate()
    {

        rb.AddForce(new Vector3(moveInput.x, 0f, moveInput.y) * force);

    }
}
