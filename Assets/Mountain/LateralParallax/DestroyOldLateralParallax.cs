using UnityEngine;

public class DestroyOldLateralParallax : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LateralParallaxSet"))
        {
            Destroy(other.gameObject);
        }
    }
}
