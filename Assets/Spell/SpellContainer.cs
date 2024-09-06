using UnityEngine;
public class SpellContainer : MonoBehaviour
{
    public static SpellContainer Instance { get; private set; }

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

    public SpellInfo GetSpellById(int spellId)
    {
        return SpellDataHandler.Instance.Spells.Find(spell => spell.Id == spellId);
    }

    public SpellInfo GetSpellByName(string spellName)
    {
        return SpellDataHandler.Instance.Spells.Find(spell => spell.Name.Equals(spellName, System.StringComparison.OrdinalIgnoreCase));
    }

    public SpellAttributes GetSpellAttributes(int spellId)
    {
        SpellInfo spell = GetSpellById(spellId);
        return spell != null ? spell.Attributes : SpellAttributes.SPELL_ATTR_NONE;
    }

    public float GetSpellCastTime(int spellId)
    {
        SpellInfo spell = GetSpellById(spellId);
        return spell != null ? spell.CastTime : 0f;
    }

    public UnitSpell Convert(SpellInfo spellInfo)
    {
        if (spellInfo == null) return null;

        return new UnitSpell(spellInfo);
    }

}
