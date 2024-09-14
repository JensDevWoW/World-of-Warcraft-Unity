using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System.Linq;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    private SQLiteConnection _authConnection;
    private SQLiteConnection _charConnection;
    private SQLiteConnection _worldConnection;

    public List<Account> Accounts { get; private set; } = new List<Account>();
    public List<SpawnData> WorldData { get; private set; } = new List<SpawnData>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Updated paths to point to the 'Database' folder in Assets
        string authdbPath = Path.Combine(Application.dataPath, "Database", "auth.db");
        string charactersdbPath = Path.Combine(Application.dataPath, "Database", "characters.db");
        string worlddbPath = Path.Combine(Application.dataPath, "Database", "world.db");

        _authConnection = new SQLiteConnection(authdbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        _charConnection = new SQLiteConnection(charactersdbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        _worldConnection = new SQLiteConnection(worlddbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        LoadAccounts();
        LoadWorldData();
    }

    private void LoadAccounts()
    {
        Accounts = _authConnection.Table<Account>().ToList();
        Debug.Log("Accounts loaded successfully.");
    }

    private void LoadWorldData()
    {
        WorldData = _worldConnection.Table<SpawnData>().ToList();
        Debug.Log("World data loaded successfully.");
    }

    public Account GetAccountByUsername(string username)
    {
        return Accounts.Find(account => account.accountName == username);
    }

    public List<Character> GetCharactersByAccountId(int accountId)
    {
        return _charConnection.Table<Character>().Where(character => character.accountId == accountId).ToList();
    }

    void OnDestroy()
    {
        _authConnection?.Close();
        _charConnection?.Close();
        _worldConnection?.Close();
    }
}

public class SpawnData
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string PrefabName { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public string ParentTag { get; set; }
    public int UI { get; set; }
    public bool HasCoordinates => !(X == 0 && Y == 0 && Z == 0);
}
