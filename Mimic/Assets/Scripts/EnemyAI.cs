using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;           // Reference to the player object
    public float chaseDistance = 10f; // Distance at which the enemy starts chasing the player
    public float wanderRadius = 5f;   // Radius within which the enemy wanders
    public float wanderSpeed = 2f;    // Speed of wandering
    public float chaseSpeed = 5f;     // Speed of chasing
    private NavMeshAgent agent;
    private Vector3 wanderTarget;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        wanderTarget = GetRandomPointInRadius();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseDistance)
        {
            // Chase the player
            isChasing = true;
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
        else if (isChasing)
        {
            // Stop chasing if the player is out of range
            isChasing = false;
            agent.speed = wanderSpeed;
            wanderTarget = GetRandomPointInRadius();
            agent.SetDestination(wanderTarget);
        }

        // Check if we've reached the destination while wandering
        if (!isChasing && agent.remainingDistance < 0.1f)
        {
            wanderTarget = GetRandomPointInRadius();
            agent.SetDestination(wanderTarget);
        }
    }

    Vector3 GetRandomPointInRadius()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1);
        return hit.position;
    }
}
