using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;
    public List<SpellScriptableObject> spells;

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

    public void LoadSpells(List<SpellScriptableObject> spellObjects)
    {
        spells = spellObjects;
    }

    public void CastSpell(int spellId, float speed, Transform caster, Transform target)
    {
        SpellScriptableObject spell = spells.Find(s => s.spellId == spellId);

        if (spell != null)
        {
            GameObject selectedPrefab = spell.spellPrefabs[Random.Range(0, spell.spellPrefabs.Count)];

            GameObject spellInstance = Instantiate(selectedPrefab, caster.position, Quaternion.identity);
            SpellBase spellScript = spellInstance.GetComponent<SpellBase>();

            if (spellScript != null)
            {
                spellScript.Initialize(caster, target, speed);
                spellScript.enabled = true; // Enable the script to start the logic
            }
        }
        else
        {
            Debug.LogError($"Spell with ID {spellId} not found.");
        }
    }
}
