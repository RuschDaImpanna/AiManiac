using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Enum with the different player states
public enum PlayerState
{
    Normal,
    Danger,
    Warning,
    Dead
}

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private AudioSource windSound;
    public GameObject Player
    {
        get { return player; }
    }

    [SerializeField] private GameObject loseScreen;
    [SerializeField] private GameObject playScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private Text speedText;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject weaponCooldownLabel;
    [SerializeField] private ScreenBorder screenBorder;

    private SpeedBar playerSpeedBar;
    private WeaponRecoil playerWeapon;
    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    private int score;

    private float lastZPosition;
    public float LastZPosition
    {
        get { return lastZPosition; }
        set { lastZPosition = value; }
    }

    public const string highScoreKey = "HighScore";

    private bool isGameOver = false;
    public bool IsGameOver
    {
        get { return isGameOver; }
    }

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
        InitializeGame();
    }

    private void InitializeGame()
    {
        // Find and subscribe to the SpeedBar's state change event
        playerSpeedBar = FindFirstObjectByType<SpeedBar>();

        if (playerSpeedBar != null)
        {
            playerSpeedBar.OnStateChanged += HandlePlayerStateChange;
        }

        // Find the player's weapon
        if (player != null)
        {
            playerWeapon = player.GetComponentInChildren<WeaponRecoil>();
            playerMovement = player.GetComponent<PlayerMovement>();
            playerInput = player.GetComponent<PlayerInput>();
        }

        // Load high score and initialize score
        loadHighScore();
        score = 0;

        if (player != null)
        {
            lastZPosition = player.transform.position.z;
        }

        // Initialize screens
        if (loseScreen != null) loseScreen.SetActive(false);
        if (playScreen != null) playScreen.SetActive(true);
        if (pauseScreen != null) pauseScreen.SetActive(false);

        // Bind the pause action
        if (playerInput != null)
        {
            InputAction pauseGameAction = playerInput.actions["PauseGame"];
            pauseGameAction.performed += OnPauseGamePerformed;
        }
    }

    private void OnPauseGamePerformed(InputAction.CallbackContext ctx)
    {
        if (ctx.control.path.Contains("escape"))
        {
            PauseGame();
        }
    }

    private void FixedUpdate()
    {
        // Check for null references
        if (player == null || playerSpeedBar == null) return;

        // Update score UI
        if (player.transform.position.z - lastZPosition > 1)
        {
            score += 1;
            UpdateScoreUI();
            lastZPosition = player.transform.position.z;
        }

        // Update speed UI
        speed = playerSpeedBar.Speed * 3.6f;

        if (speedText != null)
        {
            speedText.text = speed.ToString("F1");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reset time scale when a new scene is loaded
        Time.timeScale = 1f;
        isGameOver = false;

        // Cleanup old subscriptions
        CleanupSubscriptions();

        // Re-initialize everything after scene load
        // Use Invoke to wait one frame for all objects to be ready
        Invoke(nameof(InitializeGame), 0.1f);

        if (windSound != null && !windSound.isPlaying)
        {
            windSound.UnPause();
        }
    }

    void OnDestroy()
    {
        Debug.Log("GameManager destruido!");
        CleanupSubscriptions();
    }

    private void CleanupSubscriptions()
    {
        // Limpiar suscripciones
        if (playerSpeedBar != null)
        {
            playerSpeedBar.OnStateChanged -= HandlePlayerStateChange;
        }

        // Limpiar suscripción del input
        if (playerInput != null)
        {
            InputAction pauseGameAction = playerInput.actions["PauseGame"];
            if (pauseGameAction != null)
            {
                pauseGameAction.performed -= OnPauseGamePerformed;
            }
        }
    }

    public void PauseGame()
    {
        if (isGameOver) return;

        // Check for null references
        if (playScreen == null || pauseScreen == null || windSound == null)
        {
            Debug.LogWarning("Cannot pause game - missing references");
            return;
        }

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
        if (playScreen == null || loseScreen == null) return;

        playScreen.SetActive(false); // Hide play screen
        loseScreen.SetActive(true); // Show lose screen

        // Unlock the cursor and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Update lose screen with final score and high score
        int highScore = PlayerPrefs.GetInt(highScoreKey, 0);
        LoseScreen loseScreenComponent = loseScreen.GetComponent<LoseScreen>();
        if (loseScreenComponent != null)
        {
            loseScreenComponent.UpdateScores(score, highScore);
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game...");

        // Cleanup before restarting
        CleanupSubscriptions();

        isGameOver = false;
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

        // Check for null references
        if (speedText == null || screenBorder == null) return;

        switch (newState)
        {
            case PlayerState.Normal:
                Debug.Log("Player is in Normal state.");
                speedText.color = new Color(0, 1, 0.9f);
                screenBorder.SetNormal();
                break;
            case PlayerState.Warning:
                Debug.Log("Player is in Warning state.");
                speedText.color = Color.yellow;
                screenBorder.SetWarning(fastTransition: true);
                break;
            case PlayerState.Danger:
                Debug.Log("Player is in Danger state.");
                speedText.color = Color.red;
                screenBorder.SetDanger(fastTransition: true);
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

        isGameOver = true;

        if (Time.timeScale == 0f) return;

        Time.timeScale = 0f; // Pause the game

        if (windSound != null)
        {
            windSound.Pause();
        }

        // Check high score
        if (score > PlayerPrefs.GetInt(highScoreKey, 0))
        {
            PlayerPrefs.SetInt(highScoreKey, score);
            PlayerPrefs.Save();
            Debug.Log("New High Score: " + score);
        }

        ShowLoseScreen();

        // Cleanup
        CleanupSubscriptions();

        score = 0;
    }
}