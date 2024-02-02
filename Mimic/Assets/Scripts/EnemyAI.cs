using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public GameObject floatingText;
    private int damage;
    public float chaseDistance = 10f;
    public float playerDistance = 3f;
    public float wanderRadius = 5f;
    public float wanderSpeed = 2f;
    public float chaseSpeed = 5f;
    public float idleDuration = 3f;
    public float attackDelay = 2f;
    public Animator animator;

    private NavMeshAgent agent;
    private Transform player;
    private Vector3 wanderTarget;
    private bool isChasing = false;
    private bool isWandering = true;
    private float idleTimer = 0f;
    private float attackTimer = 0f;
    private bool isAttacking = false;

    private PokemonAttack enemyAttack;
    private bool isAnimationPlaying = false;

    public HealthSystem healthSystem; // Reference to the enemy's health system
    public Material flashMaterial;  // The material to switch to for the flash effect

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //player = GameObject.FindWithTag("Player").transform;
        wanderTarget = GetRandomPointInRadius();
        agent.speed = wanderSpeed;
        agent.SetDestination(wanderTarget);
        animator.SetBool("isWalking", true);

        enemyAttack = GetComponent<PokemonAttack>();
    }

    void Update()
    {
        //Quick fix, might need to change this later
        player = GameObject.FindWithTag("Player").transform;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseDistance)
        {
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0f;

            if (distanceToPlayer < playerDistance)
            {
                Vector3 newPosition = player.position - directionToPlayer.normalized * playerDistance;
                agent.SetDestination(newPosition);
                if (enemyAttack != null && !isAttacking)
                {
                    damage = enemyAttack.CalculateDamage(player.GetComponent<PokemonAttack>().type);
                    Debug.Log("Dealt " + damage + " damage to the player.");

                    // Apply damage to the player's health system or handle it as needed.
                    player.GetComponent<HealthSystem>().TakeDamage(damage);
                    ShowFloatingText();
                    SoundManager.PlaySound("hurt 1");

                    // Start the attack animation and set the isAttacking flag.
                    animator.SetBool("isAttacking", true);
                    isAttacking = true;
                    isAnimationPlaying = true;
                    attackTimer = 0f;

                    // Trigger the flash effect on the player
                    StartCoroutine(FlashCoroutine(player.GetComponent<Renderer>()));
                }
            }
            else
            {
                if (!isAttacking)
                {
                    isChasing = true;
                    agent.speed = chaseSpeed;
                    agent.SetDestination(player.position);
                    animator.SetBool("isWalking", true);
                }
            } 
        }
        else if (isChasing)
        {
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
                idleTimer += Time.deltaTime;
                animator.SetBool("isWalking", false);

                if (idleTimer >= idleDuration)
                {
                    idleTimer = 0f;
                    isWandering = false;
                    wanderTarget = GetRandomPointInRadius();
                    agent.SetDestination(wanderTarget);
                    animator.SetBool("isWalking", true);
                }
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }

        if (!isChasing && agent.remainingDistance < 0.1f && !isWandering)
        {
            idleTimer = 0f;
            isWandering = true;
        }

        if (isAnimationPlaying)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackDelay)
            {
                animator.SetBool("isAttacking", false);
                isAttacking = false;
                isAnimationPlaying = false;
            }
        }
    }

    public bool IsPlayerNear(Vector3 playerPosition, float detectionRadius)
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
        return distanceToPlayer <= detectionRadius;
    }

    IEnumerator FlashCoroutine(Renderer targetRenderer)
{
    if (targetRenderer == null)
    {
        yield break; // Exit the coroutine if the targetRenderer is null.
    }

    // Store the original material of the target
    Material targetOriginalMaterial = targetRenderer.material;

    // Change target's material emission color to indicate flash
    Color originalEmissionColor = targetOriginalMaterial.GetColor("_EmissionColor");
    targetOriginalMaterial.EnableKeyword("_EMISSION");
    targetOriginalMaterial.SetColor("_EmissionColor", flashMaterial.GetColor("_EmissionColor"));

    // Record the start time
    float startTime = Time.time;

    // Wait for 0.1 seconds
    while (Time.time - startTime < 0.1f)
    {
        // Check if the target is still valid before reverting the material
        if (targetRenderer == null)
        {
            // If the target is destroyed during the wait, exit the coroutine
            targetOriginalMaterial.SetColor("_EmissionColor", originalEmissionColor);
            yield break;
        }

        // Check if the player left the trigger box during the wait
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > playerDistance) // Change to your desired distance variable
        {
            // Revert target's material emission color back to original
            targetOriginalMaterial.SetColor("_EmissionColor", originalEmissionColor);

            // Exit the coroutine if the player left the trigger box
            yield break;
        }

        yield return null;
    }

    // Revert target's material emission color back to original
    targetOriginalMaterial.SetColor("_EmissionColor", originalEmissionColor);
}

    private void ShowFloatingText()
    {
        var go = Instantiate(floatingText, player.transform.position, Quaternion.identity, player.transform);
        go.GetComponent<TextMesh>().text = damage.ToString();

        if(damage == 20)
        {
            go.GetComponent<TextMesh>().color = Color.green;
            SoundManager.audioSource.pitch = 2f;
        }
        if(damage == 10)
        {
            go.GetComponent<TextMesh>().color = Color.white;
            SoundManager.audioSource.pitch = 1.5f;
        }
        if(damage == 5)
        {
            go.GetComponent<TextMesh>().color = Color.red;
            SoundManager.audioSource.pitch = 1f;
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
