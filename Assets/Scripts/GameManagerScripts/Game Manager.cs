using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject gameOverPanel;
    public Text gameOverText;
    public GameObject stopPanel;

    [Header("Food")]
    [SerializeField] private Text foodText;
    //private Canvas canvas;
    private MyTime myTime;
    private bool isShowMenu = true;

    private void Start()
    {
        myTime = new MyTime();
        //canvas = GetComponent<Canvas>();
        if (foodText != null)
        {
            foodText.text = "0";
        }
        if (gameOverText != null)
        {
            gameOverText.text = "";
        }

        //gameOverPanel.SetActive(false);

        //stopPanel.SetActive(false);

    }
    private void Update()
    {
        ShowMenu();
    }
    private void ShowMenu()
    {
        if (isShowMenu)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame();
                stopPanel.SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.Escape))
            {
                ResumeGame();
                stopPanel.SetActive(false);

            }
        }
        
    }
    // Funzione per mettere in pausa il gioco
    private void PauseGame()
    {

        Time.timeScale = 0; // Ferma il gioco
        myTime.Stop();

        
    }

    // Funzione per riprendere il gioco
    private void ResumeGame()
    {

        Time.timeScale = 1; // Riprendi il gioco
        myTime.Restart();

    }

    // Funzione per fermare il gioco definitivamente
    public void StopGame()
    {
        Time.timeScale = 0; // Ferma il gioco
        myTime.Stop();
        gameOverPanel.SetActive(true); // Mostra il pannello di Game Over
        //canvas.sortingOrder = 100;
        gameOverText.text = "Game Over";
        gameOverText.fontStyle = FontStyle.Bold;
        foodText.enabled = false;
        isShowMenu = false;
    }
    public void UpdateEggsText(int foodCounter)
    {
        if (foodText != null)
            foodText.text = foodCounter.ToString();

    }
}
