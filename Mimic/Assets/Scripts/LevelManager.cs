using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class LevelManager : MonoBehaviour
{
    public SceneAsset[] scenes; // Array to hold the SceneAsset objects
    private MusicManager musicManager;

    void Start()
    {
        musicManager = FindObjectOfType<MusicManager>();

        if (musicManager == null)
        {
            Debug.LogWarning("MusicManager not found in the scene. Make sure to attach the MusicManager script to an object.");
        }
    }

    public void LoadScene(string sceneName)
    {
        if (sceneName != null && sceneName != "")
        {
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

        // Load the new scene
        SceneManager.LoadScene(sceneName);

        // The MusicManager script will handle the music transition when the new scene is loaded
    }
}
