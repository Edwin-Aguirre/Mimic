using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class Mirror : MonoBehaviour
{
    public GameObject mirror;
    public InputAction mirrorButton;
    public string collectibleTag = "Collectible";

    private Animator animator;
    private bool hasMirror;
    private bool isButtonPressed = false;
    private bool isCooldown = false;

    // Cooldown duration in seconds
    private float cooldownDuration = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Animator component attached to the player
        animator = GetComponent<Animator>();

        mirror.SetActive(false);
        hasMirror = false;
        mirrorButton.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCooldown)
        {
            // Check for button press
            if (mirrorButton.IsPressed() && hasMirror == true)
            {
                // If the button was not pressed before, start the animation and show the mirror
                if (!isButtonPressed)
                {
                    PlayAnimation();
                    mirror.SetActive(true);
                    isButtonPressed = true;
                }
            }
            else
            {
                // If the button was pressed before, stop the animation and start the coroutine to hide the mirror
                if (isButtonPressed)
                {
                    animator.SetBool("isUsingMirror", false);
                    StartCoroutine(HideMirror());
                    isButtonPressed = false;

                    // Start the cooldown
                    StartCoroutine(Cooldown());
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(collectibleTag))
        {
            Debug.Log("Got Mirror");
            // Handle collectible item logic
            CollectItem(other.gameObject);
            hasMirror = true;
        }
    }

    void CollectItem(GameObject collectible)
    {
        // Deactivate the collectible item
        collectible.SetActive(false);
    }

    void PlayAnimation()
    {
        // Trigger the animation
        animator.SetBool("isUsingMirror", true);
        mirror.SetActive(true);
    }

    IEnumerator HideMirror()
    {
        // Wait for 1 second
        yield return new WaitForSeconds(0.5f);

        // Deactivate the mirror after 1 second
        mirror.SetActive(false);
    }

    IEnumerator Cooldown()
    {
        // Set cooldown flag to true
        isCooldown = true;

        // Wait for the cooldown duration
        yield return new WaitForSeconds(cooldownDuration);

        // Reset the cooldown flag
        isCooldown = false;
    }
}
