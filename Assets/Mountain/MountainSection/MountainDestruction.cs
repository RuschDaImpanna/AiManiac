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
        if (other.gameObject.CompareTag("Zone_MountainDestructor"))
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
