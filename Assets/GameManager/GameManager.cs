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
    public Text speedText;
    public Text scoreText;
    public GameObject weaponCooldownLabel;

    [Header("Audio Source")]
    [SerializeField] private AudioSource windSound;

    private SpeedBar playerSpeedBar;
    private WeaponRecoil playerWeapon;
    private PlayerMovement playerMovement;
    private int score;
    private float lastZPosition;
    public float LastZPosition {
        get { return lastZPosition; }
        set { lastZPosition = value; }
    }
    public const string highScoreKey = "HighScore";

    // Add a cooldown to start be considering danger/dead states
    private float initialCooldownTime = 5f;

    public float speed;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

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
        playerWeapon = player.GetComponentInChildren<WeaponRecoil>();

        // Find the player's movement script
        playerMovement = player.GetComponent<PlayerMovement>();

        // Load high score and initialize score
        loadHighScore();
        score = 0;
        lastZPosition = player.transform.position.z;

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
        // Update score UI
        if (player.transform.position.z - lastZPosition > 1)
        {
            score += 1;
            UpdateScoreUI();
            lastZPosition = player.transform.position.z;


        }

        // Update speed UI
        speed = playerMovement.Speed*3.6f;

        speedText.text = speed.ToString("F1");

        
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset time scale when a new scene is loaded
        Time.timeScale = 1f;
        IsGameOver = false;

        if (windSound.isPlaying == false)
        {
            windSound.UnPause();
        }
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

            windSound.UnPause();
        }
        else
        {
            Time.timeScale = 0f; // Pause the game
            playScreen.SetActive(false);
            pauseScreen.SetActive(true);
            // Unlock the cursor and show it
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            windSound.Pause();
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

        // Unlock the cursor and show the cursor
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
                speedText.color = new Color(0, 1, 0.9f);
                break;
            case PlayerState.Danger:
                Debug.Log("Player is in Danger state.");
                speedText.color = Color.yellow;
                break;
            case PlayerState.Dead:
                Debug.Log("Player is Dead.");
                speedText.color = Color.red;
                GameOver();
                break;
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over.");
        
        IsGameOver = true;
        
        if (Time.timeScale == 0f) return;
        
        Time.timeScale = 0f; // Pause the game

        windSound.Pause();

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
