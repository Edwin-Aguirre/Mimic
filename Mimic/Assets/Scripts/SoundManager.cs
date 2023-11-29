using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager: MonoBehaviour
{
    public static AudioClip menuMoveSound;
    public static AudioClip menuClickSound;


    static AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        menuMoveSound = Resources.Load<AudioClip>("select 2");
        menuClickSound = Resources.Load<AudioClip>("select 1");
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "sfx_menu_move4":
                audioSource.PlayOneShot(menuMoveSound);
                break;
            case "sfx_menu_select1":
                audioSource.PlayOneShot(menuClickSound);
                break;
        }
    }
}
