using UnityEngine;

public class DeathSeaGameOver : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered Death Sea, triggering game over sequence.");
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
        }
    }
}
