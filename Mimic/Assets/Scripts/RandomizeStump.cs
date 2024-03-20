using UnityEngine;
using System.Collections.Generic;

public class RandomizeStump : MonoBehaviour
{
    // Number of game objects to enable the script on
    public int numberOfObjectsToEnableScript = 1;

    // The script to enable
    private QuestTracker scriptToEnable;

    // The material to apply
    public Material materialToApply;

    // Array of game objects
    public GameObject[] targetObjects;

    void Start()
    {
        
    }

    public void EnableRandomScript()
    {
        // Shuffle the array of game objects
        Shuffle(targetObjects);

        // Enable script on a certain number of objects
        for (int i = 0; i < Mathf.Min(numberOfObjectsToEnableScript, targetObjects.Length); i++)
        {
            scriptToEnable = targetObjects[i].GetComponent<QuestTracker>();

            // Enable the script on the object
            scriptToEnable.enabled = true;

            // Apply the material to the object
            Renderer renderer = targetObjects[i].GetComponent<Renderer>();
            if (renderer != null && materialToApply != null)
            {
                renderer.material = materialToApply;
            }

            // Access the collider and enable isTrigger if it exists
            Collider collider = targetObjects[i].GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }
        }
    }

    // Function to shuffle the array
    void Shuffle(GameObject[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GameObject temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}
