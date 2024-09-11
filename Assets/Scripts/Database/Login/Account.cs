using SQLite4Unity3d;

public class Account
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; } // Unique identifier for each account

    public string accountName { get; set; } // Username for the account
    public string accountPass { get; set; } // Password for the account (should be securely stored and encrypted)
    public string Email { get; set; } // Optional email associated with the account

    // Additional fields can be added as needed
    public int AccountStatus { get; set; } // Status of the account (e.g., active, banned)
}
