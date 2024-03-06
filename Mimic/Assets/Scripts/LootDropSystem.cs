using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootDrop
{
    public GameObject itemPrefab;
    public int minAmount;
    public int maxAmount;
    public float dropChance;
}

public class LootDropSystem : MonoBehaviour
{
    public List<LootDrop> lootTable = new List<LootDrop>();

    public void DropLoot(Vector3 spawnPosition)
    {
        foreach (LootDrop drop in lootTable)
        {
            float rand = Random.value;
            if (rand <= drop.dropChance)
            {
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);
                float angleStep = 360f / amount;

                for (int i = 0; i < amount; i++)
                {
                    float angle = i * angleStep;
                    Vector3 spawnOffset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * Random.Range(0.5f, 1f); // Adjust radius here
                    Vector3 spawnPos = spawnPosition + spawnOffset + Vector3.up * 0.1f; // Adjust Y position here
                    Instantiate(drop.itemPrefab, spawnPos, Quaternion.identity);
                }
            }
        }
    }
}
