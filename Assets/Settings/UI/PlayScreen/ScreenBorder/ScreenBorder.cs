using System;
using UnityEngine;
using UnityEngine.UI;

public class ScreenBorder : MonoBehaviour
{
    [Header("Border Settings")]
    [SerializeField] private Image interiorBorderImage;
    [SerializeField] private Image exteriorBorderImage;
    [SerializeField] private Image flashImage;
    [SerializeField] private Color normalColor = new Color(1, 1, 1, 0); // Transparent
    [SerializeField] private Color warningColor = new Color(1, 1, 0, 0.5f); // Yellow
    [SerializeField] private Color dangerColor = new Color(1, 0, 0, 0.5f); // Red
    [SerializeField] private Color freezeColor = new Color(0, 1, 1, 0.5f); // Cyan
    [SerializeField] private float fadeSpeed = 5f;

    [Header("Flash Settings")]
    [SerializeField] private Color flashColor = new Color(1, 0, 0, 0.7f); // Bright Red
    [SerializeField] private Color flashBorderColor = new Color(0, 0, 0, 0.5f);
    [SerializeField] private float flashDuration = 1f;

    private Color targetColor;
    private Color currentColor;
    private bool isFlashing = false;

    public Color DangerColor { get { return dangerColor; } }
    public Color WarningColor { get { return warningColor; } }
    
    void Start()
    {
        interiorBorderImage.color = freezeColor;
        exteriorBorderImage.color = freezeColor;
        targetColor = normalColor;
    }

    void Update()
    {
        // Smoothly transition to target color
        if (!isFlashing) changeBorderColorProgressively(interiorBorderImage.color, targetColor, fadeSpeed);
    }

    // Call these methods based on your conditions
    public void SetNormal()
    {
        targetColor = normalColor;
        currentColor = normalColor;
    }

    public void SetWarning(bool fastTransition = false)
    {
        if (fastTransition && currentColor != dangerColor)
        {
            changeBorderColor(warningColor);
        }

        targetColor = warningColor;
        currentColor = warningColor;
    }

    public void SetDanger(bool fastTransition = false)
    {
        if (fastTransition)
        {
            changeBorderColor(normalColor);
            StartCoroutine(FlashCoroutine(flashDuration, onComplete: () =>
            {
                changeBorderColor(dangerColor);
            }));
        }

        targetColor = dangerColor;
        currentColor = dangerColor;
    }

    // Flash effect for damage
    public void FlashDanger(float duration = 1f)
    {
        StartCoroutine(FlashCoroutine(duration));
    }

    private System.Collections.IEnumerator FlashCoroutine(float duration, Action onComplete = null)
    {
        isFlashing = true;
        float transitionSpeed = 1f;

        flashImage.color = Color.Lerp(flashImage.color, flashColor, transitionSpeed);
        
        yield return new WaitForSeconds(duration);

        flashImage.color = Color.Lerp(flashImage.color, normalColor, transitionSpeed);

        isFlashing = false;
        onComplete?.Invoke();
    }

    private void changeBorderColorProgressively(Color colorFrom, Color toColor, float transitionVelocity)
    {
        interiorBorderImage.color = Color.Lerp(colorFrom, toColor, Time.deltaTime * transitionVelocity);
        exteriorBorderImage.color = Color.Lerp(colorFrom, toColor, Time.deltaTime * transitionVelocity);
    }

    private void changeBorderColor(Color newColor)
    {
        interiorBorderImage.color = newColor;
        exteriorBorderImage.color = newColor;
    }
}