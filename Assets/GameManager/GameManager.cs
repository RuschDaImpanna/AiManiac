using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public GameObject playScreen;
    public GameObject pauseScreen;
    public Text scoreText;
    public GameObject weaponCooldownLabel;

    private Text weaponCooldownText;
    private SpeedBar playerSpeedBar;
    private RecoilWeapon playerWeapon;
    private int score;
    public float lastZPosition;
    public const string highScoreKey = "HighScore";

    // Add a cooldown to start be considering danger/dead states
    private float initialCooldownTime = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find and subscribe to the SpeedBar's state change event
        playerSpeedBar = FindFirstObjectByType<SpeedBar>();

        if (playerSpeedBar != null )
        {
            playerSpeedBar.OnStateChanged += HandlePlayerStateChange;
        }
        
        // Find the player's weapon
        playerWeapon = player.GetComponentInChildren<RecoilWeapon>();

        // Load high score and initialize score
        loadHighScore();
        score = 0;
        lastZPosition = player.transform.position.z;

        // Initialize cooldown label
        weaponCooldownText = weaponCooldownLabel.GetComponent<Text>();
        weaponCooldownLabel.SetActive(false);

        // Initialize screens
        loseScreen.SetActive(false);
        playScreen.SetActive(true);
        pauseScreen.SetActive(false);

        // Bind the pause action
        InputAction pauseGameAction = player.GetComponent<PlayerInput>().actions["PauseGame"];
        pauseGameAction.performed += ctx => 
        {
            if (ctx.control.path.Contains("escape")) PauseGame();
        };
    }

    private void FixedUpdate()
    {
        if (player.transform.position.z - lastZPosition > 1)
        {
            score += 1;
            UpdateScoreUI();
            lastZPosition = player.transform.position.z;

            float weaponCooldown = playerWeapon.CurrentCooldown;

            if (weaponCooldown > 0)
            {
                if (weaponCooldownLabel.activeInHierarchy == false)
                {
                    weaponCooldownLabel.SetActive(true);
                }

                weaponCooldownText.text = weaponCooldown.ToString("F2") + "s";
            } else
            {
                if (weaponCooldownLabel.activeInHierarchy == true)
                {
                    weaponCooldownLabel.SetActive(false);
                }

                weaponCooldownText.text = "";
            }
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
    public void PauseGame()
    {
        if (IsGameOver) return;
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f; // Resume the game
            playScreen.SetActive(true);
            pauseScreen.SetActive(false);
            // Lock the cursor and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Time.timeScale = 0f; // Pause the game
            playScreen.SetActive(false);
            pauseScreen.SetActive(true);
            // Unlock the cursor and show it
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    private void ShowLoseScreen()
    {
        playScreen.SetActive(false); // Hide play screen
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
        Debug.Log("Game Over.");
        
        IsGameOver = true;
        
        if (Time.timeScale == 0f) return;
        
        Time.timeScale = 0f; // Pause the game

        // Check high score
        if (score > PlayerPrefs.GetInt(highScoreKey, 0))
        {
            PlayerPrefs.SetInt(highScoreKey, score);
            PlayerPrefs.Save();
            Debug.Log("New High Score: " + score);
        }

        ShowLoseScreen();

        // Cleanup
        if (playerSpeedBar != null)
        {
            playerSpeedBar.OnStateChanged -= HandlePlayerStateChange;
        }

        score = 0;
    }
}
