using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public class EggsLoadingManager : MonoBehaviour
{
    #region Public Variables
    public Transform kobraHead;
    public Transform eggs;
    public LoadingKobraScript kobraScript;
    public TextMeshProUGUI loadingText;
    #endregion

    #region Private Variables
    private float collisionDistance = 0.1f;
    #endregion

    #region Cycle Life

    void Update()
    {
        EatingEggs();
        AllEggsEaten();
    }
    #endregion

    #region Private Methods
    // Check if the distance between egg and kobra is enough for destroing the egg
    private void EatingEggs()
    {
        for (int i = 0; i < eggs.childCount; i++)
        {
            Transform egg = eggs.GetChild(i);
            if (Vector3.Distance(egg.position, kobraHead.position) < collisionDistance)
            {
                kobraScript.AddTail();
                Destroy(egg.gameObject);
            }
        }
    }
    // Change text when all egg are eaten
    private void AllEggsEaten()
    {
        int eggsCounter = eggs.childCount;

        if (eggsCounter == 0)
        {
            loadingText.text = "Press any key to start";
        }
    }
    #endregion
}
