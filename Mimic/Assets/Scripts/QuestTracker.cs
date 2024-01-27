using System.Collections;
using TMPro;
using UnityEngine;

public class QuestTracker : MonoBehaviour
{
    public Quest quest; // Reference to the quest associated with this tracker
    public QuestInitializer questInitializer;

    private new Collider collider;

    public TextMeshProUGUI questCompletedText;
    public TextMeshProUGUI questObjectAmountText;
    public float fadeInDuration = 1f;
    public float displayTime = 3f;
    public float fadeOutDuration = 1f;

    private void Start()
    {
        collider = GetComponent<Collider>();
        InitializeQuest();
        questObjectAmountText.text = "Tree Stumps Found " + quest.currentObjectCount + " / " + quest.targetObjectCount;
    }

    private void InitializeQuest()
    {
        quest.Initialize();

        // If the quest is not initialized, set the collider as non-trigger
        if (!quest.isCompleted)
        {
            SetColliderAsNonTrigger();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !quest.isCompleted)
        {
            quest.IncrementObjectCount();
            SoundManager.PlaySound("select 3");
            gameObject.GetComponent<MeshRenderer>().enabled = false; // Hide the object when picked up, adjust as needed
            gameObject.GetComponent<BoxCollider>().enabled = false;
            questObjectAmountText.text = "Tree Stumps Found " + quest.currentObjectCount + " / " + quest.targetObjectCount;
            // If the quest is completed after incrementing the count, set the collider as non-trigger
            if (quest.isCompleted)
            {
                SetColliderAsNonTrigger();
                ShowQuestCompletedText();
                questInitializer.questNameText.text = "<s>" + questInitializer.questToStart.name + "<s>";
                questInitializer.questDescriptionText.text = "";
                questObjectAmountText.text = "";
            }
        }
    }

    private void SetColliderAsNonTrigger()
    {
        collider.isTrigger = false;
    }

    private void ShowQuestCompletedText()
    {
        Debug.Log("Showing Quest Completed Text");
        questCompletedText.text = "Quest Completed: " + "\n" + quest.questName;
        questCompletedText.gameObject.SetActive(true);
        // Set initial alpha to fully transparent
        questCompletedText.color = new Color(questCompletedText.color.r, questCompletedText.color.g, questCompletedText.color.b, 0f);

        // Start the coroutine to handle fading
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            questCompletedText.color = new Color(questCompletedText.color.r, questCompletedText.color.g, questCompletedText.color.b, alpha);

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
            questCompletedText.color = new Color(questCompletedText.color.r, questCompletedText.color.g, questCompletedText.color.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        questCompletedText.gameObject.SetActive(false);
    }
}
