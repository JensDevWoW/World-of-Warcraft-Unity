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
