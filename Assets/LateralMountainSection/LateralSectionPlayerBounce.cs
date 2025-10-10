using UnityEngine;
using Unity.Cinemachine;

public class LateralSectionPlayerBounce : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float bounceForce = 1f;

    [SerializeField, Range(0f, 1f)]
    private float speedReduction = 2f / 3f;

    [SerializeField]
    private CinemachineImpulseSource impulseSource;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                impulseSource.GenerateImpulse(1.5f);
                
                // Invert the player's horizontal velocity to create a bounce effect
                playerRigidbody.linearVelocity = new Vector3(
                    -playerRigidbody.linearVelocity.x * bounceForce, 
                    playerRigidbody.linearVelocity.y, 
                    playerRigidbody.linearVelocity.z * speedReduction
                );
            }
        }
    }
}
