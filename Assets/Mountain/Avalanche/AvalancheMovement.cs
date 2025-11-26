using UnityEngine;

public class AvalancheMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject prefabMountainSection;
    [SerializeField] private GameObject mountain;
    [SerializeField] private SpeedBar speedBar; // Para obtener el estado del jugador

    [Header("Distance Settings")]
    [SerializeField] private float normalDistance = 2.5f; // Distancia cuando está en estado Normal
    [SerializeField] private float warningDistance = 2.0f; // Distancia cuando está en Warning
    [SerializeField] private float dangerDistance = 1.5f; // Distancia cuando está en Danger
    [SerializeField] private float deadDistance = 1.0f; // Distancia cuando está en Dead (muy cerca)

    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 2f; // Velocidad de transición entre distancias
    [SerializeField] private bool useSmoothing = true;

    private float currentDistanceMultiplier;
    private float targetDistanceMultiplier;
    private PlayerState lastState = PlayerState.Normal;

    void Start()
    {
        // Buscar SpeedBar si no está asignado
        if (speedBar == null)
        {
            speedBar = player.GetComponent<SpeedBar>();
        }

        if (speedBar == null)
        {
            Debug.LogWarning("SpeedBar no encontrado. La distancia no cambiará según el estado.");
        }
        else
        {
            // Suscribirse al evento de cambio de estado
            speedBar.OnStateChanged += HandleStateChange;
        }

        // Inicializar la distancia
        currentDistanceMultiplier = normalDistance;
        targetDistanceMultiplier = normalDistance;
    }

    void OnDestroy()
    {
        // Limpiar suscripción
        if (speedBar != null)
        {
            speedBar.OnStateChanged -= HandleStateChange;
        }
    }

    void Update()
    {
        if (player == null || prefabMountainSection == null || mountain == null) return;

        // Suavizar la transición de distancia
        if (useSmoothing)
        {
            currentDistanceMultiplier = Mathf.Lerp(
                currentDistanceMultiplier,
                targetDistanceMultiplier,
                Time.deltaTime * smoothSpeed
            );
        }
        else
        {
            currentDistanceMultiplier = targetDistanceMultiplier;
        }

        // Calcular la posición con la distancia actual
        transform.position = player.transform.position + new Vector3(
            0f,
            prefabMountainSection.transform.localScale.z * Mathf.Sin(mountain.transform.rotation.eulerAngles.x * Mathf.PI / 180) * currentDistanceMultiplier,
            -prefabMountainSection.transform.localScale.z * Mathf.Cos(mountain.transform.rotation.eulerAngles.x * Mathf.PI / 180) * currentDistanceMultiplier
        );
    }

    private void HandleStateChange(PlayerState newState)
    {
        lastState = newState;

        // Cambiar la distancia objetivo según el estado
        targetDistanceMultiplier = newState switch
        {
            PlayerState.Normal => normalDistance,
            PlayerState.Warning => warningDistance,
            PlayerState.Danger => dangerDistance,
            PlayerState.Dead => deadDistance,
            _ => normalDistance
        };

        Debug.Log($"Avalanche: Estado cambiado a {newState}, distancia objetivo: {targetDistanceMultiplier}");
    }

    // Método público para cambiar la distancia manualmente si es necesario
    public void SetTargetDistance(float distance)
    {
        targetDistanceMultiplier = distance;
    }

    // Método para obtener la distancia actual
    public float GetCurrentDistance()
    {
        return currentDistanceMultiplier;
    }

    // Método para forzar una distancia inmediatamente (sin suavizado)
    public void SetDistanceImmediate(float distance)
    {
        currentDistanceMultiplier = distance;
        targetDistanceMultiplier = distance;
    }
}