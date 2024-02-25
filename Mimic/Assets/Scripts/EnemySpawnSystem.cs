using System.Collections;
using UnityEngine;

public class EnemySpawnSystem : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Array of enemy prefabs to spawn
    public Transform[] spawnPoints; // Array of spawn points
    public int[] enemiesPerSpawnPoint; // Array specifying the number of enemies to spawn at each point
    public float spawnInterval = 2.0f; // Time between spawns
    public int maxEnemies = 10; // Maximum number of enemies in the scene

    private int currentEnemyCount;

    private void Start()
    {
        currentEnemyCount = 0;
        StartCoroutine(SpawnInitialEnemies());
    }

    private IEnumerator SpawnInitialEnemies()
    {
        while (currentEnemyCount < maxEnemies)
        {
            if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0 || enemiesPerSpawnPoint.Length == 0)
                yield return null;

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                int enemiesToSpawn = Mathf.Min(enemiesPerSpawnPoint[i], maxEnemies - currentEnemyCount);

                for (int j = 0; j < enemiesToSpawn; j++)
                {
                    int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
                    GameObject newEnemy = Instantiate(enemyPrefabs[randomEnemyIndex], spawnPoints[i].position, Quaternion.identity);
                    currentEnemyCount++;

                    yield return new WaitForSeconds(spawnInterval);
                }
            }
        }

        // Now that initial enemies are spawned, keep spawning remaining enemies
        StartCoroutine(SpawnRemainingEnemies());
    }

    private IEnumerator SpawnRemainingEnemies()
    {
        while (true) // Continue spawning enemies indefinitely
        {
            yield return new WaitForSeconds(spawnInterval);

            // Check if we need to spawn more enemies
            if (currentEnemyCount < maxEnemies)
            {
                for (int j = 0; j < enemiesPerSpawnPoint.Length; j++)
                {
                    int enemiesToSpawn = Mathf.Min(enemiesPerSpawnPoint[j], maxEnemies - currentEnemyCount);

                    for (int k = 0; k < enemiesToSpawn; k++)
                    {
                        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
                        int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);

                        GameObject newEnemy = Instantiate(enemyPrefabs[randomEnemyIndex], spawnPoints[randomSpawnPointIndex].position, Quaternion.identity);
                        currentEnemyCount++;
                    }
                }
            }
        }
    }

    public void EnemyDestroyed()
    {
        currentEnemyCount--;

        if (currentEnemyCount < maxEnemies)
        {
            // No need to respawn immediately, as it's handled by SpawnRemainingEnemies coroutine
        }
    }
}
