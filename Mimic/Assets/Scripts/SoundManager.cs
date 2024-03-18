using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager: MonoBehaviour
{
    public static AudioClip enemyHitSound;
    public static AudioClip playerHitSound;
    public static AudioClip questStartedSound;
    public static AudioClip questCompletedSound;
    public static AudioClip objectPickupSound;
    public static AudioClip cannotTransformSound;


    public static AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        enemyHitSound = Resources.Load<AudioClip>("hurt 1");
        playerHitSound = Resources.Load<AudioClip>("hurt 2");
        questStartedSound = Resources.Load<AudioClip>("loading 3");
        questCompletedSound = Resources.Load<AudioClip>("power 6");
        objectPickupSound = Resources.Load<AudioClip>("select 3");
        cannotTransformSound = Resources.Load<AudioClip>("select 12");
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
            case "loading 3":
                audioSource.PlayOneShot(questStartedSound);
                break;
            case "power 6":
                audioSource.PlayOneShot(questCompletedSound);
                break;
            case "select 3":
                audioSource.PlayOneShot(objectPickupSound);
                break;
            case "select 12":
                audioSource.PlayOneShot(cannotTransformSound);
                break;                
        }
    }
}
