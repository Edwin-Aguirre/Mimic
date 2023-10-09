using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage = 20;
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;
    public KeyCode attackKey = KeyCode.Space;

    // Update is called once per frame
    private void Update()
    {
        // Check if enough time has passed since the last attack
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // Detect player input for attack (you can customize this based on your input system)
            if (Input.GetKeyDown(attackKey)) // Change "Fire1" to your desired attack input
            {
                Attack();
            }
        }
    }

    // Function to perform an attack
    private void Attack()
    {
        // Perform attack logic here
        // For example, raycast from the player to the enemy and deal damage if hit

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            HealthSystem targetHealth = hit.collider.GetComponent<HealthSystem>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }
        }

        // Update the last attack time
        lastAttackTime = Time.time;
    }
}
