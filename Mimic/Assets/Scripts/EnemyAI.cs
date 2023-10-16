using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public float chaseDistance = 10f;   // Distance at which the enemy starts chasing the player
    public float playerDistance = 3f;  // Minimum distance to maintain from the player
    public float wanderRadius = 5f;    // Radius within which the enemy wanders
    public float wanderSpeed = 2f;     // Speed of wandering
    public float chaseSpeed = 5f;      // Speed of chasing
    public float idleDuration = 3f;    // Duration to stay idle at a spot
    public Animator animator;          // Reference to the Animator component
    public BoxCollider triggerBox;     // Trigger box for detecting the player

    private NavMeshAgent agent;
    private Transform player;
    private Vector3 wanderTarget;
    private bool isChasing = false;
    private bool isWandering = true;
    private float idleTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform; // Assuming the player has a "Player" tag
        wanderTarget = GetRandomPointInRadius();
        agent.speed = wanderSpeed;
        agent.SetDestination(wanderTarget);
        animator.SetBool("isWalking", true);
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseDistance)
        {
            // Calculate the direction from enemy to player
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0f;

            // Maintain a minimum distance from the player
            if (distanceToPlayer < playerDistance)
            {
                Vector3 newPosition = player.position - directionToPlayer.normalized * playerDistance;
                agent.SetDestination(newPosition);
            }
            else
            {
                // Chase the player
                isChasing = true;
                agent.speed = chaseSpeed;
                agent.SetDestination(player.position);
                animator.SetBool("isWalking", true);
            }
        }
        else if (isChasing)
        {
            // Stop chasing if the player is out of range
            isChasing = false;
            agent.speed = wanderSpeed;
            wanderTarget = GetRandomPointInRadius();
            agent.SetDestination(wanderTarget);
            animator.SetBool("isWalking", true);
        }
        else if (agent.remainingDistance < 0.1f)
        {
            if (isWandering)
            {
                // Start the idle timer
                idleTimer += Time.deltaTime;
                animator.SetBool("isWalking", false);

                if (idleTimer >= idleDuration)
                {
                    // Reset the timer and continue wandering
                    idleTimer = 0f;
                    isWandering = false;
                    wanderTarget = GetRandomPointInRadius();
                    agent.SetDestination(wanderTarget);
                    animator.SetBool("isWalking", true);
                }
            }
            else
            {
                // Play the idle animation once the idle duration is reached
                animator.SetBool("isWalking", false);
            }
        }

        // Check if we've reached the destination while wandering
        if (!isChasing && agent.remainingDistance < 0.1f && !isWandering)
        {
            // Start the idle timer when reaching a new spot
            idleTimer = 0f;
            isWandering = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
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
