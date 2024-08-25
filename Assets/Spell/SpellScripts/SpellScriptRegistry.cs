using System;
using System.Collections.Generic;
using UnityEngine;

public class SpellScriptRegistry
{
    // A dictionary to map spell IDs to SpellScript types
    private static readonly Dictionary<int, Type> spellScriptMap = new Dictionary<int, Type>
    {
        { 13, typeof(PyroblastScript) },
        { 15, typeof(IceLance) },
        { 2, typeof(Frostbolt) },
        // Add more mappings as needed
    };

    public static Type GetSpellScriptType(int spellId)
    {
        if (spellScriptMap.TryGetValue(spellId, out Type scriptType))
        {
            return scriptType;
        }
        return null;
    }
}
