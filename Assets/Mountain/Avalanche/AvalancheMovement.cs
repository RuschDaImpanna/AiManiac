using UnityEngine;

public class AvalancheMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject prefabMountainSection;
    [SerializeField] private GameObject mountain;
    [SerializeField] private SpeedBar speedBar; 
    [SerializeField] private AudioSource avalancheAudio; 

    [Header("Distance Settings")]
    [SerializeField] private float normalDistance = 2.5f;
    [SerializeField] private float warningDistance = 2.0f;
    [SerializeField] private float dangerDistance = 1.5f;
    [SerializeField] private float deadDistance = 1.0f;

    [Header("Audio Volume Settings")]
    [SerializeField] private float normalVolume = 0.2f;
    [SerializeField] private float warningVolume = 0.4f;
    [SerializeField] private float dangerVolume = 0.7f;
    [SerializeField] private float deadVolume = 1.0f;

    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 2f;
    [SerializeField] private bool useSmoothing = true;

    private float currentDistanceMultiplier;
    private float targetDistanceMultiplier;

    private float currentVolume;
    private float targetVolume;

    private PlayerState lastState = PlayerState.Normal;

    void Start()
    {
        // Buscar SpeedBar si no está asignado
        if (speedBar == null)
            speedBar = player.GetComponent<SpeedBar>();

        if (speedBar != null)
            speedBar.OnStateChanged += HandleStateChange;

        // Volumen inicial si el audio está asignado
        if (avalancheAudio != null)
        {
            currentVolume = normalVolume;
            targetVolume = normalVolume;
            avalancheAudio.volume = currentVolume;
        }

        // Inicializar distancia
        currentDistanceMultiplier = normalDistance;
        targetDistanceMultiplier = normalDistance;
    }

    void OnDestroy()
    {
        if (speedBar != null)
            speedBar.OnStateChanged -= HandleStateChange;
    }

    void Update()
    {
        if (player == null || prefabMountainSection == null || mountain == null) return;

        // Suavizar transición de distancia
        if (useSmoothing)
        {
            currentDistanceMultiplier = Mathf.Lerp(
                currentDistanceMultiplier,
                targetDistanceMultiplier,
                Time.deltaTime * smoothSpeed
            );

            // Suavizado del volumen
            if (avalancheAudio != null)
            {
                currentVolume = Mathf.Lerp(
                    currentVolume,
                    targetVolume,
                    Time.deltaTime * smoothSpeed
                );
                avalancheAudio.volume = currentVolume;
            }
        }
        else
        {
            currentDistanceMultiplier = targetDistanceMultiplier;

            if (avalancheAudio != null)
            {
                currentVolume = targetVolume;
                avalancheAudio.volume = currentVolume;
            }
        }

        // Calcular la posición de la avalancha
        transform.position = player.transform.position + new Vector3(
            0f,
            prefabMountainSection.transform.localScale.z *
            Mathf.Sin(mountain.transform.rotation.eulerAngles.x * Mathf.PI / 180) *
            currentDistanceMultiplier,

            -prefabMountainSection.transform.localScale.z *
            Mathf.Cos(mountain.transform.rotation.eulerAngles.x * Mathf.PI / 180) *
            currentDistanceMultiplier
        );
    }

    private void HandleStateChange(PlayerState newState)
    {
        lastState = newState;

        // --- Distancia ---
        targetDistanceMultiplier = newState switch
        {
            PlayerState.Normal => normalDistance,
            PlayerState.Warning => warningDistance,
            PlayerState.Danger => dangerDistance,
            PlayerState.Dead => deadDistance,
            _ => normalDistance
        };

        // --- Volumen ---
        targetVolume = newState switch
        {
            PlayerState.Normal => normalVolume,
            PlayerState.Warning => warningVolume,
            PlayerState.Danger => dangerVolume,
            PlayerState.Dead => deadVolume,
            _ => normalVolume
        };

        //Debug.Log($"Avalanche: Estado -> {newState}, Distancia: {targetDistanceMultiplier}, Volumen: {targetVolume}");
    }

    // Métodos públicos por si quieres modificar distancia o volumen externamente
    public void SetTargetDistance(float distance) => targetDistanceMultiplier = distance;
    public float GetCurrentDistance() => currentDistanceMultiplier;

    public void SetDistanceImmediate(float distance)
    {
        currentDistanceMultiplier = distance;
        targetDistanceMultiplier = distance;
    }
}
