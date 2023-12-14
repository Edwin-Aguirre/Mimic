using UnityEngine;
using UnityEngine.InputSystem;

public class Mirror : MonoBehaviour
{
    public GameObject mirror;
    public InputAction mirrorButton;
    public string collectibleTag = "Collectible";

    private Animator animator;
    public bool hasMirror;
    private bool isWalking = false;
    private float walkingThreshold = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Animator component attached to the player
        animator = GetComponent<Animator>();

        mirror.SetActive(false);
        hasMirror = false;
        mirrorButton.Enable();

        if (PlayerPrefs.HasKey("AnimationPlayed"))
        {
            hasMirror = PlayerPrefs.GetInt("AnimationPlayed") == 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckWalking();

        if (isWalking)
        {
            StopAnimation();
            HideMirror();
        }
        else if (hasMirror && mirrorButton.IsPressed())
        {
            PlayAnimation();
        }
    }

    void PlayAnimation()
    {
        // Trigger the animation
        animator.SetBool("isUsingMirror", true);
        mirror.SetActive(true);
        PlayerPrefs.SetInt("AnimationPlayed", 1);
        PlayerPrefs.Save();
    }

    void StopAnimation()
    {
        animator.SetBool("isUsingMirror", false);
    }

    void HideMirror()
    {
        mirror.SetActive(false);
    }

    void CheckWalking()
    {
        isWalking = Mathf.Abs(Input.GetAxis("Vertical")) > walkingThreshold || Mathf.Abs(Input.GetAxis("Horizontal")) > walkingThreshold;
    }
}
