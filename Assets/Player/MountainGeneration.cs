using System;
using System.Collections;
using UnityEngine;

public class MountainGeneration : MonoBehaviour
{

    public GameObject lastMountainSectionGenerated;
    private GameObject duplicatedLastMountainSectionGenerated;
    private int sectionsGenerated = 0;

    public Boolean needDuplicate = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Zone_MountainGenerator"))
        {
            int customSeed = UnityEngine.Random.Range(1, 10000);

            // Create new mountain section
            GameObject newSection = Instantiate(lastMountainSectionGenerated);
            newSection.transform.parent = lastMountainSectionGenerated.transform.parent;
            newSection.transform.localPosition = lastMountainSectionGenerated.transform.localPosition + new Vector3(0f, 0f, lastMountainSectionGenerated.transform.localScale.z);
            newSection.transform.rotation = lastMountainSectionGenerated.transform.rotation;
            newSection.name = "MountainSection_" + (++sectionsGenerated);

            // Initialize obstacles generation for the new section (if enabled)
            ObstaclesGeneration lastMountainObstaclesGeneration = lastMountainSectionGenerated.GetComponent<ObstaclesGeneration>();
            ObstaclesGeneration obstaclesGen = newSection.GetComponent<ObstaclesGeneration>();
            
            if (lastMountainObstaclesGeneration.enabled == false)
            {
                obstaclesGen.enabled = false;
            } else
            {
                obstaclesGen.StartCoroutine(
                    obstaclesGen.Initialize(
                        deleteExistingObstacles: true,
                        customSeed: customSeed
                    )
                );

                yield return new WaitUntil(() => obstaclesGen.isInitialized);
            }

            // Duplicate last section for loop reset
            if (needDuplicate)
            {
                GameObject duplicatedSection = Instantiate(lastMountainSectionGenerated);
                duplicatedSection.transform.parent = lastMountainSectionGenerated.transform.parent;
                duplicatedSection.transform.localPosition = lastMountainSectionGenerated.transform.localPosition - new Vector3(0f, 0f, lastMountainSectionGenerated.transform.localScale.z * 2);
                duplicatedSection.transform.rotation = lastMountainSectionGenerated.transform.rotation;
                duplicatedSection.name = "MountainSection_Duplicate_" + sectionsGenerated;

                duplicatedLastMountainSectionGenerated = duplicatedSection;
            }

            lastMountainSectionGenerated = newSection;
        } else if (other.gameObject.CompareTag("Zone_LoopReset"))
        {
            // Reset position to duplicated section
            transform.position -= CalculteRelativeNextSectionPosition() * 3f;
            lastMountainSectionGenerated = duplicatedLastMountainSectionGenerated;
        }
    }

    public Vector3 CalculteRelativeNextSectionPosition()
    {
        Vector3 relativeNextSectionPosition = new Vector3(
            0,
            -lastMountainSectionGenerated.transform.localScale.z * (float)Math.Sin(lastMountainSectionGenerated.transform.rotation.eulerAngles.x * Math.PI / 180),
            lastMountainSectionGenerated.transform.localScale.z * (float)Math.Cos(lastMountainSectionGenerated.transform.rotation.eulerAngles.x * Math.PI / 180)
        );

        return relativeNextSectionPosition;
    }

    public void SetCurrentMountainSection(GameObject currentMountainSection)
    {
        this.lastMountainSectionGenerated = currentMountainSection;
    }
}
