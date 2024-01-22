using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager: MonoBehaviour
{
    public static AudioClip enemyHitSound;
    public static AudioClip playerHitSound;


    public static AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        enemyHitSound = Resources.Load<AudioClip>("hurt 1");
        playerHitSound = Resources.Load<AudioClip>("hurt 2");
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
            case "hurt 1":
                audioSource.PlayOneShot(enemyHitSound);
                break;
            case "hurt 2":
                audioSource.PlayOneShot(playerHitSound);
                break;
        }
    }
}
