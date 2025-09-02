using UnityEngine;

public class SectionTrigger : MonoBehaviour
{

    public GameObject mountainSection;
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
        if (other.gameObject.CompareTag("Zone_MountainGenerator"))
        {
            Instantiate(mountainSection, new Vector3(0, 0, 30), Quaternion.identity);
        }
    }
}
