using UnityEngine;
using UnityEngine.InputSystem;

public class HandsAnimations : MonoBehaviour
{

    public GameObject player;

    private InputController input;

    private GameObject LGun;
    private GameObject RGun;

    private Animator LAnimator;
    private Animator RAnimator;

    public RectTransform LRt;


    void Awake()
    {

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

        //Asigna los booleanos según lo que se presionó y detecto InputController del player
        bool LAttack = input.LShoot();
        bool RAttack = input.RShoot();

        LAnimator.SetBool("Shoot", LAttack);
        RAnimator.SetBool("Shoot", RAttack);
        
        AnimatorStateInfo stateInfo = LAnimator.GetCurrentAnimatorStateInfo(0);
        Vector2 offset = LRt.offsetMax;
        
        AnimatorClipInfo[] clipInfo = LAnimator.GetCurrentAnimatorClipInfo(0);

if (clipInfo.Length > 0)
{
    string clipName = clipInfo[0].clip.name;
    Debug.Log("Se está reproduciendo el clip: " + clipName);
}

        if (stateInfo.IsName("ShootLGun"))
        {

            offset.y = 30;

        }
        else if (stateInfo.IsName("IdleLGun"))
        {

            offset.y = 0;

        }

        LRt.offsetMax = offset;
    }
}
