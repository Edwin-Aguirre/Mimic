using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera; // Reference to your Cinemachine Virtual Camera

    private void Start()
    {
        // Start with no target
        //virtualCamera.Follow = null;
        //virtualCamera.LookAt = null;
    }

    private void Update()
    {
        // During gameplay, continuously check for the GameObject with the "Player" tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Assign the "Follow" and "Look At" targets to the Virtual Camera
            virtualCamera.Follow = player.transform;
            virtualCamera.LookAt = player.transform;
        }
    }
}
