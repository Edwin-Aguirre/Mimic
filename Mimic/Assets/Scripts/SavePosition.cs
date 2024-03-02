using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePosition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetSpawnPointForAllPlayers(other.transform.position);
            Debug.Log("Spawn Set by Player: " + other.name);
        }
    }

    private void SetSpawnPointForAllPlayers(Vector3 spawnPoint)
    {
        ThirdPersonController[] controllers = GetAllThirdPersonControllers();

        foreach (ThirdPersonController controller in controllers)
        {
            controller.SetSpawnPoint(spawnPoint);
        }
    }

    private ThirdPersonController[] GetAllThirdPersonControllers()
    {
        List<ThirdPersonController> allControllers = new List<ThirdPersonController>();

        // Find all active objects with ThirdPersonController component
        ThirdPersonController[] activeControllers = FindObjectsOfType<ThirdPersonController>();
        allControllers.AddRange(activeControllers);

        // Find all inactive objects with ThirdPersonController component
        GameObject[] inactiveObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in inactiveObjects)
        {
            if (obj.hideFlags == HideFlags.NotEditable || obj.hideFlags == HideFlags.HideAndDontSave)
                continue;

            ThirdPersonController controller = obj.GetComponent<ThirdPersonController>();
            if (controller != null && !allControllers.Contains(controller))
            {
                allControllers.Add(controller);
            }
        }

        return allControllers.ToArray();
    }
}
