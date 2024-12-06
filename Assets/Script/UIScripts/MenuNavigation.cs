using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MenuNavigation : MonoBehaviour
{
    #region Public Variables
    public GameObject menu;
    public GameObject rules;
    public GameObject levels;
    #endregion

    #region Life Cycle
    private void Start()
    {
        // Set the main menu the first window active
        menu.SetActive(true);
    }

    private void Update()
    {
        // Permit to return the main menu window pressing the ESC key
        bool back = !menu.activeSelf && Input.GetKey(KeyCode.Escape);
        if (back)
        {
            menu.SetActive(true);
            rules.SetActive(false);
            levels.SetActive(false);
        }
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
    #endregion
}
