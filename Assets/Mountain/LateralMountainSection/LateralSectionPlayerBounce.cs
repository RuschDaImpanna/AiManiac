using UnityEngine;
using Unity.Cinemachine;

public class LateralSectionPlayerBounce : MonoBehaviour
{
    [SerializeField]
    private float bounceForce;

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
            Debug.Log("Player hit lateral section, applying bounce effect.");

            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                impulseSource.GenerateImpulse();
                
                // Invert the player's horizontal velocity to create a bounce effect
                playerRigidbody.linearVelocity = new Vector3(
                    -playerRigidbody.linearVelocity.x,
                    playerRigidbody.linearVelocity.y, 
                    playerRigidbody.linearVelocity.z * speedReduction
                );

                playerRigidbody.AddForce(new Vector3(playerRigidbody.linearVelocity.x * bounceForce, 0, 0), ForceMode.Impulse);
            }
        }
    }
}
