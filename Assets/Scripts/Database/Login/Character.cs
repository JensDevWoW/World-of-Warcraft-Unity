using SQLite4Unity3d;

public class Character
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Foreign key linking to the Account table
    public int accountId { get; set; }

    // Character-specific attributes
    public int classId { get; set; } // e.g., Mage, Warrior, etc.
    public int specId { get; set; } // Specialization within the class
    public string characterName { get; set; } // Name of the character
    public int factionId { get; set; } // Faction (e.g., Alliance, Horde)

    // Additional fields can be added as needed
    public int Level { get; set; } // Character level
    public int Experience { get; set; } // Experience points
    public float Health { get; set; } // Current health
    public float MaxHealth { get; set; } // Maximum health
    public float Mana { get; set; } // Current mana
    public float MaxMana { get; set; } // Maximum mana
}
