using System.Collections;
using UnityEngine;
using UnityEditor;
using EasyTransition;
using TMPro;

public class LevelManager : MonoBehaviour
{
    private MusicManager musicManager;

    public TransitionSettings transition;
    public float loadDelay;

    public TextMeshProUGUI levelIntroText;
    public string levelText;
    public float fadeInDuration = 1f;
    public float displayTime = 3f;
    public float fadeOutDuration = 1f;

    void Start()
    {
        ShowLevelText();

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

    private void ShowLevelText()
    {
        levelIntroText.text = levelText;;
        levelIntroText.gameObject.SetActive(true);
        // Set initial alpha to fully transparent
        levelIntroText.color = new Color(levelIntroText.color.r, levelIntroText.color.g, levelIntroText.color.b, 0f);

        // Start the coroutine to handle fading
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            levelIntroText.color = new Color(levelIntroText.color.r, levelIntroText.color.g, levelIntroText.color.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Wait for the specified display time
        yield return new WaitForSeconds(displayTime);

        // Start the fade-out phase
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            levelIntroText.color = new Color(levelIntroText.color.r, levelIntroText.color.g, levelIntroText.color.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        levelIntroText.gameObject.SetActive(false);
    }
}
