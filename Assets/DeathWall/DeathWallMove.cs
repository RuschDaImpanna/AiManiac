using UnityEngine;

public class DestroyWallMove : MonoBehaviour
{
    public GameObject player;
    public GameObject mountainSection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(0, 0, player.transform.position.z - 30);
    }
}
