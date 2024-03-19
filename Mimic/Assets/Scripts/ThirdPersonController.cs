using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ThirdPersonController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float runSpeedMultiplier = 2.0f;
    public float runDuration = 5.0f;
    public float stopThreshold = 0.1f;
    public float staminaRechargeDelay = 2.0f; // Delay before stamina starts recharging
    public float staminaRechargeRate = 1.0f; // Rate at which stamina recharges per second
    public float gravity = 9.81f;
    public KeyCode runKey = KeyCode.LeftShift; // Define the key for running
    public Slider staminaSlider; // Reference to the UI slider for stamina

    private Vector3 spawnPoint;
    public Animator animator;

    private CharacterController characterController;
    private HealthSystem healthSystem;
    public Vector3 velocity;
    private bool isMoving = false;
    private bool isRunning = false;
    private float runTimer = 0.0f;
    private float stamina = 1.0f; // Current stamina level (normalized between 0 and 1)
    private float lastRunTime = 0.0f;
    private float staminaRechargeDelayTimer = 0.0f;
    private bool isDelayOver = true;
    private bool wasRunning = false;
    private float previousStamina = 1.0f;
    private bool isDying = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        healthSystem = GetComponent<HealthSystem>();
        spawnPoint = transform.position;
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned!");
        }

        // Initialize stamina slider
        if (staminaSlider != null)
        {
            staminaSlider.value = stamina;
        }
    }

    private void Update()
    {
        ApplyGravity();
        PlayerDied();

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (gameObject.CompareTag("Player"))
        {
            staminaSlider.gameObject.SetActive(true);
        }

        // Check if stick button is pressed
        if (Gamepad.current != null && Gamepad.current.leftStickButton.wasPressedThisFrame)
        {
            isRunning = !isRunning; // Toggle running state
            runTimer = 0.0f; // Reset run timer
            lastRunTime = Time.time; // Record the time when running started
        }

        // Check keyboard input for running
        if (Input.GetKeyDown(runKey))
        {
            isRunning = true;
            runTimer = 0.0f; // Reset run timer
            lastRunTime = Time.time; // Record the time when running started
        }
        else if (Input.GetKeyUp(runKey))
        {
            isRunning = false;
            lastRunTime = Time.time; // Record the time when running stopped
        }

        // Update stamina
        UpdateStamina();

        // Update UI slider for stamina
        if (staminaSlider != null)
        {
            staminaSlider.value = stamina;
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
                    lastRunTime = Time.time; // Record the time when running stopped
                }
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
            if (!isMoving)
            {
                isRunning = false; // Stop running if player stopped moving
                lastRunTime = Time.time; // Record the time when running stopped
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

        animator.ResetTrigger("Die");
        animator.SetBool("isAlive", true);
        moveSpeed = 5f;
    }

    public void PlayerDied()
    {
        if (healthSystem.currentHealth == 0 && gameObject.name == "Player" && !isDying)
        {
            StartCoroutine(PlayerDeathAnimation());
        }
    }

    private IEnumerator PlayerDeathAnimation()
    {
        isDying = true;
        animator.SetBool("isAlive", false);

        // Trigger death animation
        animator.SetTrigger("Die");

        moveSpeed = 0f;

        // Wait for the death animation to finish
        yield return new WaitForSeconds(4);

        // Apply death particles then respawn
        healthSystem.deathParticles.gameObject.SetActive(true);
        healthSystem.deathParticles.Play();
        gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
        yield return new WaitForSeconds(0.5f);

        // Respawn the player
        gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
        Respawn();

        isDying = false;
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

    void UpdateStamina()
    {
        if (isRunning)
        {
            // Reduce stamina over the run duration
            float elapsedTime = Time.time - lastRunTime;
            stamina = Mathf.Clamp01(previousStamina - (elapsedTime / runDuration));

            // If stamina is depleted, stop running
            if (stamina <= 0)
            {
                isRunning = false;
            }
            
            // Reset delay timer only when starting to run
            if (!wasRunning) 
            {
                staminaRechargeDelayTimer = 0.0f;
                isDelayOver = false;
            }
            
            wasRunning = true;
        }
        else if (stamina < 1)
        {
            if (!isDelayOver)
            {
                // If delay timer is not over, increment the timer
                staminaRechargeDelayTimer += Time.deltaTime;
                if (staminaRechargeDelayTimer >= staminaRechargeDelay)
                {
                    isDelayOver = true;
                }
            }
            else
            {
                // Start stamina recharge if not already running and delay is over
                stamina += staminaRechargeRate * Time.deltaTime;
                stamina = Mathf.Clamp01(stamina);
            }
            
            // Store the previous stamina value
            previousStamina = stamina;
            wasRunning = false;
        }
    }
}
