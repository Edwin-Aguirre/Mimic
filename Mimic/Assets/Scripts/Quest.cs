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

    public void Initialize()
    {
        currentObjectCount = 0;
        isCompleted = false;
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
        if (monsterType == targetMonsterType)
        {
            IncrementObjectCount();
        }
    }

    public void Complete()
    {
        isCompleted = true;
        Debug.Log("Quest Completed: " + questName);
        SoundManager.PlaySound("power 6");
        SoundManager.audioSource.pitch = 1;
        // Add any additional logic for quest completion, such as giving rewards
    }
}
