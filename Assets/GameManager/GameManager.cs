using UnityEngine;

// Enum with the different player states
public enum PlayerState
{
    Normal,
    Danger,
    Dead
}

public class GameManager : MonoBehaviour
{
    public GameObject player;

    private SpeedBar playerSpeedBar;
    private int score;
    private float lastZPosition;
    public const string highScoreKey = "HighScore";

    // Add a cooldown to start be considering danger/dead states
    private float initialCooldownTime = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpeedBar playerSpeedBar = FindFirstObjectByType<SpeedBar>();

        if (playerSpeedBar != null )
        {
            playerSpeedBar.OnStateChanged += HandlePlayerStateChange;
        }

        loadHighScore();
        score = 0;
        lastZPosition = player.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (player.transform.position.z - lastZPosition > 1)
        {
            score += 1;
            lastZPosition = player.transform.position.z;
        }

        Debug.Log("Score: " + score);
    }

    private void loadHighScore()
    {
        int highScore = PlayerPrefs.GetInt(highScoreKey, 0);
        Debug.Log("High Score: " + highScore);
    }

    private void HandlePlayerStateChange(PlayerState newState)
    {
        if (Time.timeSinceLevelLoad < initialCooldownTime) return;

        switch (newState)
        {
            case PlayerState.Normal:
                Debug.Log("Player is in Normal state.");
                // Handle normal state logic here
                break;
            case PlayerState.Danger:
                Debug.Log("Player is in Danger state.");
                // Handle danger state logic here
                break;
            case PlayerState.Dead:
                Debug.Log("Player is Dead.");
                // Handle dead state logic here
                GameOver();
                break;
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over triggered by GameManager.");
        // Implement game over logic here (e.g., show game over screen, restart level, etc.)

        if (Time.timeScale == 0f) return;

        Time.timeScale = 0f; // Pause the game

        if (playerSpeedBar != null)
        {
            playerSpeedBar.OnStateChanged -= HandlePlayerStateChange;
        }

        // Optionally, you can also reset the score or perform other cleanup tasks here
        if (score > PlayerPrefs.GetInt(highScoreKey, 0))
        {
            PlayerPrefs.SetInt(highScoreKey, score);
            PlayerPrefs.Save();
            Debug.Log("New High Score: " + score);
        }

        score = 0;
    }
}
