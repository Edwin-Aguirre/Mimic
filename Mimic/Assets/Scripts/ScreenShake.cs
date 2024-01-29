using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance; // Singleton instance

    private CinemachineVirtualCamera virtualCamera;
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        // Singleton pattern to access the instance from other scripts
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Get references to the Virtual Camera and Impulse Source components
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // Call this method to trigger a screen shake
    public void ShakeScreen(float amplitude, float frequency)
    {
        impulseSource.m_ImpulseDefinition.m_AmplitudeGain = amplitude;
        impulseSource.m_ImpulseDefinition.m_FrequencyGain = frequency;

        impulseSource.GenerateImpulse();
    }
}
