using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera zoomCamera; // The camera whose FOV will be changed
    public VolumeProfile volume;
    private DepthOfField depthOfField;

    public bool isCameraEnabled = true;

    private void Start()
    {
        mainCamera = GetComponent<CinemachineVirtualCamera>();
        if (volume.TryGet<DepthOfField>(out depthOfField))
        {
            depthOfField.focusDistance.value = 25f;
            depthOfField.focalLength.value = 300f;
        }
    }

    public void ToggleCameraAndBokehSettings()
    {
        isCameraEnabled = !isCameraEnabled;

        // Toggle camera state
        mainCamera.enabled = isCameraEnabled;

        if (volume.TryGet<DepthOfField>(out depthOfField))
        {
            depthOfField.focusDistance.value = isCameraEnabled ? 25f : 15f;
            depthOfField.focalLength.value = isCameraEnabled ? 300f : 170f;
        }

        // Change the FOV of the target camera
        ChangeFOVOfTargetCamera();
    }

    private void ChangeFOVOfTargetCamera()
    {
        // Check if both cameras are not null
        if (mainCamera != null && zoomCamera != null)
        {
            // Change the FOV of the target camera to match the main camera
            zoomCamera.m_Lens.FieldOfView = mainCamera.m_Lens.FieldOfView;
        }
    }
}
