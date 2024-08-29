using System.Collections.Generic;
using UnityEngine;

public class WorldServer : MonoBehaviour
{
    public static WorldServer Instance { get; private set; }

    private List<Unit> allUnits = new List<Unit>();

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

    private void Update()
    {
        // Continuously check for new units in the game
        Unit[] units = FindObjectsOfType<Unit>();
        foreach (Unit unit in units)
        {
            if (!allUnits.Contains(unit))
            {
                AddUnit(unit);
            }
        }
    }

    public void AddUnit(Unit unit)
    {
        allUnits.Add(unit);
        MapHandler.Instance.AddUnit(unit);
        Debug.Log($"Unit {unit.name} added to WorldServer and Map ID 1.");
    }

    public void RemoveUnit(Unit unit)
    {
        if (allUnits.Contains(unit))
        {
            allUnits.Remove(unit);
            MapHandler.Instance.RemoveUnit(unit);
            Debug.Log($"Unit {unit.name} removed from WorldServer and Map ID 1.");
        }
    }

    public void PlayerJoined(Player player)
    {
        // Handle player joining the game
        Debug.Log($"Player {player.name} has joined the game.");
        // Add player to the list as a unit
        AddUnit(player.GetComponent<Unit>());
    }

    public void PlayerLeft(Player player)
    {
        // Handle player leaving the game
        Debug.Log($"Player {player.name} has left the game.");
        // Remove player from the list as a unit
        RemoveUnit(player.GetComponent<Unit>());
    }

    public List<Unit> GetAllUnits()
    {
        return allUnits; 
    }
}
