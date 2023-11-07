using System.Collections;
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

    private Material emissionMaterial;  // Material for beacon's emission effect
    private Material regularMaterial;   // Material for the regular appearance

    private float emissionLerp = 0f; // Used for lerping emission intensity

    private bool isActive = false; // Track if the beacon is currently active

    private static SpawnBeacon previousActiveBeacon; // Track the previously activated beacon

    private void Start()
    {
        setSpawnKey.Enable();
        setSpawnKey.performed += _ => HandleSetSpawnKey();

        // Initialize materials and disable emission at the start
        emissionMaterial = GetComponent<Renderer>().materials[1]; 
        regularMaterial = GetComponent<Renderer>().materials[0]; 

        DisableEmission(emissionMaterial);
    }

    private void HandleSetSpawnKey()
    {
        if (playerNearSpawnpoint && !isActive)
        {
            if (previousActiveBeacon != null)
            {
                previousActiveBeacon.Deactivate(activatingPlayer);
            }
            SetPlayerSpawnPoint(activatingPlayer);
        }
    }

    public void SetPlayerSpawnPoint(Transform player)
    {
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
            isActive = true;

            EnableEmission(emissionMaterial);
            DisableEmission(regularMaterial);

            // Smoothly interpolate the emission intensity
            StartCoroutine(LerpEmissionIntensity(emissionMaterial, 0f, 2f, 1.0f)); // Adjust the duration as needed

            // Set this beacon as the previously activated beacon
            previousActiveBeacon = this;
        }
    }

    public void Deactivate(Transform player)
    {
        if (activatingPlayer == player)
        {
            activatingPlayer = null;
            Debug.Log("Spawn Beacon Deactivated for Player: " + player.name);
        }
        
        isActive = false;

        DisableEmission(emissionMaterial);
        EnableEmission(regularMaterial);

        ResetEmissionIntensity(emissionMaterial);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearSpawnpoint = true;
            activatingPlayer = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearSpawnpoint = false;
        }
    }

    private void EnableEmission(Material material)
    {
        material.EnableKeyword("_EMISSION");
    }

    private void DisableEmission(Material material)
    {
        material.DisableKeyword("_EMISSION");
    }

    private void ResetEmissionIntensity(Material material)
    {
        material.SetColor("_EmissionColor", Color.black);
    }

    // Coroutine to smoothly interpolate emission intensity
    private IEnumerator LerpEmissionIntensity(Material material, float startIntensity, float targetIntensity, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duration;
            emissionLerp = Mathf.Lerp(startIntensity, targetIntensity, t);
            material.SetColor("_EmissionColor", Color.white * emissionLerp);
            yield return null;
        }
    }
}
