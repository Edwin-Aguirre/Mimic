using System.Collections;
using UnityEngine;

public class EnemySpawnSystem : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Array of enemy prefabs to spawn
    public Transform[] spawnPoints; // Array of spawn points
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
            if (enemyPrefabs.Length == 0 || spawnPoints.Length == 0)
                yield return null;

            int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
            int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);

            GameObject newEnemy = Instantiate(enemyPrefabs[randomEnemyIndex], spawnPoints[randomSpawnPointIndex].position, Quaternion.identity);
            currentEnemyCount++;

            yield return new WaitForSeconds(spawnInterval);
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

        int randomEnemyIndex = Random.Range(0, enemyPrefabs.Length);
        int randomSpawnPointIndex = Random.Range(0, spawnPoints.Length);

        GameObject newEnemy = Instantiate(enemyPrefabs[randomEnemyIndex], spawnPoints[randomSpawnPointIndex].position, Quaternion.identity);
        currentEnemyCount++;
    }
}
