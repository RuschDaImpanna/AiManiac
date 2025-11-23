using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class WeaponRecoil : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] AudioSource shootSound;

    [Header("Pulse Prefab")]
    [SerializeField] private GameObject pulsePrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Pulse Settings")]
    [SerializeField] private float pulseRadius = 10f;

    [Header("Shooting")]
    [SerializeField] private float cooldown = 1.5f;
    public float CurrentCooldown { get { return currentCooldown; } }
    private float lastShootTime = -Mathf.Infinity;
    private float currentCooldown = 0f;

    [Header("Recoil")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform playerCameraHolder;
    [SerializeField] private float repulseForce = 1000f;
    [SerializeField] private float recoilForce = 1f;
    [SerializeField] private Vector3 recoilDirection = new (0, 0.2f, -1f);
    [SerializeField] private CameraWeaponRecoil cameraWeaponRecoil;

    [Header("New Recoil Settings")]
    [SerializeField] private float recoilSpeed = 5f;
    [SerializeField] private float desaccelerationTime = 1f;

    private float desacceleration = 0f;
    private float currentShootDirection = 0f;
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
    }

    void FixedUpdate()
    {
        // Apply desacceleration to the player rigidbody in x axis
        if (-rb.linearVelocity.x * currentShootDirection > 0) {
            UpdateLateralSpeed(
                rb.linearVelocity.x + desacceleration / 50f
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

    // Called when left mouse button is clicked
    void OnShootWeaponRecoil()
    {   
        if (Time.timeScale == 0f) return;
        if (currentCooldown != 0f) return;

        // Apply physical recoil
        Quaternion diference = Quaternion.Inverse(rb.transform.rotation) * playerCameraHolder.localRotation;
        float yawAngleDiference = Mathf.DeltaAngle(0, diference.eulerAngles.y);
        float recoilSpeedScale = Mathf.Abs(yawAngleDiference) / 90f; // Scale recoil based on how much the player is looking to the sides

        currentShootDirection = yawAngleDiference >= 0 ? 1f : -1f;
        desacceleration = recoilSpeed * recoilSpeedScale * currentShootDirection / desaccelerationTime;

        UpdateLateralSpeed(
            - recoilSpeed * recoilSpeedScale,
            currentShootDirection
        );

        // Old recoil code
        //Vector3 vector3 = playerCamera.forward;
        //Vector3 recoilForceDirection = - new Vector3(vector3.x, 3*vector3.y/4, vector3.z/10);
        //rb.AddForce(recoilForceDirection * repulseForce, ForceMode.Impulse);
        //lastShootTime = Time.time;

        // Apply camera recoil
        //Debug.Log("Shooting weapon recoil");
        impulseSource.GenerateImpulse(recoilDirection.normalized * recoilForce);
        cameraWeaponRecoil.ApplyRecoil();

        // Trigger shoot sound 
        shootSound.Play();

        // Spawn visual pulse
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject pulse = Instantiate(pulsePrefab, spawnPos, Quaternion.identity);
        pulse.GetComponent<Rigidbody>().linearVelocity = rb.linearVelocity; // Inherit player's velocity
    }

    public void UpdateLateralSpeed(float speed, float direction = 1f)
    {
        rb.linearVelocity = new Vector3(
            speed * direction,
            rb.linearVelocity.y,
            rb.linearVelocity.z
        );
    }
}
