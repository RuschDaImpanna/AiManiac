using UnityEngine;

public class FrontalParallaxMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;

    private float initialyposition;
    private float initialzposition;

    private void Start()
    {
        initialyposition = transform.position.y;
        initialzposition = transform.position.z;

    }

    void Update()
    {
        transform.position = player.transform.position + new Vector3(
            0f,
            initialyposition,
            initialzposition
        );
    }
}
