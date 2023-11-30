using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeTextColor : MonoBehaviour
{
    public TMP_Text text;
    public Color textColorEnter;
    public Color textColorExit;

    public void ChangeColorEnter()
    {
        text.color = textColorEnter;
    }

    public void ChangeColorExit()
    {
        text.color = textColorExit;
    }
}
