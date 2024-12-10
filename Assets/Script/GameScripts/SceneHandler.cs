using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class SceneHandler : MonoBehaviour
{
    #region Public Varibles
    public GameObject loading; // Loading Canvas
    public Transform eggs; // Eggs transform in loading Canvas
    public MenuNavigation menuNavigation; 
    #endregion
    #region Private Variables
    private string levelToLoad = ""; 
    private LoadSceneMode loadSceneMode = LoadSceneMode.Single;
    AsyncOperation operation;
    #endregion

    #region Cycle Life
    // Set the new scene to load
    private void Start()
    {
        levelToLoad = menuNavigation.LevelToLoad;
        operation = SceneManager.LoadSceneAsync(levelToLoad, loadSceneMode);
        operation.allowSceneActivation = false;
    }

    void Update()
    {
        LoadSceneWithKey();
    }
    #endregion

    #region Private Methods
    // Method for changing the scene with conditions 
    private void LoadSceneWithKey()
    {
        bool readyToChangeScene = loading.activeSelf && Input.anyKeyDown && eggs.childCount == 0;

        if (readyToChangeScene)
        {
            operation.allowSceneActivation = true;
        }
    }
    #endregion
}
