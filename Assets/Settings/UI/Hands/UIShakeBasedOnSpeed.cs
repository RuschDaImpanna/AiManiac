using UnityEngine;

public class UIShakeBasedOnSpeed : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody playerRb;  // Rigidbody del jugador

    [Header("Speed Settings")]
    [SerializeField] private float minSpeed = 100f;
    [SerializeField] private float maxSpeed = 200f;

    [Header("UI Shake Settings")]
    [SerializeField] private float maxShakeAmount = 10f; // Movimiento máximo en píxeles
    [SerializeField] private float shakeSpeed = 20f;

    private RectTransform rectTransform;
    private Vector2 originalPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float currentSpeed = playerRb.linearVelocity.magnitude * 3.6f;

        if (currentSpeed < minSpeed)
        {
            rectTransform.anchoredPosition = originalPosition;
            return;
        }

        float speedPercent = Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed);

        float shakeAmount = maxShakeAmount * speedPercent;

        float offsetX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0) - 0.5f) * shakeAmount;
        float offsetY = (Mathf.PerlinNoise(0, Time.time * shakeSpeed) - 0.5f) * shakeAmount;

        rectTransform.anchoredPosition = originalPosition + new Vector2(offsetX, offsetY);
    }
}
