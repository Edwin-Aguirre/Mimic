using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] sceneMusic; // Array to hold the music for each scene
    public static AudioSource audioSource;
    public AudioMixerGroup musicMixer;
    private int currentSceneIndex;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();

            // Assign the Audio Mixer to the Audio Source output
            audioSource.outputAudioMixerGroup = musicMixer;

            audioSource.loop = true;
            audioSource.volume = 0.3f;
            audioSource.pitch = 1;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate MusicManager objects
            return;
        }

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
            audioSource.pitch = 1;

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
