using UnityEngine;
using UnityEngine.InputSystem;

public class RecoilWeapon : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    public Transform playerCamera;
    public float recoilForce = 1000f;
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

    // Called when left mouse button is clicked
    void OnShootRecoilWeapon()
    {
        //Debug.Log("Horizontal recoil camera: " + -playerCamera.forward + " Vertical recoil camera: " + playerCamera.up);
        Vector3 recoilDirection = -playerCamera.forward;
        playerRigidbody.AddForce(recoilDirection * recoilForce, ForceMode.Impulse);
    }
}
