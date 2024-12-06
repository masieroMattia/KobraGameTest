using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MyTime
{
    float startTime;
    int startFrameCount;

    bool isRealTime;
    bool isPaused;
    float pausedTime;
    float currentTime => isRealTime ? Time.realtimeSinceStartup : Time.time;

    public MyTime(bool isRealTime = false)
    {
        this.isRealTime = isRealTime;
        Restart();
    }

    public void Restart()
    {
        startTime = currentTime;
        startFrameCount = Time.frameCount;
        isPaused = false;
    }
    public void Stop()
    {
        if (!isPaused)
        {
            pausedTime = currentTime;
            isPaused = true;
        }
    }

    public void Resume()
    {
        if (isPaused)
        {
            startTime += currentTime - pausedTime;
            isPaused = false;
        }
    }

    public float ElapsedSeconds()
    {
        return currentTime - startTime;
    }

    public int ElapsedFrames()
    {
        return Time.frameCount - startFrameCount;
    }

    public bool Every(float seconds)
    {
        if (ElapsedSeconds() >= seconds)
        {
            return true;
        }

        return false;
    }

    public void Every(float seconds, UnityAction<float> action)
    {
        if (ElapsedSeconds() >= seconds)
        {
            action(ElapsedSeconds());
            Restart();
        }
    }
    public void Every(float seconds, UnityAction action)
    {
        if (ElapsedSeconds() >= seconds)
        {
            action();
            Restart();
        }
    }


}

