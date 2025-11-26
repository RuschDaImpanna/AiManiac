using Unity.Cinemachine;
using UnityEngine;

public class FOVScaleCompensator : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CinemachineCamera cinemachineCamera; // Tu cámara de Cinemachine

    [Header("Configuración")]
    [SerializeField] private float baseFOV = 60f; // Tu normalFOV (el FOV mínimo)
    [SerializeField] private float scaleMultiplier = 1f; // Ajusta la intensidad de la compensación
    [SerializeField] private bool smoothTransition = true; // Suavizar el cambio de escala
    [SerializeField] private float smoothSpeed = 5f; // Velocidad del suavizado (igual que tu FOV)

    [Header("Opciones Avanzadas")]
    [SerializeField] private bool useSquareRoot = true; // Usar raíz cuadrada para compensación más suave

    private Vector3 baseScale;
    private Vector3 targetScale;
    private float currentFOV;

    void Start()
    {
        // Buscar la cámara Cinemachine si no está asignada
        if (cinemachineCamera == null)
        {
            cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
        }

        if (cinemachineCamera == null)
        {
            Debug.LogError("No se encontró CinemachineCamera. Asigna la cámara en el Inspector.");
            enabled = false;
            return;
        }

        // Guardar la escala base del objeto
        baseScale = transform.localScale;

        // Establecer el FOV base desde la cámara si no se ha configurado
        if (baseFOV <= 0)
        {
            baseFOV = cinemachineCamera.Lens.FieldOfView;
        }

        Debug.Log($"FOVScaleCompensator iniciado en {gameObject.name} - Base FOV: {baseFOV}");
    }

    void Update()
    {
        if (cinemachineCamera == null) return;

        // Obtener el FOV actual de la cámara Cinemachine
        currentFOV = cinemachineCamera.Lens.FieldOfView;

        // Calcular el factor de escala basado en el FOV actual
        float fovRatio = currentFOV / baseFOV;

        // Calcular el factor de escala
        float scaleFactor;
        if (useSquareRoot)
        {
            // Usar raíz cuadrada para que el cambio sea menos extremo y más natural
            scaleFactor = Mathf.Sqrt(fovRatio) * scaleMultiplier;
        }
        else
        {
            // Compensación lineal (más agresiva)
            scaleFactor = fovRatio * scaleMultiplier;
        }

        // Calcular la nueva escala objetivo
        targetScale = baseScale * scaleFactor;

        // Aplicar la escala con o sin suavizado
        if (smoothTransition)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                Time.deltaTime * smoothSpeed
            );
        }
        else
        {
            transform.localScale = targetScale;
        }
    }

    // Método para ajustar el FOV base dinámicamente
    public void SetBaseFOV(float fov)
    {
        baseFOV = fov;
        Debug.Log($"Base FOV actualizado a: {baseFOV}");
    }

    // Método para ajustar el multiplicador dinámicamente
    public void SetScaleMultiplier(float multiplier)
    {
        scaleMultiplier = multiplier;
    }

    // Método para resetear la escala a la base
    public void ResetScale()
    {
        transform.localScale = baseScale;
    }

    // Obtener información de debug
    public void LogDebugInfo()
    {
        Debug.Log($"Object: {gameObject.name}\n" +
                  $"Current FOV: {currentFOV:F1}\n" +
                  $"Base FOV: {baseFOV:F1}\n" +
                  $"FOV Ratio: {(currentFOV / baseFOV):F2}\n" +
                  $"Current Scale: {transform.localScale}\n" +
                  $"Base Scale: {baseScale}");
    }
}