using UnityEngine;

public class AnimationControllerSwap : MonoBehaviour
{
    public RuntimeAnimatorController newAnimatorController; // Reference to the new Animator controller

    public void SwapAnimatorController()
    {
        Animator enemyAnimator = GetComponent<Animator>();
        enemyAnimator.runtimeAnimatorController = newAnimatorController;
    }
}
