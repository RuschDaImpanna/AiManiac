using UnityEngine;
using UnityEngine.SceneManagement;

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
    public GameObject loseScreen;

    private SpeedBar playerSpeedBar;
    private int score;
    private float lastZPosition;
    public const string highScoreKey = "HighScore";

    // Add a cooldown to start be considering danger/dead states
    private float initialCooldownTime = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find and subscribe to the SpeedBar's state change event
        SpeedBar playerSpeedBar = FindFirstObjectByType<SpeedBar>();

        if (playerSpeedBar != null )
        {
            playerSpeedBar.OnStateChanged += HandlePlayerStateChange;
        }

        // Load high score and initialize score
        loadHighScore();
        score = 0;
        lastZPosition = player.transform.position.z;

        // Initialize lose screen
        loseScreen.SetActive(false);
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

    private void ShowLoseScreen()
    {
        loseScreen.SetActive(true); // Show lose screen

        // Unlock the cursos and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Update lose screen with final score and high score
        int highScore = PlayerPrefs.GetInt(highScoreKey, 0);
        loseScreen.GetComponent<LoseScreen>().UpdateScores(score, highScore);
    }

    public static bool IsGameOver = false;

    public void RestartGame()
    {
        Debug.Log("Restarting game...");

        IsGameOver = false;
        Time.timeScale = 1f; // Resume the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
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
        IsGameOver = true;
        Debug.Log("Game Over triggered by GameManager.");
        // Implement game over logic here (e.g., show game over screen, restart level, etc.)

        if (Time.timeScale == 0f) return;

        Time.timeScale = 0f; // Pause the game
        ShowLoseScreen();

        if (playerSpeedBar != null)
        {
            playerSpeedBar.OnStateChanged -= HandlePlayerStateChange;
        }

        // Check high score
        if (score > PlayerPrefs.GetInt(highScoreKey, 0))
        {
            PlayerPrefs.SetInt(highScoreKey, score);
            PlayerPrefs.Save();
            Debug.Log("New High Score: " + score);
        }

        // Clean up score
        score = 0;
    }
}
