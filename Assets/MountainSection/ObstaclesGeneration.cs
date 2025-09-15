using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ObstaclesGeneration : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public int seed;
    public bool isInitialized = false;
    private List<(Vector3 position, Vector3 size)> existingObstacles = new List<(Vector3 position, Vector3 size)>();
    private static List<List<Vector3>> gridPositions = new List<List<Vector3>>();
    private Vector3 gridScale = new Vector3(3f, 3f, 3f);
    private Vector3 relativeGridScale;

    public IEnumerator Initialize(int customSeed = 0, bool deleteExistingObstacles = true)
    {
        isInitialized = false;

        if (customSeed != 0)
        {
            seed = customSeed;
        }

        // Generate grid positions
        if (gridPositions.Count == 0)
        {
            GenerateGridPositions(gridScale, 0f, 0f);
            relativeGridScale = GetRelativeSize(gridScale); // Calculate relative grid scale based on parent scale
        }

        if (deleteExistingObstacles)
        {
            // Delete existing obstacles
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (child.CompareTag("Obstacle"))
                {
                    Destroy(child.gameObject);
                }
            }

            yield return null;
        }

        // Generate obstacles
        if (GetChildrenObstaclesCount() == 0)
        {
            GenerateObstacles();
        }

        yield return null;

        isInitialized = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!isInitialized)
        {
            StartCoroutine(Initialize());
        }
    }

    Vector3 GetRelativeSize(Vector3 size)
    {
        return new Vector3(size.x / transform.localScale.x, size.y / transform.localScale.y, size.z / transform.localScale.z);
    }

    bool CheckPosAvailability(GameObject prefab, Vector3 localPos)
    {
        // Check if the prefab fits within the bounds of the parent object at the given local position
        Vector3 prefabRelativeSize = GetRelativeSize(prefab.transform.localScale);

        if (prefabRelativeSize.x > 1f || prefabRelativeSize.z > 1f)
        {
            return false;
        }

        // Check if the prefab would exceed the bounds of the parent object
        if (localPos.x - prefabRelativeSize.x / 2f < -0.5f || localPos.x + prefabRelativeSize.x / 2f > 0.5f ||
            localPos.z - prefabRelativeSize.z / 2f < -0.5f || localPos.z + prefabRelativeSize.z / 2f > 0.5f)
        {
            return false;
        }

        // Check for overlaps with existing obstacles
        foreach (var obstacle in existingObstacles)
        {
            if (CheckOverlap(localPos, prefabRelativeSize, obstacle.position, obstacle.size))
            {
                return false; // Overlap detected
            }
        }

        return true;
    }

    private bool CheckOverlap(Vector3 pos1, Vector3 size1, Vector3 pos2, Vector3 size2)
    {
        bool overlapX = pos1.x - size1.x / 2f < pos2.x + size2.x / 2f && pos1.x + size1.x / 2f > pos2.x - size2.x / 2f;
        bool overlapZ = pos1.z - size1.z / 2f < pos2.z + size2.z / 2f && pos1.z + size1.z / 2f > pos2.z - size2.z / 2f;

        return overlapX && overlapZ;
    }

    private void GenerateObstacles()
    {
        // Set random seed
        seed = seed == 0 ? Random.Range(1, 10000) : seed;
        Random.State originalState = Random.state;
        Random.InitState(seed);

        // Instantiate obstacles at random positions
        Vector3 obstacleRelativeScale = GetRelativeSize(obstaclePrefab.transform.localScale);

        var obstacleSettings = obstaclePrefab.GetComponent<ObstacleSettings>();
        int marginX = obstacleSettings != null ? obstacleSettings.marginX : 0;
        int marginZ = obstacleSettings != null ? obstacleSettings.marginZ : 0;

        foreach (List<Vector3> list in gridPositions)
        {
            foreach (Vector3 pos in list)
            {
                if (Random.Range(0, 1000) < 5f) // 0.5% chance to spawn an obstacle
                {
                    if (CheckPosAvailability(obstaclePrefab, pos))
                    {
                        // Instantiate the obstacle
                        GameObject newObstacle = Instantiate(obstaclePrefab);
                        newObstacle.transform.parent = transform;
                        newObstacle.transform.localPosition = pos + new Vector3(0, obstacleRelativeScale.y / 2f, 0);
                        newObstacle.transform.localRotation = Quaternion.identity;
                        newObstacle.transform.localScale = obstacleRelativeScale;

                        // Add margin to the occupied space
                        Vector3 occupedSpace = new Vector3(
                            obstacleRelativeScale.x + (marginX *  relativeGridScale.x), 
                            obstacleRelativeScale.y, 
                            obstacleRelativeScale.z + (marginZ * relativeGridScale.z)
                        );

                        // Store the position and size of the new obstacle
                        existingObstacles.Add((pos, occupedSpace));
                    }
                }
            }
        }

        // Restore original random state
        Random.state = originalState;
    }

    private int GetChildrenObstaclesCount()
    {
        int count = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag("Obstacle"))
            {
                count++;
            }
        }
        return count;
    }

    private void GenerateGridPositions(Vector3 gridScale, float gapX = 0, float gapZ = 0)
    {
        for (float x = -(transform.localScale.x) / 2f; (x + gridScale.x) <= transform.localScale.x / 2f; x += gridScale.x + gapX)
        {
            List<Vector3> list = new List<Vector3>();

            for (float z = -(transform.localScale.z) / 2f; (z + gridScale.z) <= transform.localScale.z / 2f; z += gridScale.z + gapZ)
            {
                list.Add(new Vector3((x + gridScale.x / 2f) / transform.localScale.x, 0.5f, (z + gridScale.z / 2f) / transform.localScale.z));

            }

            gridPositions.Add(list);
        }
    }
}
