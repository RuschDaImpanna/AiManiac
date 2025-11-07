using UnityEngine;

public class AmbienceSound : MonoBehaviour
{
    public Collider Area;
    public GameObject player; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Locate closest point on the collider to the player 
        Vector3 closestpoint = Area.ClosestPoint(player.transform.position);
        // Set position to closest point to the player
        transform.position = closestpoint;  
        
    }
}
