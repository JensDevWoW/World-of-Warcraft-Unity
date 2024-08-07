using Mirror;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Reference to the NetworkIdentity component
    public NetworkIdentity Identity { get; private set; }

    // Data
    public string m_name = "Voreli";
    public int m_health = 1000;
    public int m_mana = 100;

    public bool IsCasting { get; protected set; }

    // Initialize the NetworkIdentity reference
    void Start()
    {
        // Get the NetworkIdentity component attached to the same GameObject
        Identity = GetComponent<NetworkIdentity>();

        if (Identity == null)
        {
            Debug.LogError("NetworkIdentity component is missing on the Unit's GameObject.");
        }
    }

    // Method to set the unit as casting
    public virtual void SetCasting()
    {
        IsCasting = true;
        Debug.Log("Started casting");
    }

    // Method to stop the unit from casting
    public virtual void StopCasting()
    {
        IsCasting = false;
    }

    // Get the target unit (this can be adapted based on your actual target acquisition logic)
    public Unit GetTarget()
    {
        return null;
    }

    // Check if this unit is hostile to the given target
    public bool IsHostileTo(Unit target)
    {
        return true; // Replace with actual hostility logic
    }

    // Convert this Unit to a Player if applicable
    public Player ToPlayer()
    {
        return this as Player;
    }

    // Check if this unit is the local player
    public bool IsLocalPlayer()
    {
        return Identity != null && Identity.isLocalPlayer;
    }

    //public void DealDamage(Spell spell, Unit target, DamageType damageType)
   // {
     //   if (!spell || !target)
     //       return;

        //SpellInfo info = spell.GetSpellInfo();

        //SpellSchoolMask school = info.SpellSchoolMask;

       // SpellType type = info.SpellType;


 //   }
}
