using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TypeChart : MonoBehaviour
{
    [SerializeField] private GameObject typeChartScreen;
    [SerializeField] private InputAction escapeButton;

    [SerializeField] private string scriptsToDisable; // Names of scripts to disable, separated by commas

    private bool isGamePaused = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        escapeButton.Enable();
    }

    // Update is called once per frame
    void Update()
    {
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
        typeChartScreen.SetActive(false);
        isGamePaused = false;
        Cursor.visible = false;
        EnableScripts(scriptsToDisable);
    }

    void Pause()
    {
        typeChartScreen.SetActive(true);
        isGamePaused = true;
        Cursor.visible = true;
        DisableScripts(scriptsToDisable);
    }

    public void onClickSettingsButton()
    {
        typeChartScreen.SetActive(true);
        Cursor.visible = true;
        DisableScripts(scriptsToDisable);
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
