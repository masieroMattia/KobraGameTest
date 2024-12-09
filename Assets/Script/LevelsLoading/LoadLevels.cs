
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadScenes : MonoBehaviour
{
    public LoadSceneMode loadMode = LoadSceneMode.Single;
    AsyncOperation currentSceneLoad = null;
    int levelIndex;
    public void LoadLevel(int levelIndex)
    {
        currentSceneLoad = SceneManager.LoadSceneAsync(levelIndex);
    }
}
