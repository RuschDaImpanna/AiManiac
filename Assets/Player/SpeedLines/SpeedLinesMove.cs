using UnityEngine;

public class SpeedLinesMove : MonoBehaviour
{
    private Rigidbody rbPlayer;
    private SpeedBar playerSpeedBar;
    private float speed;

    void Start()
    {
        // Find and subscribe to the SpeedBar's state change event
        playerSpeedBar = FindFirstObjectByType<SpeedBar>();

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }
}
