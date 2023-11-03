using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float gravity = 9.81f;
    public Animator animator;

    private CharacterController characterController;
    private Vector3 velocity;

    // New parameter to control the transition from attacking to idle
    private bool isMoving = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        // Make sure your Animator is set in the Inspector
        if (animator == null)
        {
            Debug.LogError("Animator is not assigned!");
        }
    }

    private void Update()
    {
        ApplyGravity();

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
