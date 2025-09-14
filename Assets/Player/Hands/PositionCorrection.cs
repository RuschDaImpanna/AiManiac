using UnityEngine;

public class PositionCorrection : MonoBehaviour
{
    public Transform parent;
    private float[] angles = { -70f, 0f, 70f };

    [Header("Remap de mano (Y,Z)")]
    public Vector2[] pos = {
        new Vector2(-1.95f, 4.5f), // (-70)
        new Vector2(-1.0f, 7f),  // (0)
        new Vector2(-0.7f, 4.5f)   // (70)
    };

    void Update()
    {
        float rotX = parent.localEulerAngles.x;
        if (rotX > 180f) rotX -= 360f; // Ajuste a rango [-180,180]

        // Posición más cercana
        int i = 0;
        while (i < angles.Length - 1 && rotX > angles[i + 1])

            i++;

            int j = Mathf.Min(i + 1, angles.Length - 1);

            float t = Mathf.InverseLerp(angles[i], angles[j], rotX);

            // Interpolación de posición (Y,Z)
            float newY = Mathf.Lerp(pos[i].x, pos[j].x, t);
            float newZ = Mathf.Lerp(pos[i].y, pos[j].y, t);

            // Aplicar la nueva posición (solo cambia Y y Z)
            transform.localPosition = new Vector3(transform.localPosition.x, newY, newZ);
            
        }
}
