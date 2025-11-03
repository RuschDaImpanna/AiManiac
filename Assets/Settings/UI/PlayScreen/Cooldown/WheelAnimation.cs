using UnityEngine;

public class WheelAnimation : MonoBehaviour
{

    private Animator WheelAnimator;
    
    public WeaponRecoil weapon;

    void Awake()
    {

        WheelAnimator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        float cooldown = weapon.CurrentCooldown;


        WheelAnimator.SetFloat("Cooldown", cooldown);

        
    }
}
