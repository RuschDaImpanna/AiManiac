using UnityEngine;
using UnityEngine.InputSystem;

public class HandsAnimations : MonoBehaviour
{

    public GameObject player;

    private WeaponRecoil weapon;

    private InputController input;

    private GameObject LGun;
    private GameObject RGun;

    private Animator LAnimator;
    private Animator RAnimator;

    public RectTransform LRt;


    void Awake()
    {
        //Tener la cosa esa de la pistola
        weapon = player.GetComponent<WeaponRecoil>();

        //Tener el script de los disparos
        input = player.GetComponent<InputController>();

        //Obtener los hijos
        LGun = this.gameObject.transform.GetChild(0).gameObject;
        RGun = this.gameObject.transform.GetChild(1).gameObject;

        //Obtener el controlador de animación de cada uno
        LAnimator = LGun.GetComponent<Animator>();
        RAnimator = RGun.GetComponent<Animator>();

    }

    void FixedUpdate()
    {

        //Cooldown
        float cooldown = weapon.CurrentCooldown;

        //Asigna los booleanos según lo que se presionó y detecto InputController del player
        bool LAttack = input.LShoot();
        bool RAttack = input.RShoot();

        LAnimator.SetBool("Shoot", LAttack);
        LAnimator.SetFloat("Cooldown", cooldown);

        RAnimator.SetBool("Shoot", RAttack);
        
        AnimatorStateInfo stateInfo = LAnimator.GetCurrentAnimatorStateInfo(0);
        Vector2 offset = LRt.offsetMax;
        
        AnimatorClipInfo[] clipInfo = LAnimator.GetCurrentAnimatorClipInfo(0);

if (clipInfo.Length > 0)
{
    string clipName = clipInfo[0].clip.name;
    //Debug.Log("Se está reproduciendo el clip: " + clipName);
}

        if (stateInfo.IsName("ShootLGun"))
        {

            offset.y = 100;

        }
        else {

            offset.y = 0;

        }

        LRt.offsetMax = offset;
    }
}
