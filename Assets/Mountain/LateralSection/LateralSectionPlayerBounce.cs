using UnityEngine;
using Unity.Cinemachine;

public class LateralSectionPlayerBounce : MonoBehaviour
{
    [Header("Cinemachine Impulse Settings")]
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float impulseScale = 1f;

    [Header("Bounce Settings")]
    [SerializeField] private float bounceForce;
    [SerializeField, Range(0f, 1f)] private float speedReduction = 2f / 3f;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit lateral section, applying bounce effect.");

            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                impulseSource.GenerateImpulse(playerRigidbody.linearVelocity * impulseScale);

                float direction = Mathf.Sign(playerRigidbody.linearVelocity.x);

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
                // Invert the player's horizontal velocity to create a bounce effect
                //playerRigidbody.linearVelocity = new Vector3(
                //    -playerRigidbody.linearVelocity.x,
                //    playerRigidbody.linearVelocity.y * speedReduction, 
                //    playerRigidbody.linearVelocity.z * speedReduction
                //);

                //playerRigidbody.AddForce(new Vector3(playerRigidbody.linearVelocity.x * bounceForce, 0, 0), ForceMode.Impulse);
            }
        }
    }
}
