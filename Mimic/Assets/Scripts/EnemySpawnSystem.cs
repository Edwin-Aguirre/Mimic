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
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
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
                    int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);

                    GameObject newEnemy = Instantiate(enemyPrefabs[randomEnemyIndex], spawnPoints[i].position, Quaternion.identity);
                    currentEnemyCount++;

                    yield return new WaitForSeconds(spawnInterval);
                }
            }
        }
    }

    public void EnemyDestroyed()
    {
        currentEnemyCount--;

        if (currentEnemyCount < maxEnemies)
        {
            // Respawn an enemy after a delay
            StartCoroutine(RespawnEnemy());
        }
    }

    private IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(spawnInterval);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int enemiesToSpawn = Mathf.Min(enemiesPerSpawnPoint[i], maxEnemies - currentEnemyCount);

            for (int j = 0; j < enemiesToSpawn; j++)
            {
                int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
                GameObject newEnemy = Instantiate(enemyPrefabs[randomEnemyIndex], spawnPoints[i].position, Quaternion.identity);
                currentEnemyCount++;
            }
        }
    }
}
