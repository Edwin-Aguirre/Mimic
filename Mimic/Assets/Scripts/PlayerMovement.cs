using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  // Adjust this value to control player movement speed
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get input from the player
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate the movement vector
        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);

        // Normalize the movement vector to ensure diagonal movement is not faster
        if (movement.magnitude > 1f)
        {
            movement.Normalize();
        }

        // Apply movement to the Rigidbody
        rb.velocity = movement * moveSpeed;
    }
}
