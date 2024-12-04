using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    #region Public Variables
    public float interval = 1.0f;
    public float elapsedTime = 0.0f;
    public Action eventToDo;
    #endregion

    #region Private Variables
    //private bool isPaused = false;
    #endregion

    #region Life Cycle
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= interval)
        {
            eventToDo?.Invoke();
            elapsedTime = 0.0f;
        }
    }

    public void EventToDo (Action action)
    {
        eventToDo = action;
    }

    //public void Pause() { isPaused = true; }

    //public void Resume() { isPaused = false; }
    #endregion
}