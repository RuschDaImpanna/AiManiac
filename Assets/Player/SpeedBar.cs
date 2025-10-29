using UnityEngine;
using System;

public class SpeedBar : MonoBehaviour
{
    public event Action<PlayerState> OnStateChanged; // Event for state changes

    private float deadSpeedLimit = 0f;
    private float dangerSpeedLimit = 0f;
    private PlayerState currentState = PlayerState.Normal;
    private float speed = 0f;
    public float Speed { get { return speed; } }

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on this GameObject!");
        }
    }

    private void FixedUpdate()
    {
        speed = Mathf.Sqrt(
            Mathf.Pow(rb.linearVelocity.y, 2) 
            + Mathf.Pow(rb.linearVelocity.z, 2)
        );

        PlayerState newState = currentState;

        if (speed < deadSpeedLimit)
        {
            newState = PlayerState.Dead;
        } else if (speed < dangerSpeedLimit)
        {
            newState = PlayerState.Danger;
        } else
        {
            newState = PlayerState.Normal;
        }

        if (newState != currentState)
        {
            currentState = newState;
            OnStateChanged?.Invoke(currentState);
        }

        if (speed / 2 > deadSpeedLimit)
        {
            deadSpeedLimit = speed / 2;
            //Debug.Log("Updated deadSpeedLimit to: " + deadSpeedLimit * 3.6);
            dangerSpeedLimit = speed - deadSpeedLimit / 2;
        }
    }
}
