using UnityEngine;

public class Player : MonoBehaviour
{
    public string playerName;
    public int level;

    public Unit unit { get; set; }

    public void SetCasting()
    {
        unit.SetCasting(); // Optionally call the base class implementation
        Debug.Log($"{playerName} is casting a spell at level {level}.");
        // Add any additional logic specific to the Player class here
    }

    public void SetUnit( Unit unit )
    {
        this.unit = unit;
    }

    public void StopCasting()
    {
        unit.StopCasting(); // Optionally call the base class implementation
        Debug.Log($"{playerName} has stopped casting.");
        // Add any additional logic specific to the Player class here
    }

    public Unit ToUnit()
    {
        return unit;
    }

    public void UpdatePlayer()
    {
        // Not important atm
    }
}
