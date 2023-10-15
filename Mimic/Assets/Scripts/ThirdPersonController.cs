using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public float moveSpeed = 5.0f; // Default walking speed
    public Animator animator;
    private CharacterController characterController;

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
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (moveDirection != Vector3.zero)
        {
            // Set the character's rotation to face the direction of movement
            transform.rotation = Quaternion.LookRotation(moveDirection);

            // Move the character at the specified speed
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

            // Set the animator parameter for walking
            animator.SetBool("isWalking", true);
        }
        else
        {
            // No movement, set the character to idle
            animator.SetBool("isWalking", false);
        }
    }
}
