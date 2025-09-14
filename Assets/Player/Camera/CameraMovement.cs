using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private InputController mouseScript;

    private void Awake()
    {
        mouseScript = GetComponentInParent<InputController>();
    }

    [Header("Sensibility")]        
    public float mouseSensitivity = 100f;

    [Header("Limit (Up, Down)")]
    public Vector2 limitFront = new Vector2(-70f, 70f);
    public Vector2 limitBack = new Vector2(-70f, 16f);

    float xRotation = 0f;

    private void FixedUpdate()
    {
        Vector2 mouseMove = mouseScript.GetMouseMove();

        Cursor.lockState = CursorLockMode.Locked;

        float mouseX = mouseMove.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseMove.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        // Obtener rotación Y del parent en rango [-180,180]
        float parentY = transform.parent.eulerAngles.y;
        if (parentY > 180f) parentY -= 360f;
        float absParentY = Mathf.Abs(parentY); // 0..180

        // "Booling" entre 0 (en 0°) a 1 (en 180°)
        float t = Mathf.InverseLerp(0f, 180f, absParentY);

        // Interpolar los límites en X según t
        float minX = Mathf.Lerp(limitFront.x, limitBack.x, t);
        float maxX = Mathf.Lerp(limitFront.y, limitBack.y, t);


        if (minX > maxX)
        {
            float tmp = minX; minX = maxX; maxX = tmp;
        }

        Debug.Log($"parentY={parentY:F1} | absY={absParentY:F1} | t={t:F2} | minX={minX:F1} | maxX={maxX:F1} | xRot={xRotation:F1}");


        xRotation = Mathf.Clamp(xRotation, minX, maxX);
        // Rotación vertical
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Rotación horizontal
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}