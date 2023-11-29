using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonsCanvas;

    [SerializeField]
    private GameObject optionsCanvas;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
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
    }

    public void onOptionsButton()
    {
        buttonsCanvas.SetActive(false);
        optionsCanvas.SetActive(true);
    }

    public void onExitOptionsButton()
    {
        buttonsCanvas.SetActive(true);
        optionsCanvas.SetActive(false);
    }
}
