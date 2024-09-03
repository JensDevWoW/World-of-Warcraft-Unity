/*
 * This program is free software; you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation; either version 2 of the License, or (at your
 * option) any later version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using UnityEngine;
using System.Collections.Generic;
using System;

public class AreaTrigger : MonoBehaviour
{
    public Unit caster;
    public Vector3 position;
    public float dist;
    public Spell spell;
    public float maxDuration;
    public bool active;
    public GameObject gobj;
    public List<Unit> unitsInside;
    public float timeLeft;
    public AreaTriggerScript script;

    public void Initialize(Unit caster, Spell spell, AreaTriggerInfo info)
    {
        this.caster = caster;
        this.position = spell.position;
        this.dist = info.Distance;
        this.spell = spell;
        this.maxDuration = info.Duration;
        this.active = false;
        this.unitsInside = new List<Unit>();
        this.timeLeft = info.Duration;

        // Load and initialize the appropriate AreaTriggerScript
        Type scriptType = AreaTriggerScriptRegistry.GetAreaTriggerScriptType(spell.spellId);
        if (scriptType != null)
        {
            AreaTriggerScript scriptInstance = gameObject.AddComponent(scriptType) as AreaTriggerScript;
            scriptInstance.Initialize(caster, this);
            this.script = scriptInstance;
        }
    }

    public void Activate()
    {
        active = true;
        
        // Additional logic to handle when the AreaTrigger is added to the world, if necessary
    }

    public void UpdateTrigger(float deltaTime)
    {
        if (active)
        {
            timeLeft -= deltaTime;
            if (timeLeft <= 0)
            {
                Deactivate();
            }
            else
            {
                // Logic to check for units within the trigger's area and handle their interactions
                UpdateUnitsInside();
            }
        }
    }

    private void UpdateUnitsInside()
    {
        List<Unit> currentUnits = new List<Unit>(); // Replace with actual logic to get current units inside range

        foreach (var unit in currentUnits)
        {
            if (!unitsInside.Contains(unit))
            {
                unitsInside.Add(unit);
                script.OnUnitEnter(unit);
            }
        }

        for (int i = unitsInside.Count - 1; i >= 0; i--)
        {
            if (!currentUnits.Contains(unitsInside[i]))
            {
                script.OnUnitLeave(unitsInside[i]);
                unitsInside.RemoveAt(i);
            }
        }
    }

    public void Deactivate()
    {
        active = false;
        // Handle logic for removing the AreaTrigger from the world, cleanup, etc.
        Destroy(gameObject);
    }
}
