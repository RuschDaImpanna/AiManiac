using UnityEngine;

public class AvalancheMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject prefabMountainSection;
    [SerializeField] private GameObject mountain;

    void Update()
    {
        transform.position = player.transform.position + new Vector3(
                0f,
                prefabMountainSection.transform.localScale.z * Mathf.Sin(mountain.transform.rotation.eulerAngles.x * Mathf.PI / 180) * 2.5f,
                -prefabMountainSection.transform.localScale.z * Mathf.Cos(mountain.transform.rotation.eulerAngles.x * Mathf.PI / 180) * 2.5f
            );
    }
}
