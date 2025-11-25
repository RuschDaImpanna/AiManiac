using UnityEngine;

public class Shadow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask groundLayer;

    [Header("Shadow Settings")]
    [SerializeField] private float raycastHeight = 10f;
    [SerializeField] private float initialSize = 1f;
    [SerializeField] private float minSize = 0.3f;
    [SerializeField] private float maxHeight = 5f;
    [SerializeField] private float offsetGround = 0.01f;
    [SerializeField] private bool dissipateWithHeight = true;

    private Renderer shadowRenderer;
    private Material shadowMaterial;

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        // Lanzar un raycast desde el personaje hacia abajo
        RaycastHit hit;
        Vector3 rayOrigin = player.position;

        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastHeight, groundLayer))
        {
            // Posicionar la sombra en el suelo
            Vector3 shadowPosition = hit.point + Vector3.up * offsetGround;
            transform.position = shadowPosition;

            // Calcular la altura del personaje sobre el suelo
            float currentHeight = hit.distance;

            // Calcular el tamaño de la sombra basado en la altura
            float scaleFactor = Mathf.Lerp(1f, minSize / initialSize, currentHeight / maxHeight);
            scaleFactor = Mathf.Clamp(scaleFactor, minSize / initialSize, 1f);

            // Aplicar la escala
            Vector3 scale = new Vector3(initialSize * scaleFactor, initialSize * scaleFactor, 1f);
            transform.localScale = scale;

            // Desvanecer la sombra si está habilitado
            if (dissipateWithHeight && shadowMaterial != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, currentHeight / maxHeight);
                Color color = shadowMaterial.color;
                color.a = alpha;
                shadowMaterial.color = color;
            }

            // Alinear la sombra con la normal del suelo (opcional)
            //transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            // Mostrar la sombra
            if (!shadowRenderer.enabled)
            {
                shadowRenderer.enabled = true;
            }
        }
        else
        {
            // Si no hay suelo debajo, ocultar la sombra
            if (shadowRenderer.enabled)
            {
                shadowRenderer.enabled = false;
            }
        }
    }

    // Visualización en el editor
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(player.position, player.position + Vector3.down * raycastHeight);
        }
    }
}
