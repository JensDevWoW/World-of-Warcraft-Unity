using UnityEngine;

public class SpellList
{
    public string Name { get; }
    public int SpellId { get; }
    public KeyCode KeyCode { get; }
    public bool IsAoE { get; } // Add this property to identify AoE spells

    public SpellList(string name, int spellId, KeyCode keyCode, bool isAoE = false)
    {
        Name = name;
        SpellId = spellId;
        KeyCode = keyCode;
        IsAoE = isAoE;
    }
}
