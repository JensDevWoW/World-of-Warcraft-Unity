using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIHandler : MonoBehaviour
{
    // Singleton instance
    public static UIHandler Instance { get; private set; }

    [Header("UI Elements")]
    public CastBar castBar;
    public HealthBar healthBar;
    public List<ActionButton> actionButtons; // List of all ActionButton instances
    public Text combatText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GameObject canvas = GameObject.FindWithTag("Canvas");
        castBar = canvas.GetComponent<CastBar>();
        healthBar = canvas.GetComponent<HealthBar>();

        // Initialize the list of action buttons
        actionButtons = new List<ActionButton>(canvas.GetComponentsInChildren<ActionButton>());
    }

    public void StartCast(float castTime, string name)
    {
        if (castBar != null)
        {
            castBar.StartCast(castTime, name);
        }
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void UpdateCooldown(int spellId, float cooldownTime)
    {
        foreach (var button in actionButtons)
        {
            if (button.spellId == spellId)
            {
                button.StartCooldown(cooldownTime);
            }
        }
    }

    public void StartGlobalCooldown(float gcdDuration)
    {
        foreach (var button in actionButtons)
        {
            // Only start the GCD if the button isn't already on cooldown
            if (!button.IsOnCooldown())
            {
                button.StartCooldown(gcdDuration);
            }
        }
    }

    public void DisplayCombatText(string text, Color color)
    {
        if (combatText != null)
        {
            combatText.text = text;
            combatText.color = color;
            // Add additional animations or fading effects if needed
        }
    }

    public void UpdateButtonId(GameObject buttonObj, int spellId)
    {
        if (buttonObj != null)
        {
            foreach (var button in actionButtons)
            {
                if (button.gameObject == buttonObj)
                {
                    button.spellId = spellId;
                    return;
                }
            }
        }
    }

    public void ShowAoETargetingIndicator(Vector3 position)
    {
        // Show AoE targeting UI element at the specified position
        // This could be a circle or other indicator showing where the AoE will land
    }

    public void HideAoETargetingIndicator()
    {
        // Hide the AoE targeting UI element
    }
}
