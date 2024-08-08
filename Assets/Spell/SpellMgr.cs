using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public const int SPELL_STATE_PREPARING = 1, SPELL_STATE_DELAYED = 2, SPELL_STATE_FINISHED = 3, SPELL_STATE_CASTING = 4, SPELL_STATE_NULL = 5;

    public static SpellManager Instance { get; private set; }

    private List<Spell> activeSpells = new List<Spell>();
    private List<Spell> inactiveSpells = new List<Spell>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Another instance of SpellManager exists, destroying this one.");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Update all active spells
        for (int i = activeSpells.Count - 1; i >= 0; i--)
        {
            Spell spell = activeSpells[i];
            print($"{spell} is what it is, it really is, and I hate that shit because I would love if it it worked!!!!!!!");
            // Just immediately stop if it's nulled for whatever reason
            if (activeSpells[i] == null)
            {
                activeSpells.RemoveAt(i);
                continue;
            }

           // activeSpells[i].UpdateSpell();
            Debug.Log($"{activeSpells[i]} is updating!");

            // Move the spell to the inactive list if it's no longer active
            if (activeSpells[i].GetSpellState() == SPELL_STATE_NULL)
            {
                inactiveSpells.Add(activeSpells[i]);
                activeSpells.RemoveAt(i);
            }
        }
    }

    public void AddSpell(Spell spell)
    {
        activeSpells.Add(spell);
        Debug.Log($"{spell} has been added.");
    }

    public void RemoveSpell(Spell spell)
    {
        if (activeSpells.Contains(spell))
        {
            activeSpells.Remove(spell);
        }
        else if (inactiveSpells.Contains(spell))
        {
            inactiveSpells.Remove(spell);
        }
    }
}

