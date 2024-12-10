using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using S = System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Audio;
using static Unity.VisualScripting.Member;


public class AudioManager : MonoBehaviour
{
    #region Public variables
    [Header("BGM reference")]
    public AudioClip deathScreen;
    public AudioClip level1;
    public AudioClip levelStarted;
    public AudioClip titleScreen;
    [Header("SFX reference")]
    public AudioClip eggEaten;
   
    public AudioClip kobraSpawn;
    public AudioClip kobraDeath;    
    public AudioClip kobraResurrect;
    public AudioClip kobraDirection;   

    public AudioClip maskCollected;
    public AudioClip maskSpawned;


    public AudioMixerGroup BGM;
    public AudioMixerGroup SFX;
    
    public AudioMixer mixer;

    #endregion
    #region Lifecycle
    void Update()
    {
        
    }
    #endregion
    #region Private methods
    public void PlayClipAtPoint(AudioClip clip, AudioMixerGroup targetGroup, bool autoDestroy = true, bool loop = false)
    {
        if (clip == null)
        {
            Debug.LogError("AudioClip is null!");
            return;
        }

        // Prepare the game object for audio source component
        GameObject sourceGO = new GameObject($"{clip.name}");

        // Add the audio source component to the created game object
        AudioSource source = sourceGO.AddComponent<AudioSource>();  // This 'source' is fine, as it is not inside the foreach loop

        // Prepare audio source with needed data
        source.clip = clip;
        source.spatialBlend = 0.0f; // 2D sound
        source.loop = loop;
        if (!loop)  // Looping sounds must not destroy after the duration of a single loop
            Destroy(sourceGO, clip.length);

        source.outputAudioMixerGroup = targetGroup;

        // Play audio clip
        source.Play();
    }
    public void PlayClipAtPointOneTime(AudioClip clip, AudioMixerGroup targetGroup, bool autoDestroy = true, bool loop = false)
    {
        if (clip == null)
        {
            Debug.LogError("AudioClip is null!");
            return;
        }

        // Check if the clip is already playing before playing again
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in allAudioSources) // Changed 'source' to 'audioSource'
        {
            if (audioSource.clip == clip && audioSource.isPlaying)
            {
                // If the clip is already playing, do not play it again
                return;
            }
        }

        // Prepare the game object for audio source component
        GameObject sourceGO = new GameObject($"{clip.name}@{Time.frameCount}");

        // Add the audio source component to the created game object
        AudioSource source = sourceGO.AddComponent<AudioSource>();  // This 'source' is fine, as it is not inside the foreach loop

        // Prepare audio source with needed data
        source.clip = clip;
        source.spatialBlend = 0.0f; // 3D sound
        source.loop = loop;
        if (!loop)  // Looping sounds must not destroy after the duration of a single loop
            Destroy(sourceGO, clip.length);

        source.outputAudioMixerGroup = targetGroup;

        // Play audio clip
        source.Play();
    }

    public void StopClip(AudioClip clip)
    {
        // Find all objects of type AudioSource in the scene
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (var source in allAudioSources)
        {
            // Check if the current audio source is playing the specified clip
            if (source.clip == clip)
            {
                // Stop the audio
                source.Stop();

                // Destroy the GameObject if it was dynamically created
                if (source.gameObject.name.StartsWith($"{clip.name}@"))
                {
                    Destroy(source.gameObject);
                }

                break; // Exit the loop after finding the first match
            }
        }
    }

    public void ResumeClip(AudioClip clip, AudioMixerGroup targetGroup)
    {
        // Find all objects of type AudioSource in the scene
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (var source in allAudioSources)
        {
            // Check if the current audio source is playing the specified clip
            if (source.clip == clip)
            {
                if (!source.isPlaying)
                {
                    source.Play();
                }

                return; // Resume the first matching clip
            }
        }

    }


    #endregion
}

