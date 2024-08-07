using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellList
{
    public string Name { get; private set; }
    public int SpellId { get; private set; }
    public KeyCode KeyCode { get; private set; }  // Add KeyCode field

    public SpellList(string name, int spellId, KeyCode keyCode)
    {
        Name = name;
        SpellId = spellId;
        KeyCode = keyCode;
    }
}


