using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private InputController mouseScript;

    private void Awake()
    {
        //Posición de mouse xd
        mouseScript = GetComponentInParent<InputController>();
    }

    [Header("Sensibility")]        
    public float mouseSensitivity = 100f;

    [Header("Limit (Up, Down)")]
    public Vector2 limitFront = new Vector2();
    public Vector2 limitBack = new Vector2();

    float xRotation = 0f;

    private void FixedUpdate()
    {
        Vector2 mouseMove = mouseScript.GetMouseMove();

        //En el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;

        //Calcular el movimiento del mouse con respecto a la sensibilidad y el tiempo
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

        // =======================
    //  Debug de límites
    // =======================
    Vector3 origin = transform.position;

    // Límite mínimo
    Quaternion rotMin = Quaternion.Euler(minX, transform.parent.eulerAngles.y, 0f);
    Vector3 dirMin = rotMin * Vector3.forward;
    Debug.DrawRay(origin, dirMin * 5f, Color.red);

    // Límite en 0 (centro de rango)
    float midX = (minX + maxX) * 0.5f;
    Quaternion rotMid = Quaternion.Euler(midX, transform.parent.eulerAngles.y, 0f);
    Vector3 dirMid = rotMid * Vector3.forward;
    Debug.DrawRay(origin, dirMid * 5f, Color.green);

    // Límite máximo
    Quaternion rotMax = Quaternion.Euler(maxX, transform.parent.eulerAngles.y, 0f);
    Vector3 dirMax = rotMax * Vector3.forward;
    Debug.DrawRay(origin, dirMax * 5f, Color.blue);
    }
}