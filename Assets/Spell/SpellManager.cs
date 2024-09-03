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

    public void CastSpell(int spellId, float speed, Transform caster) // No target
    {
        SpellScriptableObject spell = spells.Find(s => s.spellId == spellId);

        if (spell != null)
        {
            GameObject selectedPrefab = spell.spellPrefabs[Random.Range(0, spell.spellPrefabs.Count)];

            GameObject spellInstance = Instantiate(selectedPrefab, caster.position, Quaternion.identity);
            SpellBase spellScript = spellInstance.GetComponent<SpellBase>();

            if (spellScript != null)
            {
                spellScript.Initialize(caster, null, speed);
                spellScript.enabled = true; // Enable the script to start the logic
            }
        }
        else
        {
            Debug.LogError($"Spell with ID {spellId} not found.");
        }
    }
}
