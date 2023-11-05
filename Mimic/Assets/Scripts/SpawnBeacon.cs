using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnBeacon : MonoBehaviour
{
    // The spawn point associated with this beacon
    public Transform spawnPoint;

    // The distance within which the player can activate the beacon
    public float activationRadius = 5f;

    public InputAction setSpawnKey;

    private Transform activatingPlayer; // Track the player who activated this beacon

    private bool playerNearSpawnpoint = false;

    private void Start()
    {
        setSpawnKey.Enable();
        setSpawnKey.performed += _ => HandleSetSpawnKey();
    }

    private void HandleSetSpawnKey()
    {
        if (playerNearSpawnpoint)
        {
            SetPlayerSpawnPoint(activatingPlayer);
        }
    }

    // Function to set the player's spawn point when activated
    public void SetPlayerSpawnPoint(Transform player)
    {
        // Allow the latest player to override any previous activation
        ThirdPersonController thirdPersonController = player.GetComponent<ThirdPersonController>();

        if (thirdPersonController != null)
        {
            thirdPersonController.SetSpawnPoint(spawnPoint.position);
            Debug.Log("Spawn Set by Player: " + player.name);

            // Deactivate the beacon for any previously activating player
            if (activatingPlayer != null && activatingPlayer != player)
            {
                Deactivate(activatingPlayer);
            }

            activatingPlayer = player;
        }
    }

    // Function to deactivate the beacon for a specific player
    public void Deactivate(Transform player)
    {
        if (activatingPlayer == player)
        {
            activatingPlayer = null;
            Debug.Log("Spawn Beacon Deactivated for Player: " + player.name);
        }
    }

    // Add a trigger zone to activate the beacon when the player enters the zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearSpawnpoint = true;
            activatingPlayer = other.transform;
        }
    }

    // Reset the playerNearSpawnpoint flag when the player exits the trigger zone
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearSpawnpoint = false;
        }
    }
}
