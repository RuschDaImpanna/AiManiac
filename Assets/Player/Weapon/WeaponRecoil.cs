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

    public float Cooldown
    {
        get { return cooldown; }
    }

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
    public float CurrentCooldown { get { return currentCooldown; } }

    private float desaccelerationRate = 0f;
    private float targetLateralSpeed = 0f;

    private Quaternion mountainSectionRotation;
    private float yComponentScale;
    private float zComponentScale;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction shootAction;

    void Start()
    {
        InitializeWeapon();
    }

    void OnEnable()
    {
        // Re-subscribe when the object is enabled
        SubscribeToInput();
    }

    void OnDisable()
    {
        // Unsubscribe to prevent errors
        UnsubscribeFromInput();
    }

    void OnDestroy()
    {
        UnsubscribeFromInput();
    }

    private void InitializeWeapon()
    {
        rb = GetComponentInParent<Rigidbody>();

        if (impulseSource == null)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        // Subscribe to input
        SubscribeToInput();

        // Calculate mountain section scales
        if (prefabMountainSection != null)
        {
            mountainSectionRotation = prefabMountainSection.rotation;
            yComponentScale = Mathf.Sin(mountainSectionRotation.eulerAngles.x * Mathf.Deg2Rad) * additionalForwardScale;
            zComponentScale = Mathf.Cos(mountainSectionRotation.eulerAngles.x * Mathf.Deg2Rad) * additionalForwardScale;
        }
    }

    private void SubscribeToInput()
    {
        // Unsubscribe first to avoid duplicate subscriptions
        UnsubscribeFromInput();

        playerInput = GetComponentInParent<PlayerInput>();
        if (playerInput != null)
        {
            shootAction = playerInput.actions["Shoot"];
            if (shootAction != null)
            {
                shootAction.performed += OnShootInput;
            }
        }
    }

    private void UnsubscribeFromInput()
    {
        if (shootAction != null)
        {
            shootAction.performed -= OnShootInput;
        }
    }

    private void OnShootInput(InputAction.CallbackContext ctx)
    {
        if (ctx.control.path.Contains("leftButton"))
        {
            OnShootWeaponRecoil();
        }
        else if (ctx.control.path.Contains("rightButton"))
        {
            // Right click logic here
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        float currentX = rb.linearVelocity.x;

        if (Mathf.Abs(currentX) > 0.01f)
        {
            float deceleration = desaccelerationRate * Time.fixedDeltaTime;
            float newX = Mathf.MoveTowards(currentX, targetLateralSpeed, deceleration);
            UpdateLateralSpeed(newX, Mathf.Sign(newX), false);
        }

        // Apply desacceleration to the player rigidbody in y axis
        if (!IsGrounded())
        {
            UpdateVerticalSpeed(
                rb.linearVelocity.y - (Mathf.Abs(desaccelerationRate) / 50f)
            );
        }

        // Update cooldown
        if (Time.time - lastShootTime < cooldown)
        {
            currentCooldown = cooldown - (Time.time - lastShootTime);
        }
        else
        {
            currentCooldown = 0f;
        }
    }

    void OnShootWeaponRecoil()
    {
        if (Time.timeScale == 0f) return;
        if (currentCooldown > 0f) return;

        // Check for null references
        if (rb == null || playerCameraHolderTransform == null || playerCameraTransform == null)
        {
            Debug.LogWarning("Cannot shoot - missing references");
            return;
        }

        // Update last shoot time
        lastShootTime = Time.time;

        // Apply physical recoil
        Quaternion holderDifference = Quaternion.Inverse(rb.transform.rotation) * playerCameraHolderTransform.localRotation;
        float yawAngleDifference = Mathf.DeltaAngle(0, holderDifference.eulerAngles.y);
        float recoilLateralSpeedScale = Mathf.Abs(yawAngleDifference) / 90f;
        recoilLateralSpeedScale = recoilLateralSpeedScale > 1f ? 1f : recoilLateralSpeedScale;

        float shootDirection = Mathf.Sign(yawAngleDifference);
        float lateralRecoilSpeed = recoilSpeed * recoilLateralSpeedScale;

        //Debug.Log($"Shoot - Yaw Angle: {yawAngleDifference:F1}°, Direction: {shootDirection}, Lateral Speed: {lateralRecoilSpeed:F2}");

        UpdateLateralSpeed(
            -shootDirection * lateralRecoilSpeed,
            shootDirection,
            true
        );

        if (Mathf.Abs(yawAngleDifference) > 90f)
        {
            float recoilForwardSpeedScale = (Mathf.Abs(yawAngleDifference) - 90f) / 90f;
            UpdateForwardSpeed(
                rb.linearVelocity.y - recoilSpeed * recoilForwardSpeedScale * yComponentScale,
                rb.linearVelocity.z + recoilSpeed * recoilForwardSpeedScale * zComponentScale
            );
        }

        // Vertical recoil only if looking downwards enough
        Quaternion cameraDifference = Quaternion.Inverse(playerCameraHolderTransform.localRotation) * playerCameraTransform.localRotation;
        float pitchAngleDiffence = Mathf.DeltaAngle(0, cameraDifference.eulerAngles.x);

        if (pitchAngleDiffence > minimunRecoilVerticalAngle)
        {
            float recoilVerticalSpeedScale = Mathf.Abs(pitchAngleDiffence) / 90f;
            UpdateVerticalSpeed(
                rb.linearVelocity.y + recoilSpeed * recoilVerticalSpeedScale * additionalVerticalScale
            );
        }

        // Apply camera recoil
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse(recoilDirection.normalized * recoilForce);
        }

        if (cameraWeaponRecoil != null)
        {
            cameraWeaponRecoil.ApplyRecoil();
        }

        // Trigger shoot sound 
        if (shootSound != null)
        {
            shootSound.Play();
        }

        // Spawn visual pulse
        if (pulsePrefab != null)
        {
            Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
            GameObject pulse = Instantiate(pulsePrefab, spawnPos, Quaternion.identity);

            if (pulse != null && rb != null)
            {
                Rigidbody pulseRb = pulse.GetComponent<Rigidbody>();
                if (pulseRb != null)
                {
                    pulseRb.linearVelocity = rb.linearVelocity;
                }
            }
        }
    }

    public void UpdateLateralSpeed(float speed, float direction = 1f, bool resetDeceleration = true)
    {
        if (rb == null) return;

        if (resetDeceleration)
        {
            desaccelerationRate = Mathf.Abs(speed) / desaccelerationTime;
            targetLateralSpeed = 0f;
        }

        rb.linearVelocity = new Vector3(
            speed,
            rb.linearVelocity.y,
            rb.linearVelocity.z
        );
    }

    public void UpdateForwardSpeed(float speedY, float speedZ)
    {
        if (rb == null) return;

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            speedY,
            speedZ
        );
    }

    public void UpdateVerticalSpeed(float speed)
    {
        if (rb == null) return;

        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            speed,
            rb.linearVelocity.z
        );
    }

    private bool IsGrounded()
    {
        if (rb == null) return false;
        return Physics.Raycast(rb.transform.position, -rb.transform.up, rb.transform.localScale.y / 2f + 1f, groundLayer);
    }
}