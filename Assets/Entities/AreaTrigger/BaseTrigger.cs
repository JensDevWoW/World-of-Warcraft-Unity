using System.Collections.Generic;
using UnityEngine;

public abstract class AreaTriggerScript : MonoBehaviour
{
    // Initialize the script with necessary components
    public void Initialize(Unit caster, AreaTrigger trigger)
    {
        this.caster = caster;
        this.trigger = trigger;
    }

    public abstract void OnUnitEnter(Unit unit);
    public abstract void OnUnitLeave(Unit unit);
    public abstract void IsRemoved(List<Unit> currentUnits);

    protected Unit caster;
    protected AreaTrigger trigger;
}
