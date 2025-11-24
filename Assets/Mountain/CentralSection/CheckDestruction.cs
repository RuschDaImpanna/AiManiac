using UnityEngine;

public class CheckDestruction : MonoBehaviour
{
    private string parentName;

    private void Start()
    {
        parentName = transform.parent != null ? transform.parent.name : "No parent";
        Debug.Log($"DestructionDetector iniciado en {gameObject.name}, padre: {parentName}");
    }

    private void OnDestroy()
    {
        Debug.LogError($"¡{gameObject.name} está siendo destruido!");
        Debug.LogError($"Padre cuando fue creado: {parentName}");
        Debug.LogError($"Padre actual: {(transform.parent != null ? transform.parent.name : "NULL - El padre fue destruido")}");
        Debug.LogError("Stack trace completo:");
        Debug.LogError(System.Environment.StackTrace);
    }
}