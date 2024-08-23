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
    //public CooldownManager cooldownManager;
    //public GameObject actionBar;
    //public GameObject buffDebuffContainer;
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

    /*public void UpdateCooldowns(int spellId, float cooldownTime)
    {
        if (cooldownManager != null)
        {
            cooldownManager.SetCooldown(spellId, cooldownTime);
        }
    }*/

    /*public void ShowBuff(int buffId, Sprite buffIcon)
    {
        // Instantiate or show buff icon in buffDebuffContainer
        GameObject buffIconObj = new GameObject("BuffIcon");
        Image iconImage = buffIconObj.AddComponent<Image>();
        iconImage.sprite = buffIcon;
        buffIconObj.transform.SetParent(buffDebuffContainer.transform);
    }*/

    public void RemoveBuff(int buffId)
    {
        // Remove the specific buff icon from buffDebuffContainer
        // Implementation depends on how you are tracking the buffs
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
