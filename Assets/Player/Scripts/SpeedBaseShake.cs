using Unity.Cinemachine;
using UnityEngine;

public class SpeedBaseShake : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Header("Speed Settings")]
    [SerializeField] private float minSpeed = 100f;
    [SerializeField] private float maxSpeed = 200f;

    [Header("Shake Intensity")]
    [SerializeField] private float baseIntensity = 0.5f;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (impulseSource == null)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        rb = transform.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float currentSpeed = rb.linearVelocity.magnitude * 3.6f;

        if (currentSpeed >= minSpeed)
        {
            float speedPercent = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);
            float intensity = baseIntensity * speedPercent;

            // Generate continuous subtle impulses
            impulseSource.GenerateImpulse(intensity);
        }
    }
}
