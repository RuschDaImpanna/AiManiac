using UnityEngine;
using System;

public class MountainGeneration : MonoBehaviour
{

    public GameObject nextMountainSection;
    public Boolean needDuplicate = false;
    private GameObject duplicateCurrentMountainSection;
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
            int customSeed = UnityEngine.Random.Range(1, 10000);

            Vector3 newSectionPos = nextMountainSection.transform.localPosition + CalculteRelativeNextSectionPosition();
            GameObject newSection = Instantiate(
                nextMountainSection, 
                newSectionPos, 
                nextMountainSection.transform.rotation, 
                nextMountainSection.transform.parent
            );
            newSection.GetComponent<ObstaclesGeneration>().seed = customSeed;

            if (needDuplicate)
            {
                Vector3 duplicateSectionPos = nextMountainSection.transform.localPosition - CalculteRelativeNextSectionPosition() * 2;
                GameObject duplicatedSection = Instantiate(
                    nextMountainSection, 
                    duplicateSectionPos, 
                    nextMountainSection.transform.rotation, 
                    nextMountainSection.transform.parent
                );
                duplicatedSection.GetComponent<ObstaclesGeneration>().seed = customSeed;

                duplicateCurrentMountainSection = duplicatedSection;
            }

            nextMountainSection = newSection;
        } else if (other.gameObject.CompareTag("Zone_LoopReset"))
        {
            //transform.position -= new Vector3(
            //    0f,
            //    -currentMountainSection.transform.localScale.z * Mathf.Sin(currentMountainSection.transform.rotation.eulerAngles.x * Mathf.PI / 180) * 3f,
            //    currentMountainSection.transform.localScale.z * Mathf.Cos(currentMountainSection.transform.rotation.eulerAngles.x * Mathf.PI / 180) * 3f
            //);
            transform.position -= CalculteRelativeNextSectionPosition() * 3f;

            nextMountainSection = duplicateCurrentMountainSection;
        }
    }

    public Vector3 CalculteRelativeNextSectionPosition()
    {
        return new Vector3(
            0,
            - nextMountainSection.transform.localScale.z * (float) Math.Sin(nextMountainSection.transform.rotation.eulerAngles.x * Math.PI / 180), 
            nextMountainSection.transform.localScale.z * (float) Math.Cos(nextMountainSection.transform.rotation.eulerAngles.x * Math.PI / 180)
        );
    }

    public void SetCurrentMountainSection(GameObject currentMountainSection)
    {
        this.nextMountainSection = currentMountainSection;
    }
}
