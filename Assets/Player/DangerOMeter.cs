using UnityEngine;

public class DangerOMeter : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 previousVelocity;
    private Vector3 acceleration;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        Vector3 currentVelocity = rb.linearVelocity;
        acceleration = (currentVelocity - previousVelocity) / Time.fixedDeltaTime;

        Debug.Log($"Velocity: {currentVelocity} (Magnitude: {currentVelocity.magnitude}) | " + $"Acceleration: {acceleration} (Magnitude: {acceleration.magnitude})");

        previousVelocity = currentVelocity;
        
    }
}
