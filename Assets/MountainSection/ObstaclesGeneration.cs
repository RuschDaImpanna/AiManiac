using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum ObstacleGenerationType
{
    Random,
    Lateral
}

public class ObstaclesGeneration : MonoBehaviour
{
    [Header("Database")]
    public ObstaclesDatabase obstaclesDatabase;

    [Header("Obstacle Settings")]
    public int seed;
    public bool isInitialized = false;
    public float obstacleSpawnChance = 0.005f; // 0.5% chance to spawn an obstacle at each grid position

    private List<(Vector3 position, Vector3 size)> existingObstacles = new List<(Vector3 position, Vector3 size)>();
    public List<List<Vector3>> gridPositions = new List<List<Vector3>>(); // TODO: make private and create getter
    private Vector3 gridScale = new Vector3(3f, 3f, 3f);
    private Vector3 relativeGridScale;

    public ObstacleGenerationType generationType = ObstacleGenerationType.Random;

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
            switch (generationType)
            {
                case ObstacleGenerationType.Random:
                    GenerateObstacles();
                    break;
                case ObstacleGenerationType.Lateral:
                    GenerateLateralObstacles();
                    break;
            }
        }

        yield return null;

        isInitialized = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (obstaclesDatabase == null)
        {
            Debug.LogError("Obstacle Database not assigned!");
            return;
        }

        if (!isInitialized)
        {
            StartCoroutine(Initialize());
        }
    }

    private Vector3 GetRelativeSize(Vector3 size)
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

    private bool CheckOverlap(Vector3 pos1, Vector3 size1, Vector3 pos2, Vector3 size2, float overlapTolerance = 0.01f)
    {
        float tolX = overlapTolerance / transform.localScale.x;
        float tolZ = overlapTolerance / transform.localScale.z;

        bool overlapX = pos1.x - size1.x / 2f + tolX < pos2.x + size2.x / 2f - tolX &&
                        pos1.x + size1.x / 2f - tolX > pos2.x - size2.x / 2f + tolX;
        bool overlapZ = pos1.z - size1.z / 2f + tolZ < pos2.z + size2.z / 2f - tolZ &&
                        pos1.z + size1.z / 2f - tolZ > pos2.z - size2.z / 2f + tolZ;

        return overlapX && overlapZ;
    }

    private void GenerateObstacles()
    {
        // Set random seed
        seed = seed == 0 ? Random.Range(1, 10000) : seed;
        Random.State originalState = Random.state;
        Random.InitState(seed);

        // Instantiate obstacles at random positions
        foreach (List<Vector3> list in gridPositions)
        {
            foreach (Vector3 pos in list)
            {
                if (Random.value < obstacleSpawnChance) // 0.5% chance to spawn an obstacle
                {
                    GameObject obstaclePrefab = obstaclesDatabase.GetWeightedRandomPrefab();
                    Vector3 obstacleRelativeScale = GetRelativeSize(obstaclePrefab.transform.localScale);
                    var obstacleSettings = obstaclePrefab.GetComponent<ObstacleSettings>();
                    int marginX = obstacleSettings != null ? obstacleSettings.marginX : 0;
                    int marginZ = obstacleSettings != null ? obstacleSettings.marginZ : 0;

                    if (CheckPosAvailability(obstaclePrefab, pos))
                    {
                        SpawnObstacle(obstaclePrefab, pos, obstacleRelativeScale, marginX, marginZ);
                    }
                }
            }
        }

        // Restore original random state
        Random.state = originalState;
    }

    private void GenerateLateralObstacles()
    {
        // Instantiate lateral obstacles
        GameObject obstaclePrefab = obstaclesDatabase.GetPrefabByName("LateralSnowTree");
        Vector3 obstacleRelativeScale = GetRelativeSize(obstaclePrefab.transform.localScale);
        var obstacleSettings = obstaclePrefab.GetComponent<ObstacleSettings>();
        int marginX = obstacleSettings != null ? obstacleSettings.marginX : 0;
        int marginZ = obstacleSettings != null ? obstacleSettings.marginZ : 0;
        foreach (List<Vector3> list in gridPositions)
        {
            int index = 0;
            foreach (Vector3 pos in list)
            {
                Vector3 spawnPos = pos;
                if (index % 2 != 0)
                {
                    spawnPos.z += obstacleRelativeScale.z;
                }

                if (CheckPosAvailability(obstaclePrefab, spawnPos))
                {
                    SpawnObstacle(obstaclePrefab, spawnPos, obstacleRelativeScale, marginX, marginZ);
                    index++;
                }
            }
        }
    }

    private void SpawnObstacle(GameObject prefab, Vector3 pos, Vector3 relativeScale, int marginX = 0, int marginZ = 0)
    {
        // Instantiate the obstacle
        GameObject newObstacle = Instantiate(prefab);
        newObstacle.transform.parent = transform;
        newObstacle.transform.localPosition = pos;
        newObstacle.transform.localRotation = Quaternion.identity;
        newObstacle.transform.localScale = relativeScale;

        // Add margin to the occupied space
        Vector3 occupedSpace = new Vector3(
            relativeScale.x + (2 * marginX * relativeGridScale.x),
            relativeScale.y,
            relativeScale.z + (2 * marginZ * relativeGridScale.z)
        );

        // Store the position and size of the new obstacle
        existingObstacles.Add((pos, occupedSpace));
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
            for (float z = -(transform.localScale.z) / 2f; (z + gridScale.z) <= transform.localScale.z / 2f; z += gridScale.z + gapZ)
        {
            List<Vector3> list = new List<Vector3>();

            for (float x = -(transform.localScale.x) / 2f; (x + gridScale.x) <= transform.localScale.x / 2f; x += gridScale.x + gapX)
            {
                list.Add(new Vector3((x + gridScale.x / 2f) / transform.localScale.x, 0.5f, (z + gridScale.z / 2f) / transform.localScale.z));
            }

            gridPositions.Add(list);
        }
    }
}
