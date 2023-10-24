using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;


public class RadialProgressBarUpdater : MonoBehaviour
{
    public InputAction holdAction;  // The action for holding the button
    public Image radialProgressBar;  // The UI radial progress bar

    public Sprite pcSprite;
    public Sprite consoleSprite;

    public float fillSpeed = 1.0f; // The speed at which the fill amount changes
    public UnityEvent myEvent = null;

    private float currentFillAmount = 0.0f;
    private bool isFilling = false;

    private void Awake()
    {
        // Ensure the action map is enabled
        holdAction.Enable();
    }

    private void Update()
    {
        if (holdAction.IsPressed())
        {
            CheckInput();
            
            if (!isFilling)
            {
                isFilling = true;
                radialProgressBar.enabled = true;
            }

            currentFillAmount += Time.deltaTime * fillSpeed;
            radialProgressBar.fillAmount = currentFillAmount;

            if (currentFillAmount >= 1.0f)
            {
                currentFillAmount = 1.0f;
                radialProgressBar.fillAmount = 1.0f;
                radialProgressBar.enabled = false;
                myEvent.Invoke();
            }
        }
        else
        {
            if (isFilling)
            {
                currentFillAmount -= Time.deltaTime * fillSpeed;
                radialProgressBar.fillAmount = currentFillAmount;

                if (currentFillAmount <= 0.0f)
                {
                    currentFillAmount = 0.0f;
                    radialProgressBar.fillAmount = 0.0f;
                    radialProgressBar.enabled = false;
                    isFilling = false;
                }
            }
            else
            {
                // Reset when not holding the button
                currentFillAmount = 0.0f;
                radialProgressBar.fillAmount = 0.0f;
                radialProgressBar.enabled = false;
            }
        }

        if (holdAction.WasReleasedThisFrame())
        {
            isFilling = false;
        }
    }

    private void CheckInput()
    {
        var gamepad = Gamepad.current;

        if (Input.GetKeyDown(KeyCode.E))
        {
            radialProgressBar.sprite = pcSprite;
        } 
        if (gamepad is DualShockGamepad && holdAction.WasPerformedThisFrame())
        {
            radialProgressBar.sprite = consoleSprite;
        }
        else if (gamepad is XInputController && gamepad.name == "DualShock4GamepadHID" && holdAction.WasPerformedThisFrame())
        {
            radialProgressBar.sprite = consoleSprite;
        }
    }
}
