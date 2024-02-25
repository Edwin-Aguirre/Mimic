using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public static PlayerScript instance;

    // Start is called before the first frame update
    void Start()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Function to transfer the PlayerScript to a new player object
    public void TransferToNewPlayer(GameObject newPlayer)
    {
        // Ensure the new player object has a PlayerScript component
        PlayerScript newPlayerScript = newPlayer.GetComponent<PlayerScript>();
        if (newPlayerScript == null)
        {
            newPlayerScript = newPlayer.AddComponent<PlayerScript>();
        }

        instance = newPlayerScript; // Update instance reference to the new player
        //Destroy(gameObject); // Destroy the old player
    }

    // Update is called once per frame
    void Update()
    {

    }
}
