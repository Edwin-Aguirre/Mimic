using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public class InteractButtonPrompt : MonoBehaviour
{
    public GameObject interactPanel;
    public Image buttonImage;
    public Image attackImage;

    public Sprite pcButtonSprite;
    public Sprite psButtonSprite;
    public Sprite xboxButtonSprite;

    public Sprite pcAttackButtonSprite;
    public Sprite psAttackButtonSprite;
    public Sprite xboxAttackButtonSprite;

    public InputAction pcButton;
    public InputAction psButton;
    public InputAction xboxButton;

    public enum InputType { PC, PlayStation, Xbox }
    public InputType currentInput = InputType.PC; // Default to PC

    private bool playerNearInteractable = false;

    private bool hasInteracted = false;
    private CameraZoom cameraZoom;

    private string disableController;

    private QuestInitializer questInitializer; // Reference to the QuestInitializer script

    private bool isPanelOpen = false;

    [SerializeField]
    private bool hasQuest;

    private void Start()
    {
        interactPanel.SetActive(false);
        pcButton.Enable();
        psButton.Enable();
        xboxButton.Enable();
        cameraZoom = FindAnyObjectByType<CameraZoom>();
        disableController = "DualShock4GamepadHID";
        questInitializer = GetComponent<QuestInitializer>();
    }

    private void Update()
    {
        SetButtonPrompt();
        var gamepad = Gamepad.current;
        if (playerNearInteractable)
        {
            if (pcButton.WasPerformedThisFrame() || (gamepad is DualShockGamepad && psButton.WasPerformedThisFrame()) || (gamepad is XInputController && gamepad.name == disableController && xboxButton.WasPerformedThisFrame()))
            {
                TogglePanel();
            }
        }

        // Check if the panel is closed and hasInteracted is true
        if (!isPanelOpen && hasInteracted && hasQuest == true)
        {
            // Call the method to start the quest after the panel is closed
            questInitializer.StartQuestAfterPanelClosed();
            hasInteracted = false; // Reset hasInteracted after starting the quest
        }
    }

    void TogglePanel()
    {
        interactPanel.SetActive(!interactPanel.activeSelf);
        hasInteracted = true;
        cameraZoom.ToggleCameraAndBokehSettings();

        // Set the flag indicating the panel status
        isPanelOpen = interactPanel.activeSelf;
    }

    void SetButtonPrompt()
    {
        switch (currentInput)
        {
            case InputType.PC:
                buttonImage.sprite = pcButtonSprite;
                attackImage.sprite = pcAttackButtonSprite;
                break;
            case InputType.PlayStation:
                buttonImage.sprite = psButtonSprite;
                attackImage.sprite = psAttackButtonSprite;
                break;
            case InputType.Xbox:
                buttonImage.sprite = xboxButtonSprite;
                attackImage.sprite = xboxAttackButtonSprite;
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
            disableController = "DualShock4GamepadHID";
        }
        if (gamepad is XInputController && gamepad.name == disableController && xboxButton.WasPerformedThisFrame())
        {
            currentInput = InputType.Xbox;
        }
        else if (gamepad is XInputController)
        {
            disableController = "XInputControllerWindows";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearInteractable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearInteractable = false;
            interactPanel.SetActive(false);

            if (hasInteracted)
            {
                cameraZoom.isCameraEnabled = false;
                hasInteracted = false;
                cameraZoom.ToggleCameraAndBokehSettings();
            }
        }
    }
}
