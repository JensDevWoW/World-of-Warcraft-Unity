using UnityEngine;

public class SpellContainer : MonoBehaviour
{
    public static SpellContainer Instance { get; private set; }

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Retrieve a spell by ID using the existing SpellDataHandler instance
    public SpellInfo GetSpellById(int spellId)
    {
        return SpellDataHandler.Instance.Spells.Find(spell => spell.Id == spellId);
    }

    // Retrieve a spell by name using the existing SpellDataHandler instance
    public SpellInfo GetSpellByName(string spellName)
    {
        return SpellDataHandler.Instance.Spells.Find(spell => spell.Name.Equals(spellName, System.StringComparison.OrdinalIgnoreCase));
    }

    // Example method to get spell attributes
    public SpellAttributes GetSpellAttributes(int spellId)
    {
        SpellInfo spell = GetSpellById(spellId);
        return spell != null ? spell.Attributes : SpellAttributes.SPELL_ATTR_NONE;
    }

    // Example method to get spell cast time
    public float GetSpellCastTime(int spellId)
    {
        SpellInfo spell = GetSpellById(spellId);
        return spell != null ? spell.CastTime : 0f;
    }

    // Add additional methods to retrieve specific spell data as needed
}
