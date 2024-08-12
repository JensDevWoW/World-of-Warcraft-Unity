using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image bar;              // Reference to the Image component representing the health bar
    public GameObject healthBar;

    private float maxHealth = 1000; // Assuming max health is 100
    private float currentHealth = 1000;    // Current health value

    void Start()
    {
        // Initialize the health to max health at the start
        currentHealth = maxHealth;
        UpdateHealthBar(maxHealth);
    }

    void Update()
    {
        // You can call UpdateHealthBar here if needed, but ideally, it should be called only when health changes
    }

    // Method to update the health bar
    public void ChangeHealth(float amount, float maxHealth)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateHealthBar(maxHealth);
    }

    // Method to update the visual representation of the health bar
    private void UpdateHealthBar(float maxHealth)
    {
        // Calculate the fill amount based on the current health
        float fillAmount = currentHealth / maxHealth;
        bar.fillAmount = Mathf.Clamp01(fillAmount);
    }
}
