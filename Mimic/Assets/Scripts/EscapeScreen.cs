using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;

public class EscapeScreen : MonoBehaviour
{
    [SerializeField] private GameObject escapeScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject optionsButton;
    [SerializeField] private GameObject controlsScreen;
    [SerializeField] private GameObject controlsButton;
    [SerializeField] private GameObject resolutionDropdown;
    [SerializeField] private GameObject keyboardPanel;
    [SerializeField] private GameObject playstationPanel;
    [SerializeField] private GameObject xboxPanel;
    [SerializeField] private InputAction escapeButton;

    [SerializeField] private string scriptsToDisable; // Names of scripts to disable, separated by commas

    private bool isGamePaused = false;

    public enum InputType { PC, PlayStation, Xbox }
    public InputType currentInput = InputType.PC; // Default to PC

    private string disableController;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        escapeButton.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        SetPlatformPanel();

        if (escapeButton.WasPressedThisFrame())
        {
            if (isGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Resume()
    {
        escapeScreen.SetActive(false);
        optionsScreen.SetActive(false);
        controlsScreen.SetActive(false);
        isGamePaused = false;
        Cursor.visible = false;
        EnableScripts(scriptsToDisable);
    }

    void Pause()
    {
        escapeScreen.SetActive(true);
        isGamePaused = true;
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(optionsButton);
        DisableScripts(scriptsToDisable);
    }

    public void onClickSettingsButton()
    {
        optionsScreen.SetActive(true);
        escapeScreen.SetActive(false);
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(resolutionDropdown);
        DisableScripts(scriptsToDisable);
    }

    public void onClickControlsButton()
    {
        controlsScreen.SetActive(true);
        escapeScreen.SetActive(false);
        Cursor.visible = true;
        DisableScripts(scriptsToDisable);
    }

    void SetPlatformPanel()
    {
        var gamepad = Gamepad.current;

        if (escapeButton.WasPerformedThisFrame())
        {
            currentInput = InputType.PC;
        } 
        if (gamepad is DualShockGamepad && escapeButton.WasPerformedThisFrame())
        {
            currentInput = InputType.PlayStation;
            disableController = "DualShock4GamepadHID";
        }
        if (gamepad is XInputController && gamepad.name == disableController && escapeButton.WasPerformedThisFrame())
        {
            currentInput = InputType.Xbox;
        }
        else if (gamepad is XInputController)
        {
            disableController = "XInputControllerWindows";
        }
        
        switch (currentInput)
        {
            case InputType.PC:
                keyboardPanel.SetActive(true);
                playstationPanel.SetActive(false);
                xboxPanel.SetActive(false);
                break;
            case InputType.PlayStation:
                playstationPanel.SetActive(true);
                keyboardPanel.SetActive(false);
                xboxPanel.SetActive(false);
                break;
            case InputType.Xbox:
                xboxPanel.SetActive(true);
                keyboardPanel.SetActive(false);
                playstationPanel.SetActive(false);
                break;
        }
    }

    // Disable scripts on game objects with the specified name
    private void DisableScripts(string scriptNames)
    {
        string[] scriptArray = scriptNames.Split(',');

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            foreach (var scriptName in scriptArray)
            {
                MonoBehaviour script = player.GetComponent(scriptName.Trim()) as MonoBehaviour;
                if (script != null)
                {
                    script.enabled = false;
                }
            }
        }
    }

    // Enable scripts on game objects with the specified name
    private void EnableScripts(string scriptNames)
    {
        string[] scriptArray = scriptNames.Split(',');

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            foreach (var scriptName in scriptArray)
            {
                MonoBehaviour script = player.GetComponent(scriptName.Trim()) as MonoBehaviour;
                if (script != null)
                {
                    script.enabled = true;
                }
            }
        }
    }
}
