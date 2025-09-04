using UnityEngine;

public class ObstaclesGeneration : MonoBehaviour
{
    public GameObject obstacle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 newLocalScale = new Vector3(
            obstacle.transform.localScale.x / transform.localScale.x,
            obstacle.transform.localScale.y / transform.localScale.y,
            obstacle.transform.localScale.z / transform.localScale.z
        );

        float obstacleSpacingX = 0;
        float obstacleSpacingZ = 0;

        for (float x = -(transform.localScale.x - obstacle.transform.localScale.x) / 2f; (x + obstacle.transform.localScale.x) < transform.localScale.x / 2f; x += obstacle.transform.localScale.x + obstacleSpacingX)
        {
            for (float z = -(transform.localScale.z - obstacle.transform.localScale.z) / 2f; (z + obstacle.transform.localScale.z) < transform.localScale.z / 2f; z += obstacle.transform.localScale.z + obstacleSpacingZ)
            {
                GameObject newObstacle = Instantiate(obstacle, transform);
                newObstacle.transform.localPosition = new Vector3(x / transform.localScale.x, 1.5f, z / transform.localScale.z);
                newObstacle.transform.localRotation = Quaternion.identity;
                newObstacle.transform.localScale = newLocalScale;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
