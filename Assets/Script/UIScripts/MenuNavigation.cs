using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class MenuNavigation : MonoBehaviour
{
    #region Public Variables
    public GameObject menu;
    public GameObject rules;
    public GameObject levels;
    public GameObject loading;
    #endregion

    #region Private Variables
    private string levelToLoad = "";
    #endregion

    public string LevelToLoad { get { return levelToLoad; } }

    #region Life Cycle
    private void Start()
    {
        // Set the main menu the first window active
        menu.SetActive(true);
    }

    private void Update()
    {
        BackToMenu();
    }
    #endregion

    #region Public Methods
    // Enter rules menu
    public void ShowRules()
    {
        rules.SetActive(true);
        menu.SetActive(false);
    }

    // Enter levels selection menu
    public void ShowLevels()
    {
        levels.SetActive(true);
        rules.SetActive(false);
        menu.SetActive(false);
    }

    // Exit to the application
    public void ExitGame()
    {
        Debug.Log("Quit the game."); // Console feedback
        Application.Quit(); // Quit the game
    }

    // Enter Loading screen
    public void LevelLoading(Button button)
    {
        loading.SetActive(true);
        levels.SetActive(false);

        levelToLoad = button.name;
    }

    public void BackToMenu()
    {
        // Permit to return the main menu window pressing the ESC key
        bool back = !menu.activeSelf && !loading.activeSelf && Input.GetKey(KeyCode.Escape);
        if (back)
        {
            menu.SetActive(true);
            rules.SetActive(false);
            levels.SetActive(false);
        }
    }
    #endregion
}
