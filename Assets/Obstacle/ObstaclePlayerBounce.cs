using UnityEngine;

public class ObstaclePlayerBounce : MonoBehaviour
{
    [SerializeField]
    private float bounceForce = 10f;

    [SerializeField, Range(0f, 1f)]
    private float speedReduction = 2f / 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit obstacle, applying bounce effect.");
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                int direction = (other.transform.position.x > transform.position.x) ? 1 : -1;

                // Invert the player's horizontal velocity to create a bounce effect
                playerRigidbody.linearVelocity = new Vector3(
                    playerRigidbody.linearVelocity.x,
                    playerRigidbody.linearVelocity.y,
                    playerRigidbody.linearVelocity.z * speedReduction
                );

                playerRigidbody.AddForce(new Vector3(playerRigidbody.linearVelocity.magnitude * 3.6f * bounceForce * direction, 0, 0), ForceMode.Impulse);
            }
        }
    }
}