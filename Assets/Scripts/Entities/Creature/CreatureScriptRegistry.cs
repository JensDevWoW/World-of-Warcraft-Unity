using System;
using System.Collections.Generic;
using UnityEngine;

public class CreatureScriptRegistry
{
    // A dictionary to map spell IDs to SpellScript types
    private static readonly Dictionary<int, Type> creatureScriptMap = new Dictionary<int, Type>
    {
        { 1, typeof(TrainingDummy) }, // Here is where all creaturescripts are linked to their respective creaturescripts
        // Add more mappings as needed
    };

    public static Type GetCreatureScriptType(int creatureId)
    {
        if (creatureScriptMap.TryGetValue(creatureId, out Type scriptType))
        {
            return scriptType;
        }
        return null;
    }
}
