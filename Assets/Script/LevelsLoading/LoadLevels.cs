
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadScenes : MonoBehaviour
{
    public LoadSceneMode loadMode = LoadSceneMode.Single;
    AsyncOperation currentSceneLoad = null;
    int levelIndex;

    void Start()
    {

    }

    void Update()
    {

    }
    public void LoadLevel(int levelIndex)
    {
        currentSceneLoad = SceneManager.LoadSceneAsync(levelIndex);
    }
}
