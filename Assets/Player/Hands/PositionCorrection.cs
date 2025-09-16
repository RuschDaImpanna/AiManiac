using UnityEngine;

public class PositionCorrection : MonoBehaviour
{
    public Transform parent;
    public CameraMovement cm;
    private float[] frontAngles;
    private float[] backAngles;

    [Header("Remap de mano (Up, 0, Down)")]
    public Vector3[] frontPos = {
        new Vector3(), // Up
        new Vector3(), // 0
        new Vector3()  // Down
    };
    public Vector3[] backPos = {
        new Vector3(), // Up
        new Vector3(), // 0
        new Vector3()  // Down
    };

    void Awake()
    {

        //Tomar los ángulos límite de la cámara
        frontAngles = new float[] { cm.limitFront.x, 0f, cm.limitFront.y };
        backAngles = new float[] { cm.limitBack.x, 0f, cm.limitBack.y };

    }

    void Update()
    {
        float rotX = parent.localEulerAngles.x;
        if (rotX > 180f) rotX -= 360f; // Ajuste a rango [-180,180]

        //Interpolación de límites de ángulos según el parent
        float[] activeAngles = interpolateAngles(rotX);

        //Interpolación de posiciones (front/back)
        Vector3[] activePos = interpolatePositions(rotX);

        // Posición más cercana
        int i = 0;
        while (i < activeAngles.Length - 1 && rotX > activeAngles[i + 1])

            //Suma índice
            i++;

        //Elije el valor mínimo entre el índice y de los ángulos disponibles para encontrar el ángulo superior
        int j = Mathf.Min(i + 1, activeAngles.Length - 1);

        //Cálculo del rango entre el ángulo inferior y el ángulo superior según rotX
        float t = Mathf.InverseLerp(activeAngles[i], activeAngles[j], rotX);

        // Interpolación de posición (average)
        Vector3 newPos = Vector3.Lerp(activePos[i], activePos[j], t);

        // Aplicar la nueva posición
        transform.localPosition = newPos;

        debugDraw(activeAngles);
    }

    float[] interpolateAngles(float rotX)
    {

        // Interpolación entre front y back según rotX (average)
        float tInterp = Mathf.InverseLerp(0f, 180f, Mathf.Abs(rotX)); //Qué tan cerca está rotX de 180

        float limitX = Mathf.Lerp(cm.limitFront.x, cm.limitBack.x, tInterp); //Límite entre las x de front y back

        float limitZ = Mathf.Lerp(cm.limitFront.y, cm.limitBack.y, tInterp); //Límite entre las z de front y back
        
        return new float[] {limitX, 0f, limitZ}; // Array dinámico según rotX

    }

    Vector3[] interpolatePositions(float rotX)
    {

        Vector3[] activePos = new Vector3[3];

        // Interpolación entre front y back según rotX (average)
        float tInterp = Mathf.InverseLerp(0f, 180f, Mathf.Abs(rotX)); //Qué tan cerca está rotX de 180

        for (int k = 0; k < 3; k++)
        {
            activePos[k] = Vector3.Lerp(frontPos[k], backPos[k], tInterp);
        }

        return activePos;

    }

    void debugDraw(float[] activeAngles)
    {
        
        // Dibujo en debug de los límites
        Vector3 origin = parent.position; // punto de referencia (puede ser el pivot del objeto)

        // Límite mínimo (activeAngles[0])
        Quaternion rotMin = Quaternion.Euler(activeAngles[0], 0f, 0f);
        Vector3 dirMin = rotMin * Vector3.forward;
        Debug.DrawRay(origin, dirMin * 5f, Color.red, 5); // rojo para el límite mínimo

        // Límite en cero (activeAngles[1])
        Quaternion rotZero = Quaternion.Euler(activeAngles[1], 0f, 0f);
        Vector3 dirZero = rotZero * Vector3.forward;
        Debug.DrawRay(origin, dirZero * 5f, Color.green); // verde para el centro (0)

        // Límite máximo (activeAngles[2])
        Quaternion rotMax = Quaternion.Euler(activeAngles[2], 0f, 0f);
        Vector3 dirMax = rotMax * Vector3.forward;
        Debug.DrawRay(origin, dirMax * 5f, Color.red, 5); // rojo para el límite máximo

    }
}
