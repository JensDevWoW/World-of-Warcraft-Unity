/*
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation; either version 2 of the License, or (at your
 * option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable] // Allows this class to be serialized in JSON

[System.Flags] // Add the Flags attribute to allow bitwise operations
public enum SpellAttributes
{
    SPELL_ATTR_NONE = 0, // No attributes, default value
    SPELL_ATTR_PASSIVE = 1 << 0, // 1 << 0 is 0000 0001
    SPELL_ATTR_IS_CHANNELED = 1 << 1, // 1 << 1 is 0000 0010
    SPELL_ATTR_ALLOW_WHILE_STEALTHED = 1 << 2, // 1 << 2 is 0000 0100
    SPELL_ATTR_PREVENTS_ANIM = 1 << 3, // 1 << 3 is 0000 1000
    SPELL_ATTR_IGNORE_LINE_OF_SIGHT = 1 << 4, // 1 << 4 is 0001 0000
    SPELL_ATTR_NOT_AN_ACTION = 1 << 5, // 1 << 5 is 0010 0000
    SPELL_ATTR_CANT_CRIT = 1 << 6, // 1 << 6 is 0100 0000
    SPELL_ATTR_ALLOW_AURA_WHILE_DEAD = 1 << 7, // 1 << 7 is 1000 0000
    SPELL_ATTR_ALLOW_CAST_WHILE_CASTING = 1 << 8,
    SPELL_ATTR_AURA_IS_BUFF = 1 << 9,
    SPELL_ATTR_NOT_IN_SPELLBOOK = 1 << 10,
    SPELL_ATTR_REMOVE_ENTERING_ARENA = 1 << 11,
    SPELL_ATTR_ALLOW_WHILE_FLEEING = 1 << 12,
    SPELL_ATTR_ALLOW_WHILE_CONFUSED = 1 << 13,
    SPELL_ATTR_HASTE_AFFECTS_DURATION = 1 << 14,
    SPELL_ATTR_NOT_IN_BG_OR_ARENA = 1 << 15,
    SPELL_ATTR_NOT_USABLE_IN_ARENA = 1 << 16,
    SPELL_ATTR_REACTIVE_DAMAGE_PROC = 1 << 17,
    SPELL_ATTR_IGNORE_GCD = 1 << 18,
    SPELL_ATTR_BREAKABLE_BY_DAMAGE = 1 << 19
    // Add more attributes as needed
}

[System.Flags]
public enum SpellFlags
{
    SPELL_FLAG_NONE = 0, // No attributes, default value
    SPELL_FLAG_CAST_WHILE_MOVING = 1 << 0,
    SPELL_FLAG_AOE = 1 << 1,
    SPELL_FLAG_NEEDS_TARGET = 1 << 2,
    SPELL_FLAG_MOUSE = 1 << 3,
    SPELL_FLAG_CONE = 1 << 4,
    SPELL_FLAG_MOUSE_TARGET = 1 << 5,
    SPELL_FLAG_ALWAYS_CRIT = 1 << 6,
    SPELL_FLAG_IGNORES_GCD = 1 << 7,
    SPELL_FLAG_DISABLE_ANIM = 1 << 8,
    SPELL_FLAG_SELF_TARGET = 1 << 9,
    SPELL_FLAG_CAST_WHILE_CASTING = 1 << 10,

}

[System.Serializable]
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
    public int cooldown;
    public string attributes;
    public string flags;
    public int range;
    public int stacks;
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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadSpellData();
        LoadVFXSpells();
    }

    void LoadSpellData()
    {
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

            SpellAttributes attributes = ParseSpellAttributes(spellData.attributes);
            SpellFlags flags = ParseSpellFlags(spellData.flags);

            SpellInfo spell = new SpellInfo(
                spellData.id,
                spellData.name,
                mask,
                type,
                spellData.manaCost,
                spellData.castTime,
                spellData.spellTime,
                spellData.speed,
                spellData.positive,
                spellData.basepoints,
                spellData.damageclass,
                effects,
                spellData.spellscript,
                spellData.cooldown,
                attributes,
                flags,
                spellData.range,
                spellData.stacks
            );


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


    SpellAttributes ParseSpellAttributes(string attributesString)
    {
        SpellAttributes attributes = SpellAttributes.SPELL_ATTR_NONE;

        if (!string.IsNullOrEmpty(attributesString))
        {
            string[] attributeNames = attributesString.Split(',');

            foreach (var attribute in attributeNames)
            {
                string trimmedAttributeName = attribute.Trim();  // Use a new variable for the trimmed value

                if (Enum.TryParse(trimmedAttributeName, out SpellAttributes parsedAttribute))
                {
                    attributes |= parsedAttribute;
                }
                else
                {
                    Debug.LogWarning($"Unknown SpellAttribute: {trimmedAttributeName}");
                }
            }
        }

        return attributes;
    }

    SpellFlags ParseSpellFlags(string flagsString)
    {
        SpellFlags flags = SpellFlags.SPELL_FLAG_NONE;

        if (!string.IsNullOrEmpty(flagsString))
        {
            string[] flagNames = flagsString.Split(',');

            foreach (var flag in flagNames)
            {
                string trimmedFlagName = flag.Trim();  // Use a new variable for the trimmed value

                if (Enum.TryParse(trimmedFlagName, out SpellFlags parsedFlag))
                {
                    flags |= parsedFlag;
                }
                else
                {
                    Debug.LogWarning($"Unknown SpellAttribute: {trimmedFlagName}");
                }
            }
        }

        return flags;
    }

    public AuraInfo GetAuraInfo(int auraId)
    {
        return Auras.FirstOrDefault(aura => aura.Id == auraId);
    }

    AuraType GetAuraTypeFromString(string typeName)
    {
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
                case "AURA_EFFECT_APPLY_ABSORB":
                    effects.Add(AuraEffect.AURA_EFFECT_APPLY_ABSORB);
                    break;
                case "AURA_EFFECT_ROOT":
                    effects.Add(AuraEffect.AURA_EFFECT_ROOT);
                    break;
                case "AURA_EFFECT_INCREASE_STAT":
                    effects.Add(AuraEffect.AURA_EFFECT_INCREASE_STAT);
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

    void LoadVFXSpells()
    {
        // Load all SpellScriptableObjects from Resources or any specific folder
        SpellScriptableObject[] loadedSpells = Resources.LoadAll<SpellScriptableObject>("VFXObjects");

        if (loadedSpells.Length > 0)
        {
            List<SpellScriptableObject> spellList = new List<SpellScriptableObject>(loadedSpells);
            VFXManager.Instance.LoadSpells(spellList);
            Debug.Log("Successfully loaded SpellObjects!");
        }
        else
        {
            Debug.LogError("No SpellScriptableObjects found.");
        }
    }
}
