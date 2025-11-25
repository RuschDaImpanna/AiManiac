using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WheelAnimation : MonoBehaviour
{
    [Header("References")]
    private Image wheel;
    private Animator WheelAnimator;
    [SerializeField] private WeaponRecoil weapon;

    [Header("Cooldown Sprites")]
    [SerializeField] private Sprite sprite100; // Cooldown completo (recién disparado)
    [SerializeField] private Sprite sprite66;  // 66% del cooldown restante
    [SerializeField] private Sprite sprite33;  // 33% del cooldown restante
    [SerializeField] private Sprite sprite0;   // Cooldown terminado (listo para disparar)

    private float cooldown;
    private Sprite lastSprite;

    void Awake()
    {
        wheel = gameObject.GetComponent<Image>();
        WheelAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (weapon != null)
        {
            cooldown = weapon.Cooldown;
        }
    }

    void FixedUpdate()
    {
        if (weapon == null || wheel == null) return;


        float currentCooldown = weapon.CurrentCooldown;

        //Debug.Log($"currentCooldown: {currentCooldown}; cooldown: {cooldown}");

        // Mostrar/ocultar la rueda según si hay cooldown activo
        wheel.enabled = currentCooldown > 0;

        if (currentCooldown > 0)
        {
            // Calcular el porcentaje del cooldown restante
            float cooldownPercentage = currentCooldown / cooldown;

            // Seleccionar el sprite según el porcentaje
            Sprite newSprite = GetSpriteForPercentage(cooldownPercentage);

            // Solo cambiar el sprite si es diferente al actual (optimización)
            if (newSprite != null && newSprite != lastSprite)
            {
                //Debug.Log(newSprite.name);
                wheel.sprite = newSprite;
                lastSprite = newSprite;
            }
        }
        else
        {
            // Reset del último sprite cuando el cooldown termina
            lastSprite = null;
        }
    }

    private Sprite GetSpriteForPercentage(float percentage)
    {
        // percentage = 1.0 significa cooldown completo (100%)
        // percentage = 0.0 significa cooldown terminado (0%)

        if (percentage > 0.66f)
        {
            // Más del 66% del cooldown restante
            return sprite100;
        }
        else if (percentage > 0.33f)
        {
            // Entre 66% y 33% del cooldown restante
            return sprite66;
        }
        else if (percentage > 0.0f)
        {
            // Entre 33% y 0% del cooldown restante
            return sprite33;
        }
        else
        {
            // Cooldown terminado (listo para disparar)
            return sprite0;
        }
    }
}