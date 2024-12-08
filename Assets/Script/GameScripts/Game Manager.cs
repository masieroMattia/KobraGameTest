using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Public variables
    [Header("References Game Over")]
    [SerializeField] private GameObject gameOverPanel; // Game Over Panel GameObject
    [SerializeField] private TextMeshProUGUI gameOverText; // Text for writing game over phrase
    [Header("References Stop Game")]
    [SerializeField] private GameObject stopPanel; //Stop Panel GameObject
    [Header("References Eggs Counter")]
    [SerializeField] private TextMeshProUGUI eggsText; // Text for writing Eggs Counter

    [Header("Game Over UI Settings")]
    [Tooltip("seconds")]
    [SerializeField] private float gameOverTransitionTime = 2f; // Time for the game-over transition (in seconds)
    [SerializeField] private Color colorGameOverPanel = Color.red; // Game Over Panel color
    [SerializeField] private Color colorGameOverText = Color.black; // Game Over Text color
    [SerializeField] private FontStyles fontStyleGameOverText = FontStyles.Bold; // Game Over Text Font
    [HideInInspector] public bool isGameOverTransitioning = false; // Flag to track the transition


    [Header("Egg UI Settings")]
    [SerializeField] private Color colorEggsText = Color.black; // Game Over Text color
    [SerializeField] private FontStyles fontStyleEggsText = FontStyles.Bold; // Game Over Text Font
    [SerializeField] private Color colorEggsPanel = Color.red; // Game Over Panel color
    #endregion


    #region Private Variables
    private ColorManager colorManager; // ColorManager class reference
    private MyTime myTime; // MyTime class reference
    private AudioManager audioManager; // Audio Manager class reference

    private Canvas canvas; // Canvas reference

    private bool isShowMenu = true; // Show the stop panel
    private float elapsedTime = 0f; // Time elapsed for transition
    private float startTransitionTime = 0f; // Track when the transition starts

    #endregion


    #region Lifecycle
    private void Start()
    {
        
        // Instatiate the Time
        myTime = new MyTime();

        // Instatiate the Color Manager
        colorManager = new ColorManager();

        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            Debug.Log("Audio Manager trovato");
        }
        if (audioManager == null)
        {
            Debug.LogError("Audio Manager non trovato");
        }

        // Getting the Cavas Component
        canvas = GetComponent<Canvas>();
        if (gameOverText == null)
        {
            Debug.LogError("gameOverText is not assigned in the Inspector!");
            return;
        }
        if (gameOverPanel == null)
        {
            Debug.LogError("gameOverText is not assigned in the Inspector!");
            return;
        }
        if (stopPanel == null)
        {
            Debug.LogError("gameOverText is not assigned in the Inspector!");
            return;
        }
        if (eggsText == null)
        {
            Debug.LogError("gameOverText is not assigned in the Inspector!");
            return;
        }
        if (eggsText != null) // Check if is not null
        {
            eggsText.text = "0"; // Set Eggs Text to '0' at the Start
        }
        if (gameOverText != null) // Check if is not null
        {
            gameOverText.text = ""; // Set Game Over Text empty at the Start
        }

        gameOverPanel.SetActive(false); // Hide the Game Over Panel

        stopPanel.SetActive(false); // Hide the Stop Panel

    }
    private void Update()
    {
        ShowMenu();
        if (isGameOverTransitioning)
        {
            // Increment elapsed time with unscaled delta time (for consistent timing even when game is paused)
            elapsedTime += Time.unscaledDeltaTime;
            // If transition is complete, show the Game Over panel
            if (elapsedTime >= gameOverTransitionTime)
            {

                isGameOverTransitioning = false;
                ShowGameOver();
            }
        }
    }
    #endregion


    #region Private method
    private void ShowMenu()
    {
        if (!isShowMenu) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StopGame();
            stopPanel.SetActive(true); // Active to see the Stop Panel
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            ResumeGame();
            stopPanel.SetActive(false); // Hide the Stop Panel
        }

    } // Function that manage the Menu Visibility

    private void ResumeGame()
    {
        Time.timeScale = 1; // Resume the Game Time
        myTime.Restart(); // Resume the Class Time 

    } // Function tha Resume the Game

    private void StopGame()
    {
        Time.timeScale = 0; // Stop the Game Time
        myTime.Stop(); // Stop the Class Time
    } // Function tha Stop the Game

    public void GameOver()
    {
        // Stop level1 background music
        audioManager.StopClip(audioManager.level1);

        // Play deathScreen sound
        audioManager.PlayClipAtPoint(audioManager.deathScreen,audioManager.BGM, true, true);
        startTransitionTime = Time.realtimeSinceStartup; // Record the time when game over is triggered
        elapsedTime = 0;
        isGameOverTransitioning = true;
        Debug.Log(isGameOverTransitioning.ToString());
        StopGame();

    } // Function that handle the game over and its transitioning

    public void ShowGameOver()
    {
        Debug.Log("Eseguendo ShowGameOver");
        colorManager.ApplyColorToPanel(gameOverPanel, colorGameOverPanel); // Apply color to the Panel
        gameOverText.color = colorGameOverText; // Apply color to the Text
        gameOverPanel.SetActive(true); // Active to see the Game Over Panel
        Debug.Log("Game Over Panel attivato.");
        gameOverText.text = "Game Over"; // Printing Game Over
        gameOverText.fontStyle = fontStyleGameOverText; // Setting the Game Over Text Font Style
        gameOverText.fontSize = 20; // Setting the Game Over Text Font Size
        eggsText.enabled = false; // Hide the Egg Text Counter
        isShowMenu = false; // Hide the Stop and Resume Panel
    } // Function that set the graphic setting for the Game Over 

    #endregion


    #region Public method
    public void UpdateEggsText(int eggsCounter)
    {
        
        if (eggsText != null)// Check if is not null
            eggsText.text = eggsCounter.ToString(); // Printing Egg Counter

    } // Function that track and update the Eggs Text counter
    #endregion









}
