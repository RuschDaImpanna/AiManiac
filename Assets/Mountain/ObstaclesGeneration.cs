using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ObstaclesGeneration : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Generate grid positions
        List<List<Vector3>> gridPositions = new List<List<Vector3>>();

        Vector3 gridScale = Vector3.one;
        float gapX = 0;
        float gapZ = 0;

        for (float x = -(transform.localScale.x) / 2f; (x + gridScale.x) <= transform.localScale.x / 2f; x += gridScale.x + gapX)
        {
            List<Vector3> list = new List<Vector3>();

            for (float z = -(transform.localScale.z) / 2f; (z + gridScale.z) <= transform.localScale.z / 2f; z += gridScale.z + gapZ)
            {
                list.Add(new Vector3((x + gridScale.x / 2f) / transform.localScale.x, 1f, (z + gridScale.z / 2f) / transform.localScale.z));


                Debug.Log(x + gridScale.x + " < " + transform.localScale.x / 2f);
            }

            gridPositions.Add(list);
        }

        // Instantiate cubes at random positions
        foreach (List<Vector3> list in gridPositions)
        {
            foreach (Vector3 pos in list)
            {
                if (Random.Range(0, 100) < 30) // 30% chance to spawn an obstacle
                {
                    GameObject newObstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    newObstacle.transform.SetParent(transform);
                    newObstacle.transform.localPosition = pos;
                    newObstacle.transform.localRotation = Quaternion.identity;
                    newObstacle.transform.localScale = GetRelativeSize(Vector3.one);
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
    }

    Vector3 GetRelativeSize(Vector3 size)
    {
        return new Vector3(size.x / transform.localScale.x, size.y / transform.localScale.y, size.z / transform.localScale.z);
    }
}
