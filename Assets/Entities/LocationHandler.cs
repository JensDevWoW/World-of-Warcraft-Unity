using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationHandler : Unit
{

    public float GetDistanceFrom(Unit unit)
    {
        return 3; // Placeholder
    }
    public override Unit ToUnit()
    {
        return this as Unit;
    }
}
