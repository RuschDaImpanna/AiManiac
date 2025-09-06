using UnityEngine;

public class DangerOMeter : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 previousVelocity;
    private float currentSpeed;
    private Vector3 acceleration;

    [Header("Avalanche Settings")]
    public float avalancheStart;
    public float avalancheAcceleration;

    public float avalancheSpeed;
    private float dangerMeter;

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

        if (avalancheStart > 0f)
        {
            avalancheStart -= Time.deltaTime;
        }
        else
        {
            // Smoothly approach target avalanche speed
            avalancheSpeed = Mathf.MoveTowards(avalancheSpeed, currentSpeed, avalancheAcceleration * Time.deltaTime);


            float relativeSpeed = currentSpeed - avalancheSpeed;

            // Update danger meter based on relative speed
            dangerMeter += relativeSpeed * Time.deltaTime * 0.75f;
            //dangerMeter = Mathf.Clamp(dangerMeter, 0f, 10f);

            Debug.Log($"Avalanche Speed: {avalancheSpeed:0.00} | Danger Meter: {dangerMeter:0.00}");


        }
    }
}

