using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // Allows this class to be serialized in JSON
public class SpellData
{
    public int id;
    public string name;
    public string schoolMask;
    public string type;
    public int manaCost;
    public float castTime;
    public bool spellTime;
    public float speed;
    public bool positive;
    public int basepoints;
    public string damageclass;
    public string effects;
}

[System.Serializable]
public class SpellDataWrapper
{
    public SpellData[] spells;
}


// SpellDatabase inherits from MonoBehaviour so it can be attached to a GameObject
public class SpellDataHandler : MonoBehaviour
{
    public static SpellDataHandler Instance { get; private set; }
    public List<SpellInfo> Spells { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSpellData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadSpellData()
    {
        // Load the JSON file from the Resources folder
        TextAsset jsonData = Resources.Load<TextAsset>("SpellData");

        if (jsonData == null)
        {
            Debug.LogError("Failed to load SpellData.json from Resources.");
            return;
        }

        // Deserialize the JSON data into a SpellDataWrapper object
        SpellDataWrapper spellDataWrapper = JsonUtility.FromJson<SpellDataWrapper>(jsonData.text);

        if (spellDataWrapper == null || spellDataWrapper.spells == null || spellDataWrapper.spells.Length == 0)
        {
            Debug.LogError("Failed to parse SpellData or SpellData array is empty.");
            return;
        }

        Spells = new List<SpellInfo>();

        foreach (var spellData in spellDataWrapper.spells)
        {
            SpellSchoolMask mask = GetSchoolMaskFromString(spellData.schoolMask);
            SpellType type = GetSpellTypeFromString(spellData.type);

            List<SpellEffect> effects = ParseEffects(spellData.effects);

            SpellInfo spell = new SpellInfo(spellData.id, mask, type, spellData.manaCost, spellData.castTime, 
                spellData.spellTime, spellData.speed, spellData.positive, spellData.basepoints, spellData.damageclass, effects);

            Spells.Add(spell);
        }

        Debug.Log("Successfully loaded and parsed spells.");
    }

    List<SpellEffect> ParseEffects(string effectsString)
    {
        List<SpellEffect> effects = new List<SpellEffect>();

        // Split the string into individual effect names
        string[] effectNames = effectsString.Split(',');

        // Map the effect names to actual SpellEffect objects
        foreach (string effectName in effectNames)
        {
            switch (effectName.Trim())
            {
                case "SPELL_EFFECT_SCHOOL_DAMAGE":
                    effects.Add(SpellEffect.SPELL_EFFECT_SCHOOL_DAMAGE);
                    break;
                case "SPELL_EFFECT_CREATE_AREATRIGGER":
                    effects.Add(SpellEffect.SPELL_EFFECT_CREATE_AREATRIGGER);
                    break;
                case "SPELL_EFFECT_APPLY_AURA":
                    effects.Add(SpellEffect.SPELL_EFFECT_APPLY_AURA);
                    break;
                case "SPELL_EFFECT_DISPEL":
                    effects.Add(SpellEffect.SPELL_EFFECT_DISPEL);
                    break;
                case "SPELL_EFFECT_TELEPORT":
                    effects.Add(SpellEffect.SPELL_EFFECT_TELEPORT);
                    break;
                case "SPELL_EFFECT_INTERRUPT_CAST":
                    effects.Add(SpellEffect.SPELL_EFFECT_INTERRUPT_CAST);
                    break;
                case "SPELL_EFFECT_REMOVE_AURA":
                    effects.Add(SpellEffect.SPELL_EFFECT_REMOVE_AURA);
                    break;
                case "SPELL_EFFECT_DUMMY":
                    effects.Add(SpellEffect.SPELL_EFFECT_DUMMY);
                    break;
                    // Add other cases as necessary
            }
        }

        return effects;
    }

    SpellSchoolMask GetSchoolMaskFromString(string maskName)
    {
        switch (maskName)
        {
            case "Fire": return SpellSchoolMask.Fire;
            case "Frost": return SpellSchoolMask.Frost;
            // Add other cases as necessary
            default: return SpellSchoolMask.Physical;
        }
    }

    SpellType GetSpellTypeFromString(string typeName)
    {
        switch (typeName)
        {
            case "Damage": return SpellType.Damage;
            case "Heal": return SpellType.Heal;
            // Add other cases as necessary
            default: return SpellType.Utility;
        }
    }
}
