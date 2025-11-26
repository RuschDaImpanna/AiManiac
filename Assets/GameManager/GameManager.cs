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
        playerSpeedBar = FindFirstObjectByType<SpeedBar>();

        if (playerSpeedBar != null)
        {
            playerSpeedBar.OnStateChanged += HandlePlayerStateChange;
        }

        playerWeapon = player.GetComponentInChildren<WeaponRecoil>();
        playerMovement = player.GetComponent<PlayerMovement>();

        loadHighScore();
        score = 0;
        lastZPosition = player.transform.position.z;

        loseScreen.SetActive(false);
        playScreen.SetActive(true);
        pauseScreen.SetActive(false);

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
        }

        speed = playerSpeedBar.Speed * 3.6f;
        speedText.text = speed.ToString("F1");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
        isGameOver = false;

        UnpauseAllSounds();
    }

    void OnDestroy()
    {
        if (playerSpeedBar != null)
        {
            playerSpeedBar.OnStateChanged -= HandlePlayerStateChange;
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
        playScreen.SetActive(false);
        loseScreen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        int highScore = PlayerPrefs.GetInt(highScoreKey, 0);
        loseScreen.GetComponent<LoseScreen>().UpdateScores(score, highScore);
    }

    public void RestartGame()
    {
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
        isGameOver = true;

        if (Time.timeScale == 0f) return;

        Time.timeScale = 0f;

        PauseAllSounds();

        if (score > PlayerPrefs.GetInt(highScoreKey, 0))
        {
            PlayerPrefs.SetInt(highScoreKey, score);
            PlayerPrefs.Save();
        }

        ShowLoseScreen();

        if (playerSpeedBar != null)
        {
            playerSpeedBar.OnStateChanged -= HandlePlayerStateChange;
        }

        score = 0;
    }
}
