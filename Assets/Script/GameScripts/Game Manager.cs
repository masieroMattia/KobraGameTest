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
    [SerializeField] private TextMeshProUGUI retryText;
    [SerializeField] private TextMeshProUGUI mainMenuText;

    
    [Header("References Eggs Counter")]
    [SerializeField] private TextMeshProUGUI eggsCounterText;

    [Header("Game Over UI Settings")]
    [Tooltip("seconds")][SerializeField] private float gameOverTransitionTime = 2f;
    [SerializeField] private Color colorGameOverPanel = Color.red;
    [SerializeField] private Color colorGameOverText = Color.black;
    [SerializeField] private FontStyles fontStyleGameOverText = FontStyles.Bold;
    [SerializeField] private int fontSizeGameOverText = 60;

    [Header("Menu Game UI Settings")]
    [SerializeField] private Color colorRetryText = Color.black;
    [SerializeField] private Color colorMainMenuText = Color.black;
    [SerializeField] private FontStyles fontStyleMainMenuText = FontStyles.Bold;
    [SerializeField] private FontStyles fontStyleRetryText = FontStyles.Bold;
    [SerializeField] private int fontSizeMainMenuText = 60;
    [SerializeField] private int fontSizeRetryText = 60;
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

    private bool isStop = false;
    private bool isGameOver = false;
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
        Debug.Log(isResumeTransitioning.ToString());
        if (Input.GetKeyDown(KeyCode.Escape) && isStop == false && isGameOver == false && isResumeTransitioning == false) StopGame(); 

        else if (Input.GetKeyDown(KeyCode.Escape) && isStop == true && isGameOver == false && isResumeTransitioning == false)
        {
            ResumeGame();
        }

        HandleTransition(ref isResumeTransitioning, resumeTransitionTime, ResumeTime);
        HandleTransition(ref isGameOverTransitioning, gameOverTransitionTime, ShowGameOver);
    }
    #endregion

    #region Private Methods
    private void ResetUI()
    {
        ToggleUIElements(false, false, true, false, false);
        eggsCounterText.text = "0";
        

    }

    private void ValidateReferences()
    {
        if (!gameOverText || !gameOverPanel || !retryText || !eggsCounterText ||!mainMenuText)
        {
            Debug.LogError("One or more UI references are missing!");
        }
    }

    private void HandleTransition(ref bool isTransitioning, float transitionTime, System.Action onComplete)
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
        ToggleUIElements(true,false,false,true,true);

        retryText.color = colorRetryText;
        mainMenuText.color = colorMainMenuText;

        retryText.text = "Retry";
        mainMenuText.text = "Main Menu";
        retryText.fontStyle = fontStyleRetryText;
        mainMenuText.fontStyle = fontStyleMainMenuText;
    }

    private void ShowGameOver()
    {
        ToggleUIElements(true, true, false,true,true);
        colorManager.ApplyColorToPanel(gameOverPanel, colorGameOverPanel);
        gameOverText.color = colorGameOverText;
        gameOverText.text = "Game Over";
        retryText.text = "Retry";
        mainMenuText.text = "Menu";
        gameOverText.fontStyle = fontStyleGameOverText;
        gameOverText.fontSize = fontSizeGameOverText;
    }

    private void ToggleUIElements(bool isGameOverVisible, bool isGameOverTextVisible, bool areEggsVisible, bool isRetryVisible, bool isMainMenuVisible)
    {
        gameOverPanel.SetActive(isGameOverVisible);
        gameOverText.enableAutoSizing = isGameOverTextVisible;
        eggsCounterText.enabled = areEggsVisible;
        retryText.enabled = isRetryVisible;
        mainMenuText.enabled= isMainMenuVisible;
    }
    #endregion

    #region Public Methods
    public void ResumeGame()
    {
        Debug.Log(isStop.ToString());
        isStop = false;
        isResumeTransitioning = true;
        ToggleUIElements(false,false, true,false,false);

    }

    private void ResumeTime()
    {
        Time.timeScale = 1;
        myTime.Restart();
        if (audioManager) audioManager.PlayClipAtPointOneTime(audioManager.level1, audioManager.BGM, true, true);
    }

    public void StopGame()
    {
        Debug.Log(isStop.ToString());
        isStop = true;
        Time.timeScale = 0;
        myTime.Stop();
        if (audioManager) audioManager.StopClip(audioManager.level1);
        if (isGameOverTransitioning == true) ToggleUIElements(false,false, false, false, false);
        else ShowMenu();
    }

    public void GameOver()
    {
        if (audioManager)
        {
            audioManager.StopClip(audioManager.level1);
            audioManager.PlayClipAtPoint(audioManager.deathScreen, audioManager.BGM, true, false);
        }

        elapsedTime = 0;
        isGameOver = true;
        isGameOverTransitioning = true;
        StopGame();
    }

    public void UpdateEggsText(int eggsCounter)
    {
        eggsCounterText.fontStyle = fontStyleEggsCounterText;
        eggsCounterText.color = colorEggsCounterText;
        eggsCounterText.text = eggsCounter.ToString();
        
    }
    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to Main Menu");
    }
    public void LoadScene(int level)
    {
        Time.timeScale = 1;
        myTime.Restart();
        LoadScenes loadScenes = new LoadScenes();
        loadScenes.LoadLevel(level);
    }
    #endregion
}
