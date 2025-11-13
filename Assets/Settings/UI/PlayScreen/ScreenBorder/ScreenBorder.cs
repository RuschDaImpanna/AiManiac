using UnityEngine;
using UnityEngine.UI;

public enum FlashType
{ 
    Danger,
    Warning
}
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

    private Color targetColor;
    private Color currentColor;

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
        interiorBorderImage.color = Color.Lerp(interiorBorderImage.color, targetColor, fadeSpeed * Time.deltaTime);
        exteriorBorderImage.color = Color.Lerp(exteriorBorderImage.color, targetColor, fadeSpeed * Time.deltaTime);
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
            interiorBorderImage.color = warningColor;
            exteriorBorderImage.color = warningColor;
        }

        targetColor = warningColor;
        currentColor = warningColor;
    }

    public void SetDanger(bool fastTransition = false)
    {
        if (fastTransition)
        {
            interiorBorderImage.color = dangerColor;
            exteriorBorderImage.color = dangerColor;
        }

        targetColor = dangerColor;
        currentColor = dangerColor;
    }

    // Flash effect for damage
    public void FlashDanger(float duration = 2f, FlashType targetFlash = FlashType.Danger)
    {
        StartCoroutine(FlashCoroutine(duration, targetFlash));
    }
    
    private System.Collections.IEnumerator FlashCoroutine(float duration, FlashType targetFlash)
    {
        flashImage.color = Color.Lerp(flashImage.color, targetFlash == FlashType.Danger ? dangerColor : warningColor, Time.deltaTime);
        yield return new WaitForSeconds(duration);
        flashImage.color = Color.Lerp(flashImage.color, normalColor, Time.deltaTime);
    }
}