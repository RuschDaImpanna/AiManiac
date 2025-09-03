using UnityEngine;
using System;

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
            float currentZPosition = mountainSection.transform.localPosition.z;
            float currentYPosition = mountainSection.transform.localPosition.y;

            float newZPosition = currentZPosition + (float) (mountainSection.transform.localScale.z * Math.Cos(mountainSection.transform.rotation.eulerAngles.x * (Math.PI / 180)));
            float newYPosition = currentYPosition - (float) (mountainSection.transform.localScale.z * Math.Sin(mountainSection.transform.rotation.eulerAngles.x * (Math.PI / 180)));

            GameObject newSection = Instantiate(mountainSection, new Vector3(mountainSection.transform.localPosition.x, newYPosition, newZPosition), mountainSection.transform.rotation);

            mountainSection = newSection;
        }
    }
}
