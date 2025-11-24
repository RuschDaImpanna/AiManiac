using System;
using UnityEngine;

public class DestroyWallMove : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject prefabMountainSection;
    [SerializeField] private Boolean isFront = true;

    private GameObject mountain;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mountain = transform.parent.gameObject;
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
                -prefabMountainSection.transform.localScale.z * Mathf.Sin(mountain.transform.rotation.eulerAngles.x * Mathf.PI / 180) * 3.5f,
                prefabMountainSection.transform.localScale.z * Mathf.Cos(mountain.transform.rotation.eulerAngles.x * Mathf.PI / 180) * 3.5f
            );
        }
        
    }
}
