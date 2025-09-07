using UnityEngine;
using System.Collections.Generic;

public class ObstaclesGeneration : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public int seed;
    private List<(Vector3 position, Vector3 size)> existingObstacles = new List<(Vector3 position, Vector3 size)>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Generate grid positions
        List<List<Vector3>> gridPositions = new List<List<Vector3>>();
        List<List<bool>> availablePositions = new List<List<bool>>();

        Vector3 gridScale = Vector3.one;
        float gapX = 0;
        float gapZ = 0;

        for (float x = -(transform.localScale.x) / 2f; (x + gridScale.x) <= transform.localScale.x / 2f; x += gridScale.x + gapX)
        {
            List<Vector3> list = new List<Vector3>();
            List<bool> availList = new List<bool>();

            for (float z = -(transform.localScale.z) / 2f; (z + gridScale.z) <= transform.localScale.z / 2f; z += gridScale.z + gapZ)
            {
                list.Add(new Vector3((x + gridScale.x / 2f) / transform.localScale.x, 0.5f, (z + gridScale.z / 2f) / transform.localScale.z));
                
                availList.Add(true);
            }

            gridPositions.Add(list);
            availablePositions.Add(availList);
        }

        // Set random seed
        seed = seed == 0 ? Random.Range(1, 10000) : seed;
        Random.State originalState = Random.state;
        Random.InitState(seed);

        // Instantiate cubes at random positions
        Vector3 obstacleRelativeScale = GetRelativeSize(obstaclePrefab.transform.localScale);

        foreach (List<Vector3> list in gridPositions)
        {
            foreach (Vector3 pos in list)
            {
                if (Random.Range(0, 1000) < 5f) // 0.5% chance to spawn an obstacle
                {
                    if (CheckPosAvailability(obstaclePrefab, pos)) {
                        GameObject newObstacle = Instantiate(obstaclePrefab);
                        newObstacle.transform.parent = transform;
                        newObstacle.transform.localPosition = pos + new Vector3(0, obstacleRelativeScale.y / 2f, 0);
                        newObstacle.transform.localRotation = Quaternion.identity;

                        existingObstacles.Add((pos, obstacleRelativeScale));
                    }
                }
            }
        }

        // Restore original random state
        Random.state = originalState;
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

}
