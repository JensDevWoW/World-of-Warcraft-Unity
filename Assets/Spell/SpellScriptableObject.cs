using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "Spells/Spell", order = 1)]
public class SpellScriptableObject : ScriptableObject
{
    public int spellId;
    public string spellName;
    public List<GameObject> spellPrefabs; // List of prefabs for this spell
}
