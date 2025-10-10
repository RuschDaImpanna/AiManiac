using UnityEngine;

public class CollisionForwarder : MonoBehaviour
{
    private GameObject parentObject;

    void Start()
    {
        parentObject = transform.parent.gameObject;
    }

    void OnTriggerEnter(Collider other)
    {
        // Forward collision to parent's bounce script
        parentObject.SendMessage("OnTriggerEnter", other, SendMessageOptions.DontRequireReceiver);
    }
}