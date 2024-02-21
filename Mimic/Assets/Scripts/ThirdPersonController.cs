using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float runSpeedMultiplier = 2.0f;
    public float runDuration = 5.0f;
    public float stopThreshold = 0.1f;
    public float gravity = 9.81f;
    public KeyCode runKey = KeyCode.LeftShift; // Define the key for running

    private Vector3 spawnPoint;
    public Animator animator;

    private CharacterController characterController;
    private HealthSystem healthSystem;
    public Vector3 velocity;
    private bool isMoving = false;
    private bool isRunning = false;
    private float runTimer = 0.0f;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        healthSystem = GetComponent<HealthSystem>();
        spawnPoint = transform.position;
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned!");
        }
    }

    private void Update()
    {
        ApplyGravity();
        PlayerDied();

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Check if stick button is pressed
        if (Gamepad.current != null && Gamepad.current.leftStickButton.wasPressedThisFrame)
        {
            isRunning = !isRunning; // Toggle running state
            runTimer = 0.0f; // Reset run timer
        }

        // Check keyboard input for running
        if (Input.GetKeyDown(runKey))
        {
            isRunning = true;
            runTimer = 0.0f; // Reset run timer
        }
        else if (Input.GetKeyUp(runKey))
        {
            isRunning = false;
        }

        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        float currentMoveSpeed = isRunning ? moveSpeed * runSpeedMultiplier : moveSpeed;

        isMoving = moveDirection.magnitude > stopThreshold;

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
            characterController.Move(moveDirection * currentMoveSpeed * Time.deltaTime);
            animator.SetBool("isWalking", true);

            if (isRunning)
            {
                runTimer += Time.deltaTime;
                if (runTimer >= runDuration)
                {
                    isRunning = false; // Stop running after duration expires
                }
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
            if (!isMoving)
            {
                isRunning = false; // Stop running if player stopped moving
            }
        }

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", isRunning);
    }

    public void SetSpawnPoint(Vector3 newSpawnPoint)
    {
        spawnPoint = newSpawnPoint;
    }

    public void Respawn()
    {
        characterController.enabled = false;
        transform.position = spawnPoint;
        characterController.enabled = true;
        velocity = Vector3.zero;
        healthSystem.currentHealth = healthSystem.maxHealth;
        StartCoroutine(healthSystem.UpdateHealthBarSmoothly());
    }

    public void PlayerDied()
    {
        if (healthSystem.currentHealth == 0 && gameObject.name == "Player")
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
