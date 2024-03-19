using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider; // Reference to the UI Slider for health display
    public float healthChangeSpeed = 5f; // The speed at which the health bar updates
    [SerializeField]
    private float stunDuration = 10f; // Duration of the stun phase in seconds
    private EnemySpawnSystem enemySpawnSystem;
    public bool isStunned = false;
    public bool isAlive = true;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        enemySpawnSystem = FindAnyObjectByType<EnemySpawnSystem>();
    }

    public void TakeDamage(int damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Start a coroutine to smoothly update the health bar
        StartCoroutine(UpdateHealthBarSmoothly());

        // Check if the enemy is not already stunned and health is below half
        if (!isStunned && currentHealth <= maxHealth / 2)
        {
            StartCoroutine(StunEnemy());
        }

        if (currentHealth == 0)
        {
            if (!gameObject.CompareTag("Player"))
            {
                Die();
            }
        }
    }

    private void Die()
    {
        isAlive = false;
        GetComponent<LootDropSystem>().DropLoot(transform.position);
        PokemonAttack pokemonAttack = GetComponent<PokemonAttack>();
        QuestManager.instance.MonsterKilled(pokemonAttack.type);
        Debug.Log(gameObject.name + " has died!");

        // Trigger death animation
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // Delay destruction to allow for the death animation to play
        StartCoroutine(DestroyAfterAnimation(4));

        // No need to destroy immediately
        // Destroy(gameObject);
        enemySpawnSystem.EnemyDestroyed();
    }

    private IEnumerator DestroyAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }


    public IEnumerator UpdateHealthBarSmoothly()
    {
        float targetValue = (float)currentHealth / maxHealth;
        float startValue = healthSlider.value;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * healthChangeSpeed;
            healthSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime);
            yield return null;
        }

        // Ensure the health bar reaches the exact target value
        healthSlider.value = targetValue;
    }

    public void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }

    private IEnumerator StunEnemy()
    {
        if(gameObject.tag != "Player")
        {
            isStunned = true;
            Animator animator = GetComponent<Animator>();
            animator.SetBool("isStunned", true); // Trigger dizzy animation

            // Disable NavMeshAgent during stun phase
            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false;
            }

            yield return new WaitForSeconds(stunDuration);

            // Re-enable NavMeshAgent after stun duration
            if (navMeshAgent != null && SwitchControl.instance.playerControl)
            {
                navMeshAgent.enabled = true;
            }

            animator.SetBool("isStunned", false); // Reset dizzy animation
            isStunned = false;
        }
    }

}
