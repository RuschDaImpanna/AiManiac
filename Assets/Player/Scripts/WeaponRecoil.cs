using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponRecoil : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private float cooldown = 1.5f;
    public float CurrentCooldown { get { return currentCooldown; } }
    private float lastShootTime = -Mathf.Infinity;
    private float currentCooldown = 0f;

    [Header("Recoil")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float repulseForce = 1000f;
    [SerializeField] private float recoilForce = 1f;
    [SerializeField] private Vector3 recoilDirection = new Vector3(0, 0.2f, -1f);
    
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

        Vector3 vector3 = playerCamera.forward;
        Vector3 recoilForceDirection = - new Vector3(vector3.x, 3*vector3.y/4, vector3.z/10);
        rb.AddForce(recoilForceDirection * repulseForce, ForceMode.Impulse);
        lastShootTime = Time.time;

        // Apply camera recoil
        impulseSource.GenerateImpulseWithVelocity(recoilDirection.normalized * recoilForce);
    }
}
