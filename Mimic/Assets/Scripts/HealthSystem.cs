using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Slider healthSlider; // Reference to the UI Slider for health display
    public float healthChangeSpeed = 5f; // The speed at which the health bar updates

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Start a coroutine to smoothly update the health bar
        StartCoroutine(UpdateHealthBarSmoothly());

        if (currentHealth == 0)
        {
            if(!gameObject.CompareTag("Player"))
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        Destroy(gameObject);
    }

    public IEnumerator UpdateHealthBarSmoothly()
    {
        float targetValue = (float)currentHealth / maxHealth;
        float startValue = healthSlider.value;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * healthChangeSpeed;
            healthSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime);
            yield return null;
        }

        // Ensure the health bar reaches the exact target value
        healthSlider.value = targetValue;
    }

    public void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }
}
