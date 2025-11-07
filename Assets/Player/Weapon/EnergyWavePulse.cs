using UnityEngine;

public class EnergyWavePulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private float maxScale = 10f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0, 1, 1, 0);

    [Header("Visual")]
    [SerializeField] private Renderer waveRenderer;
    [SerializeField] private Color startColor = Color.cyan;
    [SerializeField] private Color endColor = Color.blue;

    [Header("Options")]
    [SerializeField] private bool destroyOnComplete = true;

    private float elapsedTime;
    private Vector3 originalScale;
    private Material material;

    void Start()
    {
        originalScale = transform.localScale;

        if (waveRenderer != null)
        {
            material = waveRenderer.material;
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float progress = elapsedTime / duration;

        if (progress <= 1f)
        {
            // Expand scale
            float scaleMultiplier = scaleCurve.Evaluate(progress) * maxScale;
            transform.localScale = originalScale * scaleMultiplier;

            // Fade and color shift
            if (material != null)
            {
                Color color = Color.Lerp(startColor, endColor, progress);
                color.a = fadeCurve.Evaluate(progress);
                material.color = color;
            }
        }
        else if (destroyOnComplete)
        {
            Destroy(gameObject);
        }
    }
}
