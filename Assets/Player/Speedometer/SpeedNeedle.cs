using UnityEngine;

public class SpeedNeedle : MonoBehaviour
{

    private Transform needle;
    public GameManager gm;
    public SpeedBar deadLimit;

    public const float minSpeedAngle = -90;
    public const float maxSpeedAngle = 5;

    private float speed;
    private float maxSpeed;
    
    void Awake()
    {

        needle = transform.GetChild(1);
        
    }

    void FixedUpdate()
    {

        speed = gm.speed;
        maxSpeed = deadLimit.DeadSpeedLimit * 10;

        if (speed > maxSpeed)
        {

            speed = maxSpeed;

        }

        needle.eulerAngles = new Vector3(0, 0, GetSpeedRotation());
    }
    
    
    float GetSpeedRotation()
    {

        float totalAngleSize = minSpeedAngle - maxSpeedAngle;

        float speedNormalized = speed / maxSpeed;

        return minSpeedAngle - speedNormalized * totalAngleSize;

    }
}
