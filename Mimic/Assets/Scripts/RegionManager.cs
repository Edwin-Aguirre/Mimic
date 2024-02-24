using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyTransition;
using System;

public class RegionManager : MonoBehaviour
{
    public TransitionSettings transition;
    public float loadDelay;
    public string sceneName;
    public string exitName;

    //public SceneAsset scenes; // Array to hold the SceneAsset objects
    private MusicManager musicManager;

    void Start()
    {
        musicManager = FindObjectOfType<MusicManager>();

        if (musicManager == null)
        {
            Debug.LogWarning("MusicManager not found in the scene. Make sure to attach the MusicManager script to an object.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(SaveSpawnLocation());
        }
        if (sceneName != null && sceneName != "")
        {
            //TransitionManager.Instance().Transition(sceneName, transition, loadDelay);
            // Start the transition to a new scene
            StartCoroutine(TransitionToScene(sceneName));
        }
        else
        {
            Debug.LogWarning("Invalid scene name.");
        }
    }

    IEnumerator TransitionToScene(string sceneName)
    {
        // Fade out the music
        if (musicManager != null)
        {
            yield return StartCoroutine(musicManager.FadeOutMusic(1.0f));
        }
    }

    IEnumerator SaveSpawnLocation()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerPrefs.SetString("LastExitName", exitName);
        TransitionManager.Instance().Transition(sceneName, transition, loadDelay);
    }
}
