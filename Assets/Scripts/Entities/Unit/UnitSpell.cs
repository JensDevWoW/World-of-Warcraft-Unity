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
