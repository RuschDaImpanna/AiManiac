using UnityEngine;

public class PlayerBounce : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                // Invert the player's horizontal velocity to create a bounce effect
                playerRigidbody.linearVelocity = new Vector3(-playerRigidbody.linearVelocity.x, playerRigidbody.linearVelocity.y, playerRigidbody.linearVelocity.z);
            }
        }
    }
}
