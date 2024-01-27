using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    public List<Quest> quests = new List<Quest>();

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
    }

    private void InitializeQuests()
    {
        foreach (var quest in quests)
        {
            quest.Initialize();
        }
    }

    public void CompleteQuest(string questName)
    {
        Quest quest = quests.Find(q => q.questName == questName);

        if (quest != null)
        {
            quest.Complete();
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
}
