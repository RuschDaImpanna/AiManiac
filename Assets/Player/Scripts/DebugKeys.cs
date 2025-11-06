using UnityEngine;
using UnityEngine.InputSystem;

public class DebugKeys : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    
    private GameObject player;
    private Rigidbody rb;
    void Start()
    {
        InputAction debugAction = GetComponentInParent<PlayerInput>().actions["Debug"];
        debugAction.performed += OnDebug;

        player = gameManager.player;
        rb = player.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDebug(InputAction.CallbackContext context)
    {
        Debug.Log("Debug key pressed: " + context.control.name);

        var control = context.control;

        if (control.name == "f1")
        {
            gameManager.RestartGame();
        }
        else if (control.name == "f2")
        {
            rb.linearVelocity += new Vector3(0f, -10f, 5f);
        }
        else if (control.name == "f3")
        {
            rb.linearVelocity += new Vector3(0f, 0f, -5f);
        }
    }
}
