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

    public void BlinkEffect(Unit unit, Transform casterTransform)
    {
        Vector3 startPosition = casterTransform.position;
        Vector3 forwardDirection = casterTransform.forward;

        int maxBlinkDistance = 20;
        float maxSlopeAngle = unit.charController.slopeLimit;

        // Raycast to check for obstacles and slopes
        if (Physics.Raycast(startPosition, forwardDirection, out RaycastHit hit, maxBlinkDistance))
        {
            // Check if the hit object is within slope limits
            if (Vector3.Angle(Vector3.up, hit.normal) <= unit.charController.slopeLimit)
            {
                // Move the caster to the hit point
                Vector3 targetPosition = hit.point;

                // Adjust slightly up to prevent sinking into the ground
                targetPosition.y += 0.5f;
                casterTransform.position = targetPosition;
                Debug.Log("Blink successful to: " + targetPosition);
            }
            else
            {
                // If the slope is too steep, stop at the current position
                Vector3 stopPosition = hit.point - forwardDirection * 0.5f; // Slight offset back from hit point
                casterTransform.position = stopPosition;
                Debug.Log("Blink stopped due to steep slope at: " + stopPosition);
            }
        }
        else
        {
            // No obstacles, blink to the max distance
            Vector3 blinkPosition = startPosition + forwardDirection * maxBlinkDistance;

            // Check if the final position is above a slope that’s too steep
            if (Physics.Raycast(blinkPosition + Vector3.up, Vector3.down, out RaycastHit groundHit, Mathf.Infinity))
            {
                if (Vector3.Angle(Vector3.up, groundHit.normal) <= maxSlopeAngle)
                {
                    // Valid position on the slope
                    blinkPosition = groundHit.point + Vector3.up * 0.5f;
                    casterTransform.position = blinkPosition;
                    Debug.Log("Blink successful to max distance: " + blinkPosition);
                }
                else
                {
                    // Too steep; adjust position
                    Vector3 adjustedPosition = blinkPosition - forwardDirection * 0.5f;
                    casterTransform.position = adjustedPosition;
                    Debug.Log("Blink stopped due to steep slope at: " + adjustedPosition);
                }
            }
            else
            {
                // Flat ground at max blink distance
                blinkPosition.y = startPosition.y;
                casterTransform.position = blinkPosition;
                Debug.Log("Blink successful to flat ground: " + blinkPosition);
            }
        }
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
