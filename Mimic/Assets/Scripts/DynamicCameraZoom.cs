using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;

public class DynamicCameraZoom : MonoBehaviour
{
    public CinemachineVirtualCamera mainCamera;
    public VolumeProfile volume;
    private DepthOfField depthOfField;

    // Reference to the UI camera
    public Camera uiCamera;

    // Camera zoom parameters
    public float minFOV = 40f;
    public float maxFOV = 60f;
    public float zoomSpeed = 2f;

    // Internal reference to the player transform
    private Transform player;

    private void Start()
    {
        mainCamera = GetComponent<CinemachineVirtualCamera>();

        if (volume.TryGet<DepthOfField>(out depthOfField))
        {
            depthOfField.focusDistance.value = 25f;
            depthOfField.focalLength.value = 300f;
        }

        // Find the player object based on the "Player" tag
        FindPlayer();

        // Set the initial FOV of the UI camera to match the Cinemachine camera
        if (uiCamera != null)
        {
            uiCamera.fieldOfView = mainCamera.m_Lens.FieldOfView;
        }
    }

    private void Update()
    {
        FindPlayer();
        
        // Check if player is not null (optional)
        if (player != null)
        {
            // Calculate the normalized Y position of the player
            float normalizedY = Mathf.InverseLerp(0f, 100f, player.position.y);

            // Smoothly interpolate between minFOV and maxFOV based on the player's Y position
            float targetFOV = Mathf.Lerp(minFOV, maxFOV, normalizedY);

            // Smoothly adjust the camera's FOV
            mainCamera.m_Lens.FieldOfView = Mathf.Lerp(mainCamera.m_Lens.FieldOfView, targetFOV, Time.deltaTime * zoomSpeed);

            // Synchronize UI camera FOV with Cinemachine camera FOV
            if (uiCamera != null)
            {
                uiCamera.fieldOfView = mainCamera.m_Lens.FieldOfView;
            }
        }
        else
        {
            // If the player object is null, attempt to find it again
            FindPlayer();
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player object not found. Make sure it has the 'Player' tag.");
        }
    }
}
