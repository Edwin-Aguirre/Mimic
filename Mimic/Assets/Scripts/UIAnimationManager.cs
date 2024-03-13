using System.Collections;
using System.Collections.Generic;
using HeneGames.DialogueSystem;
using UnityEngine;

public class UIAnimationManager : MonoBehaviour
{
    public void SlideIn()
    {
        Animation animationComponent = DialogueUI.instance.dialogueWindow.GetComponent<Animation>();

        animationComponent.Play("Slide In");
    }

    public void SlideOut()
    {
        Animation animationComponent = DialogueUI.instance.dialogueWindow.GetComponent<Animation>();

        animationComponent.Play("Slide Out");
    }
}
