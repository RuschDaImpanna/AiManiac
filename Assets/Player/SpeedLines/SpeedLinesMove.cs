using UnityEngine;

public class SpeedLinesMove : MonoBehaviour
{
    private SpeedBar playerSpeedBar;
    private ParticleSystem speedLines;
    private ParticleSystem.EmissionModule emission;

    private float xParentRotation;

    [SerializeField]
    private float intensityFactor = 1f;
    void Start()
    {
        // Find and subscribe to the SpeedBar's state change event
        playerSpeedBar = FindFirstObjectByType<SpeedBar>();

        speedLines = GetComponent<ParticleSystem>();
        emission = speedLines.emission;

        xParentRotation = transform.parent.gameObject.transform.rotation.eulerAngles.x;

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(
            -xParentRotation, 
            180f, 
            0f
        );

        // Adjust emission rate based on speed
        emission.rateOverTime = (playerSpeedBar.Speed * 3.6f - 100f) * intensityFactor; // Multiply by factor to tune intensity
    }
}
