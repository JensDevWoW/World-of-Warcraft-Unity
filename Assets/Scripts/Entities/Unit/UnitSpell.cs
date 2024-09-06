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

public class UnitSpell
{
    // Reference to the core spell info, containing immutable data
    public SpellInfo SpellInfo { get; private set; }
    public Unit Unit { get; private set; }
    // Local dynamic fields that can be modified
    public float currentCooldownTime; // Tracks the current cooldown time
    public int Stacks { get; private set; } // Tracks the current number of stacks
    public UnitSpell(SpellInfo spellInfo)
    {
        SpellInfo = spellInfo;
        Stacks = spellInfo.Stacks; // Initialize stacks based on the core spell info
        currentCooldownTime = 0; // Initialize with no cooldown
    }

    public void Init(Unit unit)
    {
        Unit = unit;
    }

    // Method to start the cooldown
    public void StartCooldown()
    {
        currentCooldownTime = SpellInfo.Cooldown; // Use cooldown from core spell info
    }

    // Check if the cooldown is active
    public bool IsCooldownActive()
    {
        return currentCooldownTime > 0;
    }

    public bool HasStacks()
    {
        return (SpellInfo.Stacks > 1);
    }

    // Method to get the remaining cooldown time
    public float GetRemainingCooldown()
    {
        return Mathf.Max(0, currentCooldownTime);
    }

    public void ResetCooldown()
    {
        currentCooldownTime = 0;
        if (HasStacks() && (Stacks < SpellInfo.Stacks))
        {
            UpdateStacks(Stacks + 1);
        }

    }

    // Update stacks method
    public void UpdateStacks(int newStacks)
    {
        Stacks = newStacks;
        Debug.Log($"Updated stacks for spell {SpellInfo.Name} to {Stacks}");
    }
}
