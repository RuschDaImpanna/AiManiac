using UnityEngine;

public class DangerOMeter : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 previousVelocity;
    private float currentSpeed;
    private Vector3 acceleration;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        Vector3 currentVelocity = rb.linearVelocity;
        acceleration = (currentVelocity - previousVelocity) / Time.fixedDeltaTime;
        currentSpeed = currentVelocity.magnitude;
        previousVelocity = currentVelocity;

        Debug.Log($"Player Speed: {currentSpeed:0.00} | Accel: {acceleration.magnitude:0.00}");

    }
}

