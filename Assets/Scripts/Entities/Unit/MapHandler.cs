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

public class MapHandler : MonoBehaviour
{
    private List<Unit> unitsInMap = new List<Unit>();

    // Singleton pattern for easy access
    public static MapHandler Instance { get; private set; }

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

    // Function to add a unit to the map
    public void AddUnit(Unit unit)
    {
        if (!unitsInMap.Contains(unit))
        {
            unitsInMap.Add(unit);
        }
    }

    // Function to remove a unit from the map
    public void RemoveUnit(Unit unit)
    {
        if (unitsInMap.Contains(unit))
        {
            unitsInMap.Remove(unit);
        }
    }

    // Function to get all units in the map
    public List<Unit> GetAllUnitsInMap(int mapId)
    {
        //TODO: Have different maps, right now every unit has the same mapId
        List<Unit> newList = new List<Unit>();
        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units)
        {
            newList.Add(unit);
        }
        return newList;
    }
}
