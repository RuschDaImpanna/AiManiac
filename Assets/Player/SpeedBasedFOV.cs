using Unity.Cinemachine;
using UnityEngine;

public class SpeedBasedFov : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private CinemachineCamera cinemachineCamera;

    [Header("Speed Settings")]
    [SerializeField] private float minSpeed = 100f;
    [SerializeField] private float maxSpeed = 200f;

    [Header("FOV Settings")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float speedFOV = 80f;
    [SerializeField] private float smoothSpeed = 5f;

    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float currentSpeed = rb.linearVelocity.magnitude * 3.6f;
        float speedPercent = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);
        float targetFOV = Mathf.Lerp(normalFOV, speedFOV, speedPercent);

        cinemachineCamera.Lens.FieldOfView = Mathf.Lerp(
            cinemachineCamera.Lens.FieldOfView,
            targetFOV,
            Time.deltaTime * smoothSpeed
        );
    }
}
