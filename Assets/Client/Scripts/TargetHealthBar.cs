using UnityEngine;
using UnityEngine.UI;

public class TargetHealthBar : MonoBehaviour
{
    public Image bar;  // Reference to the Image component representing the health bar

    public void Start()
    {
        UpdateHealth(1000, 1000);
    }
    // Method to update the health bar with the new health and max health values
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        // Ensure the health values are within valid ranges
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Calculate the fill amount based on the current health
        float fillAmount = currentHealth / maxHealth;

        // Update the UI element
        bar.fillAmount = Mathf.Clamp01(fillAmount);
    }
}
