using UnityEngine;
using System;

public class SpeedBar : MonoBehaviour
{
    public event Action<PlayerState> OnStateChanged; // Event for state changes

    [SerializeField] private float deadSpeedLimit;
    public float DeadSpeedLimit { 
        get { return deadSpeedLimit; } 
    }

    private float dangerSpeedLimit = 0f;
    private float warningSpeedLimit = 0f;
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
        } else if (speed < warningSpeedLimit) {
            newState = PlayerState.Warning;
        } else
        {
            newState = PlayerState.Normal;
        }

        if (newState != currentState)
        {
            currentState = newState;
            OnStateChanged?.Invoke(currentState);
        }

        if (speed * 0.5f > deadSpeedLimit)
        {
            deadSpeedLimit = speed * 0.5f;
            dangerSpeedLimit = speed * 0.65f;
            warningSpeedLimit = speed * 0.8f;
        }
    }
}
