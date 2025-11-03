using UnityEngine;
using UnityEngine.UI;

public class WheelAnimation : MonoBehaviour
{

    private Image wheel;
    private Animator WheelAnimator;
    
    public WeaponRecoil weapon;

    void Awake()
    {

        wheel = gameObject.GetComponent<Image>();

        WheelAnimator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        float cooldown = weapon.CurrentCooldown;

        wheel.enabled = cooldown > 0;

        WheelAnimator.SetFloat("Cooldown", cooldown);

        
    }
}
