using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light lightSource;
    public float cycleDuration = 60.0f; // Duration of one day/night cycle in seconds
    public float minTemperature = 1500.0f; // Minimum temperature (night)
    public float maxTemperature = 20000.0f; // Maximum temperature (day)
    public float currentTemperature = 1500.0f; // Initial temperature
    public bool reverseCycle = false; // Whether to reverse the cycle when reaching max temperature

    private float timer = 0.0f;
    private float temperatureDirection = 1.0f;

    void Start()
    {
        if (lightSource == null)
        {
            lightSource = GetComponent<Light>();
        }

        // Ensure the light source is not null
        if (lightSource == null)
        {
            Debug.LogError("No Light component found. Attach a Light component or set 'lightSource' in the inspector.");
            enabled = false;
        }

        // Set the initial temperature
        lightSource.colorTemperature = currentTemperature;
    }

    void Update()
    {
        // Update the timer
        timer += Time.deltaTime;

        // Calculate the current temperature value
        currentTemperature += temperatureDirection * ((maxTemperature - minTemperature) / cycleDuration) * Time.deltaTime;

        // Check if it's time to reverse the cycle
        if (currentTemperature >= maxTemperature)
        {
            if (reverseCycle)
            {
                currentTemperature = maxTemperature;
                temperatureDirection = -1.0f;
            }
            else
            {
                currentTemperature = minTemperature;
            }
        }
        else if (currentTemperature <= minTemperature)
        {
            currentTemperature = minTemperature;
            temperatureDirection = 1.0f;
        }

        // Update the light's temperature value
        lightSource.colorTemperature = currentTemperature;
    }
}
