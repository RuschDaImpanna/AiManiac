using UnityEngine;

public class InputController : MonoBehaviour
{

    private IA_PlayerControls playerControls;

    private void Awake()
    {

        playerControls = new IA_PlayerControls();

    }

    private void OnEnable()
    {

        playerControls.Enable();

    }
    private void OnDisable()
    {

        playerControls.Disable();

    }

    public Vector2 GetMouseMove()
    {

        return playerControls.Player.Look.ReadValue<Vector2>();

    }

    
}
