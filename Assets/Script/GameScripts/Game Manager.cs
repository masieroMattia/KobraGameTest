using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Public Variables
    [Header("References Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverText;

    [Header("References Menu Game")]
    [SerializeField] private GameObject menuGamePanel;
    [SerializeField] private Text resumeText;
    [SerializeField] private Text menuGameText;

    [Header("References Eggs Counter")]
    [SerializeField] private TextMeshProUGUI eggsCounterText;

    [Header("Game Over UI Settings")]
    [Tooltip("seconds")][SerializeField] private float gameOverTransitionTime = 2f;
    [SerializeField] private Color colorGameOverPanel = Color.red;
    [SerializeField] private Color colorGameOverText = Color.black;
    [SerializeField] private FontStyles fontStyleGameOverText = FontStyles.Bold;
    [SerializeField] private int fontSizeGameOverText = 60;

    [Header("Menu Game UI Settings")]
    [SerializeField] private Color colorMenuGamePanel = Color.red;
    [SerializeField] private Color colorResumeText = Color.black;
    [SerializeField] private Color colorMenuGameText = Color.black;
    [SerializeField] private FontStyle fontStyleMainMenuText = FontStyle.Bold;
    [SerializeField] private FontStyle fontStyleResumeText = FontStyle.Bold;
    [SerializeField] private int fontSizeMainMenuText = 60;
    [SerializeField] private int fontSizeResumeText = 60;
    [Tooltip("seconds")][SerializeField] private float resumeTransitionTime = 2f;

    [Header("Egg UI Settings")]
    [SerializeField] private Color colorEggsCounterText = Color.black;
    [SerializeField] private FontStyles fontStyleEggsCounterText = FontStyles.Bold;

    [HideInInspector] public bool isGameOverTransitioning = false;
    [HideInInspector] public bool isResumeTransitioning = false;
    #endregion

    #region Private Variables
    private ColorManager colorManager;
    private MyTime myTime;
    private AudioManager audioManager;

    private bool isShowMenu = true;
    private float elapsedTime = 0f;
    #endregion

    #region Lifecycle
    private void Start()
    {
        ValidateReferences();

        myTime = new MyTime();
        colorManager = new ColorManager();
        audioManager = FindObjectOfType<AudioManager>();

        if (!audioManager) Debug.LogError("Audio Manager not found!");

        ResetUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) StopGame();
        if (Input.GetKeyDown(KeyCode.F1)) ResumeGame();

        HandleTransition(isGameOverTransitioning, gameOverTransitionTime, ShowGameOver);
        HandleTransition(isResumeTransitioning, resumeTransitionTime, ResumeTime);
    }
    #endregion

    #region Private Methods
    private void ResetUI()
    {
        gameOverPanel.SetActive(false);
        menuGamePanel.SetActive(false);
        gameOverText.text = "";
        eggsCounterText.text = "0";
        eggsCounterText.enabled = true;
    }

    private void ValidateReferences()
    {
        if (!gameOverText || !gameOverPanel || !resumeText || !eggsCounterText)
        {
            Debug.LogError("One or more UI references are missing!");
        }
    }

    private void HandleTransition(bool isTransitioning, float transitionTime, System.Action onComplete)
    {
        if (isTransitioning)
        {
            elapsedTime += Time.unscaledDeltaTime;
            if (elapsedTime >= transitionTime)
            {
                elapsedTime = 0f;
                isTransitioning = false;
                onComplete.Invoke();
            }
        }
    }

    private void ShowMenu()
    {
        ToggleUIElements(true, false, false);

        colorManager.ApplyColorToPanel(menuGamePanel, colorMenuGamePanel);
        resumeText.color = colorResumeText;
        menuGameText.color = colorMenuGameText;

        resumeText.fontStyle = fontStyleResumeText;
        menuGameText.fontStyle = fontStyleMainMenuText;
    }

    private void ShowGameOver()
    {
        ToggleUIElements(false, true, false);

        colorManager.ApplyColorToPanel(gameOverPanel, colorGameOverPanel);
        gameOverText.color = colorGameOverText;
        gameOverText.text = "Game Over";
        gameOverText.fontStyle = fontStyleGameOverText;
        gameOverText.fontSize = fontSizeGameOverText;
    }

    private void ToggleUIElements(bool isMenuVisible, bool isGameOverVisible, bool areEggsVisible)
    {
        menuGamePanel.SetActive(isMenuVisible);
        gameOverPanel.SetActive(isGameOverVisible);
        eggsCounterText.enabled = areEggsVisible;
    }
    #endregion

    #region Public Methods
    public void ResumeGame()
    {
        isResumeTransitioning = true;
        ToggleUIElements(false, false, true);
    }

    private void ResumeTime()
    {
        Time.timeScale = 1;
        myTime.Restart();
        if (audioManager) audioManager.PlayClipAtPoint(audioManager.level1, audioManager.BGM, true, true);
    }

    public void StopGame()
    {
        Time.timeScale = 0;
        myTime.Stop();
        if (audioManager) audioManager.StopClip(audioManager.level1);
        ShowMenu();
    }

    public void GameOver()
    {
        if (audioManager)
        {
            audioManager.StopClip(audioManager.level1);
            audioManager.PlayClipAtPoint(audioManager.deathScreen, audioManager.BGM, true, false);
        }

        elapsedTime = 0;
        isGameOverTransitioning = true;
        StopGame();
    }

    public void UpdateEggsText(int eggsCounter)
    {
        if (eggsCounterText)
        {
            eggsCounterText.fontStyle = fontStyleEggsCounterText;
            eggsCounterText.color = colorEggsCounterText;
            eggsCounterText.text = eggsCounter.ToString();
        }
    }
    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to Main Menu");
    }
    #endregion
}
