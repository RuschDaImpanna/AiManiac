using Unity.Cinemachine;
using UnityEngine;

public class ObstaclePlayerBounce : MonoBehaviour
{
    [Header("Cinemachine Impulse Settings")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float impulseScale = 1f;

    [Header("Bounce Settings")]
    [SerializeField] private float bounceForce;
    [SerializeField, Range(0f, 1f)] private float speedReduction = 2f / 3f;

    private void Start()
    {
        if (impulseSource == null)
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }

    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit obstacle, applying bounce effect.");

            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();

            if (playerRigidbody != null)
            {
                impulseSource.GenerateImpulse(playerRigidbody.linearVelocity * impulseScale);

                int direction = (other.transform.position.x > transform.position.x) ? -1 : 1;

                WeaponRecoil weaponRecoil = other.GetComponent<WeaponRecoil>();
                SpeedBar speedBar = other.GetComponent<SpeedBar>();
                float speed = speedBar.Speed * 3.6f;
                float additionalBounceSpeedScale = speed / 300f;

                weaponRecoil?.UpdateLateralSpeed(
                    - direction * weaponRecoil.RecoilSpeed * additionalBounceSpeedScale * 0.75f, 
                    direction
                );

                weaponRecoil?.UpdateForwardSpeed(
                    playerRigidbody.linearVelocity.y * speedReduction,
                    playerRigidbody.linearVelocity.z * speedReduction
                );

                // Old way to apply recoil
                //playerRigidbody.linearVelocity = new Vector3(
                //    playerRigidbody.linearVelocity.x,
                //    playerRigidbody.linearVelocity.y * speedReduction,
                //    playerRigidbody.linearVelocity.z * speedReduction
                //);

                //playerRigidbody.AddForce(new Vector3(playerRigidbody.linearVelocity.magnitude * bounceForce * direction, 0, 0), ForceMode.Impulse);
            }
        }
    }
}