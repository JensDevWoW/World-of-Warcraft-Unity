using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using System.Runtime.Versioning;

public class UIHandler : MonoBehaviour
{
    // Singleton instance
    public static UIHandler Instance { get; private set; }

    [Header("UI Elements")]
    public CastBar castBar;
    public HealthBar healthBar;
    public TargetHealthBar targetHealthBar;
    public GameObject targetFrame;
    public GameObject thbObject;
    public GameObject tmbObject;
    public List<ActionButton> actionButtons; // List of all ActionButton instances
    public Text combatText;
    public Transform buffCanvas; // Reference to the Buff Canvas
    public Transform targetDebuffCanvas;
    public GameObject targetDebuffTemplate;
    public GameObject buffTemplate; // Reference to the Buff Template object
    public float buffSpacing = 10f; // Spacing between buffs
    private List<GameObject> activeBuffs = new List<GameObject>();
    private List<GameObject> activeTargetDebuffs = new List<GameObject>();
    public Unit target;
    private BuffPosition debuffPos;
    private BuffPosition buffPos;
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
        targetHealthBar = canvas.GetComponent<TargetHealthBar>();
        // Initialize the list of action buttons
        actionButtons = new List<ActionButton>(canvas.GetComponentsInChildren<ActionButton>());

        // Assign the Buff Canvas
        buffCanvas = GameObject.FindWithTag("Buffs").transform;
        buffPos = buffCanvas.GetComponent<BuffPosition>();
        targetDebuffCanvas = canvas.transform.Find("TargetDebuffs");
        debuffPos = targetDebuffCanvas.GetComponent<BuffPosition>();
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

    public void TargetHealthUpdate(float currentHealth, float maxHealth)
    {
        if (targetHealthBar != null)
        {
            targetHealthBar.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void HandleSpellFailed(int spellId, string reason)
    {
        if (reason == "cancelled")
        {
            if (castBar != null)
            {
                castBar.CastFailed();
            }
        }
    }

    public void StartCooldown(int spellId, float cooldownTime)
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
                button.StartGlobalCooldown(gcdDuration);
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

    public void SetTarget(Unit unit)
    {
        this.target = unit;
    }

    public Unit GetTarget()
    {
        return this.target;
    }

    public void UpdateTarget(Unit unit)
    {
        if (unit == null)
            return;

        thbObject.SetActive(true);
        targetFrame.SetActive(true);
        tmbObject.SetActive(true);
        targetDebuffCanvas.gameObject.SetActive(true);
        Transform healthBarTransform = unit.transform.Find("HealthBarVisual");

        if (healthBarTransform != null)
        {
            GameObject healthBarObj = null;
            foreach (Transform child in healthBarTransform)
            {
                if (child.CompareTag("UnitHealthBar"))
                {
                    healthBarObj = child.gameObject;
                    break;
                }
            }
            if (healthBarObj != null)
            {
                healthBarTransform.gameObject.SetActive(true);
                SetTarget(unit);
            }
        }
        else
        {
            Debug.LogWarning("HealthBarVisual object not found as a child of the unit.");
        }
    }

    public void AddBuff(int spellId, Sprite icon, float duration)
    {
        // Clone the BuffTemplate and add it to the buffCanvas
        GameObject newBuff = Instantiate(buffTemplate, buffCanvas);

        // Get the BuffDebuff component and initialize it
        BuffDebuff buffDebuff = newBuff.GetComponent<BuffDebuff>();
        buffDebuff.InitializeBuff(spellId, icon, duration);
        buffPos.AddBuff(newBuff);
        buffDebuff.gameObject.SetActive(true);
        // Add to the list of active buffs
        activeBuffs.Add(newBuff);
    }

    public void AddTargetDebuff(int spellId, Sprite icon, float duration)
    {
        // Clone the BuffTemplate and add it to the buffCanvas
        GameObject newBuff = Instantiate(targetDebuffTemplate, targetDebuffCanvas);

        // Get the BuffDebuff component and initialize it
        BuffDebuff buffDebuff = newBuff.GetComponent<BuffDebuff>();
        buffDebuff.InitializeBuff(spellId, icon, duration);
        debuffPos.AddBuff(newBuff);
        buffDebuff.gameObject.SetActive(true);
        // Add to the list of active buffs
        activeTargetDebuffs.Add(newBuff);
    }

    public void UpdateAura(int spellId, float duration, int stacks)
    {
        GameObject buffToRemove = null;
        foreach (var buff in activeBuffs)
        {
            if (buff != null)
            {
                BuffDebuff buffdebuff = buff.GetComponent<BuffDebuff>();
                if (buffdebuff != null)
                {
                    if (buffdebuff.spellId == spellId)
                    {
                        if (duration == 0) // UI needs to delete the buff from the list if we're updating it's duration to 0 (meaning we remove it)
                            buffToRemove = buff;
                        else
                        {
                            buffdebuff.UpdateData(duration, stacks);
                            return;
                        }
                    }
                }
            }
            else
                buffToRemove = buff; break;
        }
        if (buffToRemove != null)
        {
            Destroy(buffToRemove);
            activeBuffs.Remove(buffToRemove);
        }
    }
}
