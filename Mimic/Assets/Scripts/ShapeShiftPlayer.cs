using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeShiftPlayer : MonoBehaviour
{
    public float interactionDistance = 2f;  // Distance at which player can interact with enemies
    public KeyCode shapeshiftKey = KeyCode.E;  // Key to trigger shapeshifting
    private GameObject currentEnemy;  // Reference to the current enemy
    private bool isPlayerShapeshifted = false;  // Flag to track shapeshift state
    private Mesh originalPlayerMesh;  // Store the original player mesh
    private Material originalPlayerMaterial;  // Store the original player material
    private HealthSystem playerHealth;  // Reference to the player's health system

    private void Start()
    {
        // Store the original player mesh and material
        MeshFilter playerMeshFilter = GetComponent<MeshFilter>();
        if (playerMeshFilter != null)
        {
            originalPlayerMesh = playerMeshFilter.mesh;
        }
        MeshRenderer playerRenderer = GetComponent<MeshRenderer>();
        if (playerRenderer != null)
        {
            originalPlayerMaterial = playerRenderer.material;
        }

        // Get the player's health system
        playerHealth = GetComponent<HealthSystem>();
    }

    private void Update()
    {
        // Check for player input to trigger shapeshifting
        if (Input.GetKeyDown(shapeshiftKey))
        {
            if (isPlayerShapeshifted)
            {
                // Revert back to the player's original appearance
                RevertShapeshift();
            }
            else if (currentEnemy != null)
            {
                // Perform shapeshifting and destroy the enemy
                ShapeshiftPlayer();
            }
        }

        // Check for nearby enemies
        CheckForNearbyEnemies();
    }

    // Check for nearby enemies
    private void CheckForNearbyEnemies()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance))
        {
            // Check if the hit object is an enemy (you can use tags or layers to identify enemies)
            if (hit.collider.CompareTag("Enemy"))
            {
                // Set the current enemy
                currentEnemy = hit.collider.gameObject;
            }
            else
            {
                // Clear the current enemy if it's not an enemy
                currentEnemy = null;
            }
        }
        else
        {
            // Clear the current enemy if no enemy is in range
            currentEnemy = null;
        }
    }

    // Perform shapeshifting
    private void ShapeshiftPlayer()
    {
        if (currentEnemy != null)
        {
            // Get the enemy's health system
            HealthSystem enemyHealth = currentEnemy.GetComponent<HealthSystem>();

            if (enemyHealth != null)
            {
                // Swap materials between player and enemy
                MeshRenderer playerRenderer = GetComponent<MeshRenderer>();
                MeshRenderer enemyRenderer = currentEnemy.GetComponent<MeshRenderer>();
                MeshFilter playerMeshFilter = GetComponent<MeshFilter>();
                MeshFilter enemyMeshFilter = currentEnemy.GetComponent<MeshFilter>();

                if (playerRenderer != null && enemyRenderer != null && playerMeshFilter != null && enemyMeshFilter != null)
                {
                    Material playerMaterial = playerRenderer.material;
                    playerRenderer.material = enemyRenderer.material;
                    enemyRenderer.material = playerMaterial;
                }

                // Swap meshes between player and enemy
                Mesh playerMesh = playerMeshFilter.mesh;
                playerMeshFilter.mesh = enemyMeshFilter.mesh;
                enemyMeshFilter.mesh = playerMesh;

                // Transfer enemy's health to the player
                playerHealth.maxHealth = enemyHealth.maxHealth;
                playerHealth.currentHealth = enemyHealth.currentHealth;

                // Update the player's health bar UI
                playerHealth.UpdateHealthUI();

                // Destroy the current enemy object
                Destroy(currentEnemy);
                currentEnemy = null;

                isPlayerShapeshifted = true;
            }
            else
            {
                Debug.LogWarning("Enemy is missing the HealthSystem component.");
            }
        }
    }

    // Revert back to the player's original appearance
    private void RevertShapeshift()
    {
        MeshRenderer playerRenderer = GetComponent<MeshRenderer>();
        MeshFilter playerMeshFilter = GetComponent<MeshFilter>();

        if (playerRenderer != null && playerMeshFilter != null)
        {
            playerRenderer.material = originalPlayerMaterial;
            playerMeshFilter.mesh = originalPlayerMesh;

            isPlayerShapeshifted = false;
        }
        else
        {
            Debug.LogWarning("Player is missing one or more required components (MeshRenderer and/or MeshFilter).");
        }
    }
}
