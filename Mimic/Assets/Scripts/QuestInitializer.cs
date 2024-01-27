using System.Collections;
using TMPro;
using UnityEngine;

public class QuestInitializer : MonoBehaviour
{
    public Quest questToStart;

    private bool hasInteracted;
    private bool questStarted;

    public TextMeshProUGUI questStartedText;
    public float fadeInDuration = 1f;
    public float displayTime = 3f;
    public float fadeOutDuration = 1f;

    public GameObject questPanel;
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI questDescriptionText;

    private InteractButtonPrompt interactButtonPrompt;

    
    private void Start()
    {
        interactButtonPrompt = GetComponent<InteractButtonPrompt>();
    }

    public void StartQuestAfterPanelClosed()
    {
        if (interactButtonPrompt != null && !interactButtonPrompt.interactPanel.activeSelf && !questStarted)
        {
            // Add any additional logic for quest initialization, such as displaying UI messages
            // Start the quest after the panel is closed
            StartQuest();

            // Show the quest-related UI elements
            ShowQuestStartedText();
            ShowQuestPanel();

            questStarted = true;
        }
    }

    private void ShowQuestPanel()
    {
        questPanel.SetActive(true);
        questNameText.text = questToStart.name;
        questDescriptionText.text = questToStart.description;
    }

    private void ShowQuestStartedText()
    {
        questStartedText.text = "Quest Started: " + "\n" + questToStart.questName;
        questStartedText.gameObject.SetActive(true);
        // Set initial alpha to fully transparent
        questStartedText.color = new Color(questStartedText.color.r, questStartedText.color.g, questStartedText.color.b, 0f);

        // Start the coroutine to handle fading
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            questStartedText.color = new Color(questStartedText.color.r, questStartedText.color.g, questStartedText.color.b, alpha);

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
            questStartedText.color = new Color(questStartedText.color.r, questStartedText.color.g, questStartedText.color.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        questStartedText.gameObject.SetActive(false);
    }

    private void StartQuest()
    {
        QuestManager.instance.quests.Add(questToStart);
        Debug.Log("Quest Started: " + questToStart.questName);
        SoundManager.PlaySound("loading 3");
        hasInteracted = true;

        // Add any additional logic for quest initialization, such as displaying UI messages

        // Switch the colliders of associated QuestTracker objects to triggers
        SwitchQuestTrackerColliders(true);
    }

    private void SwitchQuestTrackerColliders(bool isTrigger)
    {
        QuestTracker[] questTrackers = FindObjectsOfType<QuestTracker>();

        foreach (var tracker in questTrackers)
        {
            if (tracker.quest == questToStart)
            {
                tracker.GetComponent<Collider>().isTrigger = isTrigger;
            }
        }
    }
}
