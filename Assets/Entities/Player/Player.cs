using UnityEngine;

public class Player : Unit
{
    public string playerName;
    public int level;

    public override void SetCasting()
    {
        base.SetCasting(); // Optionally call the base class implementation
        Debug.Log($"{playerName} is casting a spell at level {level}.");
        // Add any additional logic specific to the Player class here
    }

    public override void StopCasting()
    {
        base.StopCasting(); // Optionally call the base class implementation
        Debug.Log($"{playerName} has stopped casting.");
        // Add any additional logic specific to the Player class here
    }
}
