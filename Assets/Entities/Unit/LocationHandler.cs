using System.Collections.Generic;
using UnityEngine;

public class LocationHandler : MonoBehaviour
{
    private Transform unitTransform;
    public Unit unit {  get; private set; }
    void Awake()
    {
        unitTransform = GetComponent<Transform>();
        unit = GetComponent<Unit>();
    }

    // Method to get the current position of the unit
    public Vector3 GetPosition()
    {
        return unitTransform.position;
    }

    // Method to set the position of the unit
    public void SetPosition(Vector3 newPosition)
    {
        unitTransform.position = newPosition;
    }

    // Method to get the distance from another unit
    public float GetDistanceFrom(Unit otherUnit)
    {
        return Vector3.Distance(unitTransform.position, otherUnit.transform.position);
    }

    // Method to move the unit towards a target position
    public void MoveTowards(Vector3 targetPosition, float speed)
    {
        unitTransform.position = Vector3.MoveTowards(unitTransform.position, targetPosition, speed * Time.deltaTime);
    }

    public Unit GetNearestEnemy()
    {
        List<Unit> unitList = new List<Unit>(FindObjectsOfType<Unit>());
        float closestDistance = 100f;
        Unit nearestTarget = null;
        foreach (Unit target in unitList)
        {
            if (target != null && target.IsAlive() && target.IsHostileTo(unit) && target != unit)
            {
                float distance = GetDistanceFrom(target);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestTarget = target;
                }
            }
        }

        if (nearestTarget != null)
        {
            return nearestTarget;
        }
        else
            return null;
    }

    
}
