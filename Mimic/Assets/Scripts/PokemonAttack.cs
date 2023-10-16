using UnityEngine;
using System.Collections.Generic;

// Define the types and their effectiveness against each other.
public enum PokemonType
{
    Normal,
    Fire,
    Water,
    Grass,
    // Add more types here...
}

[System.Serializable]
public class TypeAdvantage
{
    public PokemonType attackerType;
    public List<PokemonType> strongAgainst = new List<PokemonType>();
    public List<PokemonType> weakAgainst = new List<PokemonType>();
}

public class PokemonAttack : MonoBehaviour
{
    public PokemonType type;
    public float attackRange = 2.0f; // Adjust this value to set your attack range.
    public HealthSystem targetHealth; // Reference to the HealthSystem of the target enemy.
    public Animator animator; // Reference to the Animator component.

    // Create a list of type advantages.
    public List<TypeAdvantage> typeAdvantages = new List<TypeAdvantage>();

    private Transform target; // The target enemy to attack.

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && gameObject.CompareTag("Player"))
        {
            // Trigger the attack animation whenever you press Space and have the "Player" tag.
            animator.SetBool("isAttacking", true);

            if (target != null)
            {
                // Calculate the distance to the target.
                float distance = Vector3.Distance(transform.position, target.position);

                if (distance <= attackRange)
                {
                    int damage = CalculateDamage(target.GetComponent<PokemonAttack>().type);
                    Debug.Log("Dealt " + damage + " damage to the enemy.");

                    // Apply damage to the target's health.
                    targetHealth.TakeDamage(damage);
                }
            }
        }
        else
        {
            // Ensure the "isAttacking" parameter is set to false if not attacking.
            animator.SetBool("isAttacking", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // Tag your enemy GameObject with "Enemy".
        {
            target = other.transform;
            targetHealth = other.GetComponent<HealthSystem>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            target = null;
            targetHealth = null;
        }
    }

    // Function to calculate damage based on type advantages.
    public int CalculateDamage(PokemonType target)
    {
        int baseDamage = 10; // You can set your own base damage value.
        int damage = baseDamage;

        if (type != target)
        {
            foreach (TypeAdvantage advantage in typeAdvantages)
            {
                if (advantage.attackerType == type)
                {
                    if (advantage.strongAgainst.Contains(target))
                    {
                        damage *= 2; // Double damage for strong type advantage.
                    }
                    else if (advantage.weakAgainst.Contains(target))
                    {
                        damage /= 2; // Half damage for weak type advantage.
                    }
                }
            }
        }

        return damage;
    }
}
