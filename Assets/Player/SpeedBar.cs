using UnityEngine;
using System;

// Enum with the different player states
public enum PlayerState
{
    Normal,
    Danger,
    Dead
}

public class SpeedBar : MonoBehaviour
{
    public event Action<PlayerState> OnStateChanged; // Event for state changes

    private float deadSpeedLimit = 0f;
    private float dangerSpeedLimit = 0f;
    private PlayerState currentState = PlayerState.Normal;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        OnStateChanged += StateChangeHandler;
    }

    private void FixedUpdate()
    {
        float speed = rb.linearVelocity.magnitude;

        PlayerState newState = currentState;

        if (speed < deadSpeedLimit)
        {
            newState = PlayerState.Dead;
        } else if (speed < dangerSpeedLimit)
        {
            newState = PlayerState.Danger;
        }

        if (newState != currentState)
        {
            currentState = newState;
            OnStateChanged?.Invoke(currentState);
        }

        if (speed / 2 > deadSpeedLimit)
        {
            deadSpeedLimit = speed / 2;
            dangerSpeedLimit = speed - deadSpeedLimit / 2;
        }
    }

    private void StateChangeHandler(PlayerState newState)
    {
        if (newState == PlayerState.Normal)
        {
            Debug.Log("Continua así.");
        }
        else if (newState == PlayerState.Dead)
        {
            Debug.Log("Game Over.");
        } else if (newState == PlayerState.Danger)
        {
            Debug.Log("Ten cuidado, te estan alcanzando.");
            // TODO: Trigger a cronometer to see how long the player stays in danger, and if it exceeds a certain time, trigger Game Over.
        }
    }

}
