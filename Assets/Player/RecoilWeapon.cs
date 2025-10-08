using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RecoilWeapon : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    public Transform playerCamera;
    public float recoilForce = 1000f;
    public float cooldown = 1.5f;
    private float currentCooldown = 0f;
    public float CurrentCooldown { get { return currentCooldown; } }
    private float lastShootTime = -Mathf.Infinity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRigidbody = GetComponentInParent<Rigidbody>();
        InputAction shootAction = GetComponentInParent<PlayerInput>().actions["Shoot"];

        shootAction.performed += ctx => 
        {
            if (ctx.control.path.Contains("leftButton"))
            {
                OnShootRecoilWeapon();
            }
            else if (ctx.control.path.Contains("rightButton"))
            {
            }

        };
    }

    // Update is called once per frame
    void Update()
    {
        
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
    void OnShootRecoilWeapon()
    {   
        if (currentCooldown != 0f) return;

        Vector3 vector3 = playerCamera.forward;
        Vector3 recoilDirection = - new Vector3(vector3.x, 3*vector3.y/4, vector3.z/10);
        playerRigidbody.AddForce(recoilDirection * recoilForce, ForceMode.Impulse);
        lastShootTime = Time.time;
    }
}
