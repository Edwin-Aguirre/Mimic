using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TitleMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonsCanvas;

    [SerializeField]
    private GameObject optionsCanvas;

    [SerializeField]
    private GameObject playButton;

    [SerializeField]
    private GameObject resolutionDropdown;

    [SerializeField]
    private GameObject logo;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        EventSystem.current.SetSelectedGameObject(playButton);
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void onQuitGame()
    {
        StartCoroutine(QuitButtonDelay());
    }

    private IEnumerator QuitButtonDelay()
    {
        yield return new WaitForSeconds(0.352f);
        Application.Quit();
        PlayerPrefs.DeleteAll();
    }

    public void onOptionsButton()
    {
        buttonsCanvas.SetActive(false);
        logo.SetActive(false);
        optionsCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(resolutionDropdown);
    }

    public void onExitOptionsButton()
    {
        buttonsCanvas.SetActive(true);
        logo.SetActive(true);
        optionsCanvas.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(playButton);
    }
}
