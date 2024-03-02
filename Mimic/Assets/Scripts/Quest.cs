using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    [TextArea(3, 10)]
    public string description;
    public string progress;
    public bool isCompleted;
    public int targetObjectCount; // New field for the number of objects to find
    public int currentObjectCount;
    public PokemonType targetMonsterType;
    public bool killMonsterQuest;
    public bool transformMonsterQuest;
    public bool canTransformIntoAnyType;
    public bool hasStartedQuest = false;

    public void Initialize()
    {
        currentObjectCount = 0;
        isCompleted = false;
        hasStartedQuest = false;
    }

    public void IncrementObjectCount()
    {
        currentObjectCount++;

        if (currentObjectCount >= targetObjectCount)
        {
            Complete();
        }
    }

    // Additional method for monster kill quests
    public void MonsterKilled(PokemonType monsterType)
    {
        if (monsterType == targetMonsterType && hasStartedQuest)
        {
            IncrementObjectCount();
            SoundManager.PlaySound("select 3");
            SoundManager.audioSource.pitch = 1;
        }
    }

    // Additional method for monster transformation quests
    public void MonsterTransformed(PokemonType monsterType)
    {
        if (monsterType == targetMonsterType && hasStartedQuest)
        {
            IncrementObjectCount();
            SoundManager.PlaySound("select 3");
            SoundManager.audioSource.pitch = 1;
        }
    }

    public void Complete()
    {
        isCompleted = true;
        hasStartedQuest = false;
        Debug.Log("Quest Completed: " + questName);
        SoundManager.PlaySound("power 6");
        SoundManager.audioSource.pitch = 1;
        // Add any additional logic for quest completion, such as giving rewards
    }
}
