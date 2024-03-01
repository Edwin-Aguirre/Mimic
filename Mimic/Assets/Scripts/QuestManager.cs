using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    public List<Quest> quests = new List<Quest>();

    public TextMeshProUGUI questObjectAmountText;
    public TextMeshProUGUI questCompletedText;
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI questDescriptionText;

    private float fadeInDuration = 1f;
    private float displayTime = 3f;
    private float fadeOutDuration = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeQuests();
        questObjectAmountText.text = quests[0].progress + " Killed " + 
        quests[0].currentObjectCount + " / " + quests[0].targetObjectCount + " " +
        quests[0].targetMonsterType + " Monsters";
    }

    private void InitializeQuests()
    {
        foreach (var quest in quests)
        {
            quest.Initialize();
        }
    }

    public bool AreAllQuestsCompleted()
    {
        foreach (var quest in quests)
        {
            if (!quest.isCompleted)
            {
                return false;
            }
        }
        return true;
    }

    private void CompleteQuest(Quest quest)
    {

        if(quest.isCompleted)
        {
            questCompletedText.text = "Quest Completed: " + "\n" + quest.questName;
            questCompletedText.gameObject.SetActive(true);
            // Set initial alpha to fully transparent
            questCompletedText.color = new Color(questCompletedText.color.r, questCompletedText.color.g, questCompletedText.color.b, 0f);

            // Start the coroutine to handle fading
            StartCoroutine(FadeIn());

            questNameText.text = "<s>" + quest.name + "<s>";
            questDescriptionText.text = "";
            questObjectAmountText.text = "";
        }

    }

    // Method to handle monster kills
    public void MonsterKilled(PokemonType monsterType)
    {
        foreach (var quest in quests)
        {
            if (!quest.isCompleted && quest.targetMonsterType == monsterType)
            {
                quest.MonsterKilled(monsterType);
                questObjectAmountText.text = quest.progress + " Killed " + 
                quest.currentObjectCount + " / " + quest.targetObjectCount + " " +
                quest.targetMonsterType + " Monsters";

                if(quest.isCompleted)
                {
                    CompleteQuest(quest);
                }
            }
        }
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
