using UnityEngine;
using UnityEngine.UI;

public class SpeedometerController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Image frameImage; // El marco del velocímetro
    [SerializeField] private Image backgroundFill; // Imagen de fondo para el "fill" de color
    [SerializeField] private Transform needleImageTransform; // La aguja
    [SerializeField] private SpeedBar speedBar; // Referencia al SpeedBar para obtener la velocidad

    [Header("Configuración de la Aguja")]
    [SerializeField] private float minAngle = 180f; // Ángulo cuando velocidad = 0 (180° = izquierda)
    [SerializeField] private float maxAngle = -90f; // Ángulo cuando velocidad = máxima (-90° = arriba)
    [SerializeField] private float startAngle = 270f; // Ángulo inicial de la aguja (270° = abajo)
    [SerializeField] private float smoothSpeed = 5f; // Suavizado del movimiento de la aguja

    [Header("Configuración de Colores")]
    [SerializeField] private bool useColorFeedback = true; // Activar cambio de color según estado
    [SerializeField] private Color normalColor = new Color(0f, 1f, 0.9f); // Verde/Cyan
    [SerializeField] private Color warningColor = Color.yellow; // Amarillo
    [SerializeField] private Color dangerColor = Color.red; // Rojo
    [SerializeField] private Color deadColor = new Color(0.5f, 0f, 0f); // Rojo oscuro

    private float currentNeedleAngle;
    private float targetNeedleAngle;
    private PlayerState lastState = PlayerState.Normal;

    void Start()
    {
        // Buscar SpeedBar si no está asignado
        if (speedBar == null)
        {
            speedBar = FindFirstObjectByType<SpeedBar>();
        }

        if (speedBar == null)
        {
            Debug.LogError("SpeedBar no encontrado! Asigna la referencia en el Inspector.");
        }

        // Inicializar la aguja en la posición inicial (270°)
        currentNeedleAngle = startAngle;
        if (needleImageTransform != null)
        {
            needleImageTransform.rotation = Quaternion.Euler(0, 0, currentNeedleAngle);
        }

        // Inicializar color de fondo
        if (backgroundFill != null && useColorFeedback)
        {
            backgroundFill.color = normalColor;
        }

        // Suscribirse al evento de cambio de estado
        if (speedBar != null)
        {
            speedBar.OnStateChanged += HandleStateChange;
        }
    }

    void OnDestroy()
    {
        // Limpiar suscripción al destruir el objeto
        if (speedBar != null)
        {
            speedBar.OnStateChanged -= HandleStateChange;
        }
    }

    void Update()
    {
        if (speedBar == null) return;

        // Obtener velocidad actual y máxima del SpeedBar
        float currentSpeed = speedBar.Speed;
        float maxSpeed = speedBar.MaxSpeed;

        // Actualizar aguja
        UpdateNeedle(currentSpeed, maxSpeed);
    }

    private void UpdateNeedle(float currentSpeed, float maxSpeed)
    {
        if (needleImageTransform == null) return;

        // Evitar división por cero
        if (maxSpeed <= 0f)
        {
            targetNeedleAngle = startAngle;
        }
        else
        {
            // Calcular el ángulo objetivo según la velocidad
            float speedPercentage = Mathf.Clamp01(currentSpeed / maxSpeed);
            targetNeedleAngle = Mathf.Lerp(minAngle, maxAngle, speedPercentage);
        }

        // Suavizar el movimiento de la aguja
        currentNeedleAngle = Mathf.Lerp(currentNeedleAngle, targetNeedleAngle, Time.deltaTime * smoothSpeed);

        // Obtener la rotación local actual para preservar X e Y del padre
        Vector3 currentLocalEuler = needleImageTransform.localEulerAngles;

        // Aplicar rotación solo en el eje Z, manteniendo X e Y del padre
        needleImageTransform.localRotation = Quaternion.Euler(0f, 0f, currentNeedleAngle);
    }

    private void HandleStateChange(PlayerState newState)
    {
        if (!useColorFeedback || backgroundFill == null) return;

        lastState = newState;
        UpdateBackgroundColorByState(newState);
    }

    private void UpdateBackgroundColorByState(PlayerState state)
    {
        if (backgroundFill == null) return;

        Color targetColor = state switch
        {
            PlayerState.Normal => normalColor,
            PlayerState.Warning => warningColor,
            PlayerState.Danger => dangerColor,
            PlayerState.Dead => deadColor,
            _ => normalColor
        };

        // Suavizar transición de color
        backgroundFill.color = Color.Lerp(backgroundFill.color, targetColor, Time.deltaTime * smoothSpeed);
        //backgroundFill.color = targetColor;
    }

    // Método público para cambiar color manualmente si es necesario
    public void SetBackgroundColor(Color color)
    {
        if (backgroundFill != null)
        {
            backgroundFill.color = color;
        }
    }

    // Método público para obtener el estado actual
    public PlayerState GetCurrentState()
    {
        return lastState;
    }
}