using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public const int SPELL_STATE_PREPARING = 1, SPELL_STATE_DELAYED = 2, SPELL_STATE_FINISHED = 3, SPELL_STATE_CASTING = 4, SPELL_STATE_NULL = 5;

    public static SpellManager Instance { get; private set; }

    private List<Spell> activeSpells = new List<Spell>();

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

    void Update()
    {
        // Update all active spells
        for (int i = activeSpells.Count - 1; i >= 0; i--)
        {
            activeSpells[i].Update();

            // Remove the spell if it's no longer active
            if (activeSpells[i].GetSpellState() == SPELL_STATE_FINISHED)
            {
                activeSpells.RemoveAt(i);
            }
        }
    }

    public void AddSpell(Spell spell)
    {
        activeSpells.Add(spell);
    }
}
