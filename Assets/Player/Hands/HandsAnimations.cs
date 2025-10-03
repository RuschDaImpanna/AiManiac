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


    }
}
