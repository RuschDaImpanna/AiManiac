using System;
using UnityEngine;

public class DestroyWallMove : MonoBehaviour
{
    public GameObject player;
    public GameObject prefabMountainSection;
    public Boolean isFront = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFront)
        {
            transform.position = player.transform.position + new Vector3(
                0f,
                0f,
                transform.localScale.z
            );
        } else
        {
            transform.position = player.transform.position + new Vector3(
                0f,
                -prefabMountainSection.transform.localScale.z * Mathf.Sin(prefabMountainSection.transform.rotation.eulerAngles.x * Mathf.PI / 180) * 3.5f,
                prefabMountainSection.transform.localScale.z * Mathf.Cos(prefabMountainSection.transform.rotation.eulerAngles.x * Mathf.PI / 180) * 3.5f
            );
        }
        
    }
}
