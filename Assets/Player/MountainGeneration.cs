using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Collections;
using Unity.Mathematics;

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
            Vector3 newSectionPos = lastMountainSectionGenerated.transform.position + CalculteRelativeNextSectionPosition();
            GameObject newSection = Instantiate(
                lastMountainSectionGenerated, 
                newSectionPos, 
                lastMountainSectionGenerated.transform.rotation, 
                lastMountainSectionGenerated.transform.parent
            );
            newSection.name = "MountainSection_" + (++sectionsGenerated);

            var obstaclesGen = newSection.GetComponent<ObstaclesGeneration>();
            obstaclesGen.StartCoroutine(
                obstaclesGen.Initialize(
                    deleteExistingObstacles: true, 
                    customSeed: customSeed
                )
            );

            yield return new WaitUntil(() => obstaclesGen.isInitialized);

            // Duplicate last section for loop reset
            if (needDuplicate)
            {
                Vector3 duplicateSectionPos = lastMountainSectionGenerated.transform.position - CalculteRelativeNextSectionPosition() * 2;
                GameObject duplicatedSection = Instantiate(
                    newSection, 
                    duplicateSectionPos, 
                    lastMountainSectionGenerated.transform.rotation, 
                    lastMountainSectionGenerated.transform.parent
                );
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
        return new Vector3(
            0,
            - lastMountainSectionGenerated.transform.localScale.z * (float) Math.Sin(lastMountainSectionGenerated.transform.rotation.eulerAngles.x * Math.PI / 180), 
            lastMountainSectionGenerated.transform.localScale.z * (float) Math.Cos(lastMountainSectionGenerated.transform.rotation.eulerAngles.x * Math.PI / 180)
        );
    }

    public void SetCurrentMountainSection(GameObject currentMountainSection)
    {
        this.lastMountainSectionGenerated = currentMountainSection;
    }
}
