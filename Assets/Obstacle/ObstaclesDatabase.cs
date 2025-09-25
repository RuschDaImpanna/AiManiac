using UnityEngine;

[CreateAssetMenu(fileName = "ObstaclesDatabase", menuName = "Scriptable Objects/ObstaclesDatabase")]
public class ObstaclesDatabase : ScriptableObject
{
    [System.Serializable]
    public class ObstacleData
    {
        [Header("Basic Info")]
        public string displayName;
        public GameObject prefab;

        [Header("Properties")]
        public int rarity = 1;
        public int marginX = 0;
        public int marginZ = 0;
    }

    [Header("All Obstacles")]
    public ObstacleData[] allObstacles;

    public ObstacleData GetPrefabByName(string name)
    {
        foreach (ObstacleData data in allObstacles)
        {
            if (data.displayName == name)
                return data;
        }

        Debug.LogWarning($"Obstacle '{name}' not found in database");
        return null;
    }

    public ObstacleData GetWeightedRandomPrefab()
    {
        // Calculate total weight
        int totalWeight = 0;
        foreach (ObstacleData data in allObstacles)
        {
            totalWeight += data.rarity;
        }

        if (totalWeight == 0) return null;

        // Pick random number within total weight
        int randomValue = Random.Range(0, totalWeight);

        // Find which obstacle this corresponds to
        int currentWeight = 0;
        foreach (ObstacleData data in allObstacles)
        {
            currentWeight += data.rarity;
            if (randomValue < currentWeight)
            {
                return data;
            }
        }

        // Fallback
        return allObstacles[0];
    }
}
