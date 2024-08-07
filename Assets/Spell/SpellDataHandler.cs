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

            SpellInfo spell = new SpellInfo(spellData.id, mask, type, spellData.manaCost, spellData.castTime);
            Spells.Add(spell);
        }

        Debug.Log("Successfully loaded and parsed spells.");
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
