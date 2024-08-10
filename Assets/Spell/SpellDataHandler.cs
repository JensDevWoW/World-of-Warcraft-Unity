using System.Collections.Generic;
using System.Linq;
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
    public string spellscript;
    public AuraData aura;
}

[System.Serializable]
public class AuraData
{
    public int auraId;
    public string name;
    public float duration;
    public bool periodic;
    public float ticktime;
    public int stacks;
    public string effects;
    public bool isPositive;
    public string type;
    public string aurascript;
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

    public List<AuraInfo> Auras { get; private set; }

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
        Auras = new List<AuraInfo>();

        foreach (var spellData in spellDataWrapper.spells)
        {
            SpellSchoolMask mask = GetSchoolMaskFromString(spellData.schoolMask);
            SpellType type = GetSpellTypeFromString(spellData.type);

            List<SpellEffect> effects = ParseSpellEffects(spellData.effects);

            SpellInfo spell = new SpellInfo(spellData.id, spellData.name, mask, type, spellData.manaCost, spellData.castTime, 
                spellData.spellTime, spellData.speed, spellData.positive, spellData.basepoints, spellData.damageclass, effects, spellData.spellscript);

            // If the spell has an aura, create and add it
            if (spellData.aura != null && !string.IsNullOrEmpty(spellData.aura.name))
            {
                AuraType auratype = GetAuraTypeFromString(spellData.aura.type);

                List<AuraEffect> auraEffects = ParseAuraEffects(spellData.aura.effects);
                AuraInfo aura = new AuraInfo(spellData.aura.auraId, spellData.aura.name, auratype, spellData.positive, spellData.aura.duration, spellData.aura.periodic, spellData.aura.ticktime, spellData.aura.stacks, spellData.basepoints, spellData.damageclass, auraEffects, spellData.aura.aurascript, spell);
                Auras.Add(aura);
            }

            Spells.Add(spell);
        }

        Debug.Log("Successfully loaded and parsed spells.");
    }

    public AuraInfo GetAuraInfo(int auraId)
    {
        return Auras.FirstOrDefault(aura => aura.Id == auraId);
    }

    AuraType GetAuraTypeFromString(string typeName)
    {
        Debug.Log($"Parsing AuraType from string: '{typeName}'");
        typeName = typeName.Trim();  // Trims any extra whitespace
        switch (typeName)
        {
            case "Buff": return AuraType.Buff;
            case "Debuff": return AuraType.Debuff;
            // Add other cases as necessary
            default: throw new System.ArgumentException($"Unknown AuraType: {typeName}");
        }
    }



    List<SpellEffect> ParseSpellEffects(string effectsString)
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

    List<AuraEffect> ParseAuraEffects(string effectsString)
    {
        List<AuraEffect> effects = new List<AuraEffect>();

        // Split the string into individual effect names
        string[] effectNames = effectsString.Split(',');

        // Map the effect names to actual SpellEffect objects
        foreach (string effectName in effectNames)
        {
            switch (effectName.Trim())
            {
                case "AURA_EFFECT_DAMAGE":
                    effects.Add(AuraEffect.AURA_EFFECT_DAMAGE);
                    break;
                case "AURA_EFFECT_DUMMY":
                    effects.Add(AuraEffect.AURA_EFFECT_DUMMY);
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
