using System.Collections.Generic;
using UnityEngine;

public class LocationHandler : MonoBehaviour
{
    public Unit unit {  get; private set; }
    public static LocationHandler Instance;
    private MapHandler mapHandler;

    private void Awake()
    {
        

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        mapHandler = MapHandler.Instance;
    }

    // Method to get the current position of the unit
    public Vector3 GetPosition(Unit unit)
    {
        return unit.transform.position;
    }

    // Method to set the position of the unit
    public void SetPosition(Unit unit, Vector3 newPosition)
    {
        unit.transform.position = newPosition;
    }

    // Method to get the distance from another unit
    public float GetDistanceFrom(Unit unit, Unit otherUnit)
    {
        return Vector3.Distance(unit.transform.position, otherUnit.transform.position);
    }

    // Method to move the unit towards a target position
    public void MoveTowards(Unit unit, Vector3 targetPosition, float speed)
    {
        unit.transform.position = Vector3.MoveTowards(unit.transform.position, targetPosition, speed * Time.deltaTime);
    }

    public Unit GetNearestEnemy(Unit unit)
    {
        List<Unit> unitList = new List<Unit>(FindObjectsOfType<Unit>());
        float closestDistance = 100f;
        Unit nearestTarget = null;
        foreach (Unit target in unitList)
        {
            if (target != null && target.IsAlive() && target.IsHostileTo(unit) && target != unit)
            {
                float distance = GetDistanceFrom(unit, target);
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


    public List<Unit> GetNearestEnemiesFromPosition(Unit unit, Vector3 position, int range)
    {
        List<Unit> list = new List<Unit>();
        List<Unit> mapUnits = mapHandler.GetAllUnitsInMap(unit.GetMap());
        foreach (Unit v in mapUnits)
        {
            if (IsWithinRange(position, range, v) && unit.IsHostileTo(v) && v.IsAlive() && v != unit)
            {
                list.Add(v);
            }
        }

        return list;
    }

    public List<Unit> GetEnemiesInCone(Unit unit, float coneAngle, int coneRange)
    {
        List<Unit> list = new List<Unit>();
        Vector3 unitForward = unit.transform.forward;
        Vector3 unitPosition = unit.transform.position;

        foreach (Unit v in GetNearestEnemyUnitList(unit, coneRange))
        {
            Vector3 targetPosition = v.transform.position;
            Vector3 targetDirection = (targetPosition - unitPosition).normalized;

            float dotProduct = Vector3.Dot(unitForward, targetDirection);
            float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;

            if (angle <= (coneAngle / 2) && v != unit)
            {
                list.Add(v);
            }
        }

        return list;
    }

    public bool IsWithinDist(Unit unit, int dist, Unit target)
    {
        float distance = Vector3.Distance(unit.transform.position, target.transform.position);
        return distance < dist;
    }

    public bool IsWithinRange(Vector3 position, int range, Unit target)
    {
        float distance = Vector3.Distance(position, target.transform.position);
        return distance < range;
    }

    public bool IsWithinInnerRange(Vector3 position, int innerRange, Unit target)
    {
        float distance = Vector3.Distance(position, target.transform.position);
        return distance < innerRange;
    }

    public List<Unit> GetNearestEnemyUnitsFromUnit(Unit unit, Unit targetUnit, int dist)
    {
        List<Unit> list = new List<Unit>();
        List<Unit> mapUnits = mapHandler.GetAllUnitsInMap(unit.GetMap());
        foreach (var v in mapUnits)
        {
            if (IsWithinDist(unit, dist, v) && unit.IsHostileTo(v) && v.IsAlive() && v != unit)
            {
                list.Add(v);
            }
        }

        return list;
    }

    public List<Unit> GetNearestEnemyUnitList(Unit unit, int dist)
    {
        List<Unit> list = new List<Unit>();

        foreach (var v in GetNearestUnitList(unit, dist))
        {
            if (v.IsHostileTo(unit))
            {
                list.Add(v);
            }
        }

        return list;
    }

    // Helper function to get the nearest units within a certain distance
    public List<Unit> GetNearestUnitList(Unit unit, int dist)
    {
        List<Unit> list = new List<Unit>();
        List<Unit> mapUnits = mapHandler.GetAllUnitsInMap(unit.GetMap());
        foreach (var v in mapUnits)
        {
            if (IsWithinDist(unit, dist, v))
            {
                list.Add(v);
            }
        }

        return list;
    }

}
