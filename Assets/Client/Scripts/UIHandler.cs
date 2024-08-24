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
    public Transform buffCanvas; // Reference to the Buff Canvas
    public GameObject buffTemplate; // Reference to the Buff Template object
    public float buffSpacing = 10f; // Spacing between buffs
    private List<GameObject> activeBuffs = new List<GameObject>();

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

        // Assign the Buff Canvas
        buffCanvas = GameObject.FindWithTag("Buffs").transform;
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

    public void AddBuff(int spellId, Sprite icon, float duration)
    {
        // Clone the BuffTemplate and add it to the buffCanvas
        GameObject newBuff = Instantiate(buffTemplate, buffCanvas);

        // Get the BuffDebuff component and initialize it
        BuffDebuff buffDebuff = newBuff.GetComponent<BuffDebuff>();
        buffDebuff.InitializeBuff(spellId, icon, duration);

        // Position the new buff correctly in the UI
        int row = activeBuffs.Count / 10; // 10 buffs per row, adjust as needed
        int column = activeBuffs.Count % 10;

        RectTransform buffRectTransform = newBuff.GetComponent<RectTransform>();
        buffRectTransform.anchoredPosition = new Vector2(column * (buffRectTransform.sizeDelta.x + buffSpacing), -row * (buffRectTransform.sizeDelta.y + buffSpacing));
        buffDebuff.gameObject.SetActive(true);
        // Add to the list of active buffs
        activeBuffs.Add(newBuff);
    }
}
