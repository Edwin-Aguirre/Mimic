using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyTransition;
using System;

public class RegionManager : MonoBehaviour
{
    public TransitionSettings transition;
    public float loadDelay;
    public String sceneName;

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
            // Store the player's position before loading the new scene
            SceneController.playerPosition = other.transform.position;
        }
        if (sceneName != null && sceneName != "")
        {
            TransitionManager.Instance().Transition(sceneName, transition, loadDelay);
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
}
