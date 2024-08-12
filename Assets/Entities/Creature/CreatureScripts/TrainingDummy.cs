using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : CreatureScript
{
    private bool hasCast = false;
    private float castTimer = 5;
    public override void Update()
    {
        if (castTimer <= 0)
        {
            //unit.SetTarget(unit.ToLocation().GetNearestEnemy());
            //unit.CastSpell(4, unit.GetTarget());
            castTimer = 10;
        }
        else
            castTimer -= Time.deltaTime;
    }
}
