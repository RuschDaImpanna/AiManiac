using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponRecoil : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerCameraHolderTransform;
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] AudioSource shootSound;
    [SerializeField] private GameObject pulsePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform prefabMountainSection;

    [Header("Pulse Settings")]
    [SerializeField] private float pulseRadius = 10f;

    [Header("Shooting Settings")]
    [SerializeField] private float cooldown = 1.5f;
    public float CurrentCooldown { get { return currentCooldown; } }

    [Header("Camera Recoil Settings")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float recoilForce = 1f;
    [SerializeField] private Vector3 recoilDirection = new(0, 0.2f, -1f);
    [SerializeField] private CameraWeaponRecoil cameraWeaponRecoil;

    [Header("Physic Recoil Settings")]
    [SerializeField] private float recoilSpeed = 5f;
    [SerializeField] private float desaccelerationTime = 1f;
    [SerializeField] private float additionalForwardScale = 1f;
    [SerializeField] private float additionalVerticalScale = 1f;
    [SerializeField] private float minimunRecoilVerticalAngle = 10f;
    [SerializeField] private LayerMask groundLayer;

    public float RecoilSpeed { get { return recoilSpeed; } set { recoilSpeed = value; } }

    private float lastShootTime = -Mathf.Infinity;
    private float currentCooldown = 0f;

     private float desaccelerationRate = 0f; // Renombrado para claridad
    private float targetLateralSpeed = 0f;  // Nueva variable: velocidad objetivo

    private Quaternion mountainSectionRotation;
    private float yComponentScale;
    private float zComponentScale;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();

        if (impulseSource == null)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        InputAction shootAction = GetComponentInParent<PlayerInput>().actions["Shoot"];
        shootAction.performed += ctx =>
        {
            if (ctx.control.path.Contains("leftButton"))
            {
                OnShootWeaponRecoil();
            }
            else if (ctx.control.path.Contains("rightButton"))
            {
            }

        };

        mountainSectionRotation = prefabMountainSection.rotation;
        yComponentScale = Mathf.Sin(mountainSectionRotation.eulerAngles.x * Mathf.Deg2Rad) * additionalForwardScale;
        zComponentScale = Mathf.Cos(mountainSectionRotation.eulerAngles.x * Mathf.Deg2Rad) * additionalForwardScale;
    }

    void FixedUpdate()
    {
        float currentX = rb.linearVelocity.x;

        if (Mathf.Abs(currentX) > 0.01f)
        {
            float deceleration = desaccelerationRate * Time.fixedDeltaTime;

            float newX = Mathf.MoveTowards(currentX, targetLateralSpeed, deceleration);

            UpdateLateralSpeed(newX, Mathf.Sign(newX), false); // false = not reset desacceleration

            //Debug.Log($"Lateral Decel - Current X: {currentX:F2}, Target: {targetLateralSpeed:F2}, New X: {newX:F2}, Decel Rate: {desaccelerationRate:F2}");
        }

        // Apply desacceleration to the player rigidbody in y axis
        if (!IsGrounded())
        {
            //Debug.Log($"Current Y Speed: {rb.linearVelocity.y}");
            UpdateVerticalSpeed(
                rb.linearVelocity.y - (Mathf.Abs(desaccelerationRate) / 50f)
            );
        }

        // Update cooldown
        if (Time.time - lastShootTime < cooldown)
        {
            currentCooldown = cooldown - (Time.time - lastShootTime);
        } else
        {
            currentCooldown = 0f;
        }
    }

    // Called when left mouse button is clicked
    void OnShootWeaponRecoil()
    {
        if (Time.timeScale == 0f) return;
        if (currentCooldown > 0f) return;

        // Update last shoot time
        lastShootTime = Time.time;

        // Apply physical recoil
        Quaternion holderDifference = Quaternion.Inverse(rb.transform.rotation) * playerCameraHolderTransform.localRotation;
        float yawAngleDifference = Mathf.DeltaAngle(0, holderDifference.eulerAngles.y);
        float recoilLateralSpeedScale = Mathf.Abs(yawAngleDifference) / 90f; // Scale recoil based on how much the player is looking to the sides
        recoilLateralSpeedScale = recoilLateralSpeedScale > 1f ? 1f : recoilLateralSpeedScale; // Clamp to 1

        float shootDirection = Mathf.Sign(yawAngleDifference); // -1 o 1
        float lateralRecoilSpeed = recoilSpeed * recoilLateralSpeedScale;

        Debug.Log($"Shoot - Yaw Angle: {yawAngleDifference:F1}°, Direction: {shootDirection}, Lateral Speed: {lateralRecoilSpeed:F2}");

        UpdateLateralSpeed(
            -shootDirection * lateralRecoilSpeed,
            shootDirection,
            true
        );

        if (Mathf.Abs(yawAngleDifference) > 90f) {
            float recoilForwardSpeedScale = (Mathf.Abs(yawAngleDifference) - 90f) / 90f; // Scale forward recoil based on how much the player is looking backwards
            UpdateForwardSpeed(
                rb.linearVelocity.y - recoilSpeed * recoilForwardSpeedScale * yComponentScale,
                rb.linearVelocity.z + recoilSpeed * recoilForwardSpeedScale * zComponentScale
            );
        }

        // Vertical recoil only if looking downwards enough
        Quaternion cameraDifference = Quaternion.Inverse(playerCameraHolderTransform.localRotation) * playerCameraTransform.localRotation;
        float pitchAngleDiffence = Mathf.DeltaAngle(0, cameraDifference.eulerAngles.x);
        //Debug.Log($"Pitch Angle Difference: {cameraDifference.eulerAngles.x}; {pitchAngleDiffence}");
        //Debug.Log($"playerCameraHolderTransform.localRotation {playerCameraHolderTransform.localRotation}; playerCameraTransform.localRotation {playerCameraTransform.localRotation.eulerAngles}");

        if (pitchAngleDiffence > minimunRecoilVerticalAngle)
        {
            float recoilVerticalSpeedScale = Mathf.Abs(pitchAngleDiffence) / 90f; // Scale vertical recoil based on how much the player is looking downwards
            UpdateVerticalSpeed(
                rb.linearVelocity.y + recoilSpeed * recoilVerticalSpeedScale * additionalVerticalScale
            );

        }


        // Apply camera recoil
        impulseSource.GenerateImpulse(recoilDirection.normalized * recoilForce);
        cameraWeaponRecoil.ApplyRecoil();

        // Trigger shoot sound 
        shootSound.Play();

        // Spawn visual pulse
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject pulse = Instantiate(pulsePrefab, spawnPos, Quaternion.identity);
        pulse.GetComponent<Rigidbody>().linearVelocity = rb.linearVelocity; // Inherit player's velocity
    }

    public void UpdateLateralSpeed(float speed, float direction = 1f, bool resetDeceleration = true)
    {
        if (resetDeceleration)
        {
            desaccelerationRate = Mathf.Abs(speed) / desaccelerationTime;
            targetLateralSpeed = 0f; // Target: back to 0
        }

        rb.linearVelocity = new Vector3(
            speed,
            rb.linearVelocity.y,
            rb.linearVelocity.z
        );
    }

    public void UpdateForwardSpeed(float speedY, float speedZ)
    {
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            speedY,
            speedZ
        );
    }

    public void UpdateVerticalSpeed(float speed)
    {
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            speed,
            rb.linearVelocity.z
        );
    }

    private bool IsGrounded() // Auxiliar function to check if the player is grounded
    {
        // Lanzar rayo hacia abajo
        return Physics.Raycast(rb.transform.position, -rb.transform.up, rb.transform.localScale.y / 2f + 1f, groundLayer);
    }

    // Debugging: Dibujar rayo en la escena
    //private void OnDrawGizmos()
    //{
    //    // Color según si toca el suelo
    //    Gizmos.color = IsGrounded() ? Color.green : Color.red;

    //    // Dibujar línea del rayo
    //    Vector3 start = transform.position;
    //    Vector3 end = transform.position - rb.transform.up * (rb.transform.localScale.y / 2f + 1f);
    //    Gizmos.DrawLine(start, end);

    //    // Esfera al final (opcional)
    //    Gizmos.DrawSphere(end, 0.05f);
    //}
}
