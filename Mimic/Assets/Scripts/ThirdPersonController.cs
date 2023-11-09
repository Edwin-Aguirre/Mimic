using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float gravity = 9.81f;
    private Vector3 spawnPoint;
    public Animator animator;

    private CharacterController characterController;
    private HealthSystem healthSystem;
    private Vector3 velocity;

    // New parameter to control the transition from attacking to idle
    private bool isMoving = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        healthSystem = GetComponent<HealthSystem>();
        // Set an initial spawn point if needed
        spawnPoint = transform.position;
        // Make sure your Animator is set in the Inspector
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned!");
        }

        // Hide the cursor and lock it to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        ApplyGravity();

        PlayerDied();

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (moveDirection != Vector3.zero)
        {
            // Set the character's rotation to face the direction of movement
            transform.rotation = Quaternion.LookRotation(moveDirection);
            // Move the character
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

            // Set the animator parameter for walking
            animator.SetBool("isWalking", true);

            // Player is moving, so disable the IsMoving parameter
            isMoving = true;
        }
        else
        {
            // No movement, set the character to idle
            animator.SetBool("isWalking", false);

            // Player is not moving, so enable the IsMoving parameter
            isMoving = false;
        }

        // Set the animator parameter for IsMoving to control the transition
        animator.SetBool("isMoving", isMoving);
    }

    // Function to set the spawn point for the player
    public void SetSpawnPoint(Vector3 newSpawnPoint)
    {
        spawnPoint = newSpawnPoint;
    }

    // Function to respawn the player at the spawn point
    public void Respawn()
    {
        characterController.enabled = false; // Disable the character controller temporarily
        transform.position = spawnPoint;
        characterController.enabled = true; // Re-enable the character controller
        velocity = Vector3.zero; // Reset velocity
        // You may want to reset player health, state, or other relevant variables here.
        healthSystem.currentHealth = healthSystem.maxHealth;
        StartCoroutine(healthSystem.UpdateHealthBarSmoothly());
    }

    // Example of how to call the respawn function when the player dies (you can adapt this to your game logic)
    public void PlayerDied()
    {
        if(healthSystem.currentHealth == 0 && gameObject.name == "Player")
        {
            Respawn();
        }
    }

    void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            velocity.y = 0f;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        characterController.Move(velocity * Time.deltaTime);
    }
}
