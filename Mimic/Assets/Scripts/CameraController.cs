using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera; // Reference to your Cinemachine Virtual Camera
    [SerializeField] private float enemyDetectionRadius = 3f; // Radius to detect enemies
    [SerializeField] private bool enableCameraSwitching = true; // Toggle for enabling/disabling camera switching

    private Transform target; // Current target for virtualCamera.LookAt

    private void Start()
    {
        // Start with no target
        target = null;
    }

    private void Update()
    {
        // Find the player
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            if (enableCameraSwitching)
            {
                // Find the nearest enemy and look at it
                GameObject nearestEnemy = FindNearestEnemy(player.transform.position);

                if (nearestEnemy != null && nearestEnemy.GetComponent<EnemyAI>().IsPlayerNear(player.transform.position, enemyDetectionRadius))
                {
                    virtualCamera.Follow = player.transform;
                    virtualCamera.LookAt = nearestEnemy.transform;
                    target = nearestEnemy.transform;
                }
                else
                {
                    // If no enemies are found or the player is not near, default to looking at the player
                    virtualCamera.Follow = player.transform;
                    virtualCamera.LookAt = player.transform;
                    target = player.transform; // Set the target back to the player
                }
            }
            else
            {
                // If camera switching is disabled, always look at the player
                virtualCamera.Follow = player.transform;
                virtualCamera.LookAt = player.transform;
                target = player.transform;
            }
        }
    }

    private GameObject FindNearestEnemy(Vector3 position)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float minDistance = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(position, enemy.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the enemy detection radius in the Scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyDetectionRadius);
    }
}
