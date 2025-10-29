using Unity.Cinemachine;
using UnityEngine;

public class ObstaclePlayerBounce : MonoBehaviour
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
            Debug.Log("Player hit obstacle, applying bounce effect.");

            impulseSource.GenerateImpulse();

            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                int direction = (other.transform.position.x > transform.position.x) ? 1 : -1;

                playerRigidbody.linearVelocity = new Vector3(
                    playerRigidbody.linearVelocity.x,
                    playerRigidbody.linearVelocity.y * speedReduction,
                    playerRigidbody.linearVelocity.z * speedReduction
                );

                playerRigidbody.AddForce(new Vector3(playerRigidbody.linearVelocity.magnitude * bounceForce * direction, 0, 0), ForceMode.Impulse);
            }
        }
    }
}