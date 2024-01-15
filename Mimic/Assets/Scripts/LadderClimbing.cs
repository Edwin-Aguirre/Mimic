using UnityEngine;

public class LadderClimbing : MonoBehaviour
{
    public float climbingSpeed = 3f;
    private CharacterController characterController;
    private Animator animator;
    private bool isClimbing = false;
    private ThirdPersonController thirdPersonController;
    private float originalAnimationSpeed;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        originalAnimationSpeed = animator.speed;
    }

    void Update()
    {
        // Check if the player is on the ladder
        if (isClimbing)
        {
            float verticalInput = Input.GetAxis("Vertical");

            // Move the player up or down based on input
            Vector3 climbDirection = new Vector3(0f, verticalInput, 0f).normalized;
            Vector3 climbVelocity = climbDirection * climbingSpeed;

            // Set the climbing animation parameter
            animator.SetBool("isClimbing", verticalInput != 0f);

            // Check if the player is not moving vertically
            if (verticalInput == 0f)
            {
                // Pause the climbing animation
                animator.speed = 0f;

                // Adjust the character controller's velocity to prevent falling
                thirdPersonController.velocity = Vector3.zero;
            }
            else
            {
                // Resume the climbing animation
                animator.speed = originalAnimationSpeed;
            }

            // Move the character controller
            characterController.Move(climbVelocity * Time.deltaTime);

            // Disable gravity while climbing
            thirdPersonController.gravity = 0f;
        }
        else
        {
            // Restore original gravity and animation speed when not climbing
            thirdPersonController.gravity = 20f;
            animator.speed = originalAnimationSpeed;
        }
    }

    // OnTriggerEnter is called when the player enters the ladder trigger zone
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = true;
        }
    }

    // OnTriggerExit is called when the player exits the ladder trigger zone
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            isClimbing = false;
            // Reset the climbing animation parameter when leaving the ladder
            animator.SetBool("isClimbing", false);
        }
    }
}
