using UnityEngine;

public class PlayerBounce : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float bounceForce = 1f;

    [SerializeField, Range(0f, 1f)]
    private float speedReduction = 2/3;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
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
