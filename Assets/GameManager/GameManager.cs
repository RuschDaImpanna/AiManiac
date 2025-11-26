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
    [SerializeField] private AudioSource snowboardingSound;
    [SerializeField] private AudioSource startmusicSound;

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
            speedText.text = $"{speed:F1}km/h";
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        isGameOver = false;

        // Cleanup old subscriptions
        CleanupSubscriptions();

        // Re-initialize everything after scene load
        // Use Invoke to wait one frame for all objects to be ready
        Invoke(nameof(InitializeGame), 0.1f);

        UnpauseAllSounds();
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

        // Limpiar suscripci�n del input
        if (playerInput != null)
        {
            InputAction pauseGameAction = playerInput.actions["PauseGame"];
            if (pauseGameAction != null)
            {
                pauseGameAction.performed -= OnPauseGamePerformed;
            }
        }
    }

    // ------------------------- AUDIO CONTROL -------------------------

    private void PauseAllSounds()
    {
        windSound.Pause();
        snowboardingSound.Pause();
        startmusicSound.Pause();
    }

    private void UnpauseAllSounds()
    {
        windSound.UnPause();
        snowboardingSound.UnPause();
        startmusicSound.UnPause();
    }

    // -----------------------------------------------------------------

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
            Time.timeScale = 1f;
            playScreen.SetActive(true);
            pauseScreen.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            UnpauseAllSounds();
        }
        else
        {
            Time.timeScale = 0f;
            playScreen.SetActive(false);
            pauseScreen.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            PauseAllSounds();
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
                speedText.color = new Color(0, 1, 0.9f);
                screenBorder.SetNormal();
                break;

            case PlayerState.Warning:
                speedText.color = Color.yellow;
                screenBorder.SetWarning(fastTransition: true);
                break;

            case PlayerState.Danger:
                speedText.color = Color.red;
                screenBorder.SetDanger(fastTransition: true);
                break;

            case PlayerState.Dead:
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

        PauseAllSounds();

        if (score > PlayerPrefs.GetInt(highScoreKey, 0))
        {
            PlayerPrefs.SetInt(highScoreKey, score);
            PlayerPrefs.Save();
        }

        ShowLoseScreen();

        // Cleanup
        CleanupSubscriptions();

        score = 0;
    }
}