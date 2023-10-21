using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public class ButtonPrompt : MonoBehaviour
{
    public GameObject buttonPromptUI;
    public Image buttonImage;

    public Sprite pcButtonSprite;
    public Sprite psButtonSprite;
    public Sprite xboxButtonSprite;

    public InputAction pcButton;
    public InputAction psButton;
    public InputAction xboxButton;

    public enum InputType { PC, PlayStation, Xbox }
    public InputType currentInput = InputType.PC; // Default to PC

    private bool playerNearObject = false;

    private void Start()
    {
        buttonPromptUI.SetActive(false);
        pcButton.Enable();
        psButton.Enable();
        xboxButton.Enable();
    }

    private void Update()
    {
        if (playerNearObject)
        {
            SetButtonPrompt();
        }
        else
        {
            buttonPromptUI.SetActive(false);
        }
    }

    void SetButtonPrompt()
    {
        switch (currentInput)
        {
            case InputType.PC:
                buttonImage.sprite = pcButtonSprite;
                break;
            case InputType.PlayStation:
                buttonImage.sprite = psButtonSprite;
                break;
            case InputType.Xbox:
                buttonImage.sprite = xboxButtonSprite;
                break;
        }

        var gamepad = Gamepad.current;

        if (pcButton.WasPerformedThisFrame())
        {
            currentInput = InputType.PC;
        } 
        if (gamepad is DualShockGamepad && psButton.WasPerformedThisFrame())
        {
            currentInput = InputType.PlayStation;
        }
        else if (gamepad is XInputController && gamepad.name == "DualShock4GamepadHID" && xboxButton.WasPerformedThisFrame())
        {
            currentInput = InputType.Xbox;
        }
        buttonPromptUI.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearObject = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearObject = false;
            buttonPromptUI.SetActive(false);
        }
    }
}
