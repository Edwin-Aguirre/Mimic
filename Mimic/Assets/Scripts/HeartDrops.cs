using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartDrops : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") )
        {
            HealthSystem healthSystem = other.GetComponent<HealthSystem>();
            healthSystem.currentHealth += 10;
            healthSystem.currentHealth = Mathf.Clamp(healthSystem.currentHealth, 0, healthSystem.maxHealth);
            StartCoroutine(healthSystem.UpdateHealthBarSmoothly());
            healthSystem.UpdateHealthUI();
            SoundManager.PlaySound("select 3");
            SoundManager.audioSource.pitch = 1;
            Destroy(gameObject);
        }
    }
}
