using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] sceneMusic; // Array to hold the music for each scene
    private AudioSource audioSource;
    private int currentSceneIndex;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Load the initial scene music
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        PlaySceneMusic(currentSceneIndex);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the scene has changed
        if (currentSceneIndex != scene.buildIndex)
        {
            currentSceneIndex = scene.buildIndex;

            // Stop the audio source to ensure a clean start
            audioSource.Stop();

            // Play the scene music
            PlaySceneMusic(currentSceneIndex);
        }
    }

    void PlaySceneMusic(int sceneIndex)
    {
        // Check if the scene index is within the array bounds
        if (sceneIndex >= 0 && sceneIndex < sceneMusic.Length)
        {
            // Set the new music clip and start playing
            audioSource.clip = sceneMusic[sceneIndex];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Scene index out of bounds or no music assigned for this scene.");
        }
    }

    public IEnumerator FadeOutMusic(float fadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    IEnumerator FadeInMusic(float fadeTime)
    {
        float targetVolume = audioSource.volume;

        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}
