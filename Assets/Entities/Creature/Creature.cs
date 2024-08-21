using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using JetBrains.Annotations;

public class Creature : MonoBehaviour
{
    private Unit unit;
    private Unit owner;
    private float m_duration;
    private CreatureScript c_script;
    public int creatureId;

    public void Init()
    {
        unit = GetComponent<Unit>();
        Unit target = LocationHandler.Instance.GetNearestEnemy(unit);
        unit.SetTarget(target);
        AttachCreatureScript(creatureId);
    }

    public bool HasOwner()
    {
        return owner != null;
    }

    public Unit GetOwner()
    {
        return owner;
    }

    public void SetOwner(Unit unit)
    {
        owner = unit; 
    }

    private void AttachCreatureScript(int creatureId)
    {
        // Get the script type from the registry using the spellId
        Type scriptType = CreatureScriptRegistry.GetCreatureScriptType(creatureId);

        if (scriptType != null)
        {
            // Attach the script as a component
            c_script = gameObject.AddComponent(scriptType) as CreatureScript;
            c_script.unit = unit;
            if (c_script != null)
            {
                Debug.Log($"Attached script {scriptType.Name} to {gameObject.name}");
            }
            else
            {
                Debug.LogError($"Failed to attach script for spell ID {creatureId}.");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
