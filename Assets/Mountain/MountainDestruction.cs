using UnityEngine;

public class MountainDestruction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision detected with " + other.gameObject.name);
        if (other.gameObject.CompareTag("Zone_MountainDestructor"))
        {
            Debug.Log("Mountain section destroyed: " + gameObject.name);
            Destroy(gameObject);
        }
    }
}
