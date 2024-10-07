using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System.IO;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine.ProBuilder.MeshOperations;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    private SQLiteConnection _authConnection;
    private SQLiteConnection _charConnection;
    private SQLiteConnection _worldConnection;

    public GameObject belfFemaleModel;
    public GameObject orcMaleModel;
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

    public Character GetCharacterById(int characterId)
    {
        // Query the Character table to find the character with the specified ID
        var character = _charConnection.Table<Character>()
                         .FirstOrDefault(c => c.characterId == characterId);

        if (character != null)
        {
            Debug.Log($"Character ID {characterId} loaded successfully.");
            return character;
        }
        else
        {
            Debug.LogWarning($"No character found with ID {characterId}.");
            return null;
        }
    }


    public CharacterLocation GetCharacterLocation(int characterId)
    {
        // Query the CharacterLocation table to find the location for the specified character
        var location = _charConnection.Table<CharacterLocation>()
                        .FirstOrDefault(loc => loc.Id == characterId);

        if (location != null)
        {
            Debug.Log($"Location for Character ID {characterId} loaded successfully.");
            return location;
        }
        else
        {
            Debug.LogWarning($"No location found for Character ID {characterId}.");
            return null;
        }
    }

    public GameObject LoadCharacterModel(Character character)
    {
        // TODO: Load Char Model from DB
        switch (character.raceId)
        {
            case 0:
                break;
            case 1: // Blood Elf
                if (character.bodyType == 1)
                {
                    // TODO: Add Male Belf
                }
                else
                    return belfFemaleModel;
                break;
            case 2: // Orc
                if (character.bodyType == 1)
                {
                    return orcMaleModel;
                }
                else
                    return null; // TODO: Add Female Orc
        }

        return null;
    }

    public void UpdateCharacterLocation(CharacterLocation location)
    {
        string query = "UPDATE CharacterLocation SET x = ?, y = ?, z = ?, orientation = ? WHERE Id = ?";
        _charConnection.Execute(query, location.x, location.y, location.z, location.orientation, location.Id);
    }

    public int GetFactionId(int characterId)
    {
        var character = _charConnection.Table<Character>()
                        .FirstOrDefault(c => c.characterId == characterId);
        if (character != null)
            return character.factionId;
        else
        {
            Debug.LogWarning($"No character found with ID {characterId}.");
            return -1;
        }
    }

    public List<int> GetCharacterSpells(int characterId)
    {
        // Check if the character exists
        var character = GetCharacterById(characterId);
        if (character == null)
        {
            Debug.LogWarning($"Character with ID {characterId} not found.");
            return new List<int>();
        }

        // Get spells from CharacterSpells table (personal spells)
        var characterSpells = _charConnection.Table<CharacterSpells>()
                            .Where(cs => cs.CharacterId == characterId)
                            .Select(cs => cs.SpellId)
                            .ToList();

        var specSpellsData = _charConnection.Table<SpecSpells>()
                    .Where(ss => ss.SpecId == character.specId)
                    .ToList();

        List<int> specSpells = new List<int>();
        foreach (var spell in specSpellsData)
        {
            specSpells.Add(spell.SpellId);
        }



        // Get spells from ClassSpells table (class-based spells)
        var classSpells = _charConnection.Table<ClassSpells>()
                             .Where(cs => cs.ClassId == character.classId)
                             .Select(cs => cs.SpellId)
                             .ToList();

        // Combine all spells into a single list
        var allSpells = new List<int>();
        allSpells.AddRange(characterSpells); // Add personal spells
        allSpells.AddRange(specSpells); // Add spec-based spells
        allSpells.AddRange(classSpells); // Add class-based spells

        // Remove duplicates if any
        allSpells = allSpells.Distinct().ToList();

        Debug.Log($"Returning spells for character {characterId} with class {character.classId} and spec {character.specId}");
        return allSpells;
    }


    void OnDestroy()
    {
        _authConnection?.Close();
        _charConnection?.Close();
        _worldConnection?.Close();
    }
}

public class ClassSpells
{
    [PrimaryKey, AutoIncrement]
    public int ClassId { get; set; }
    public int SpellId { get; set; }
}

public class CharacterSpells
{
    [PrimaryKey, AutoIncrement]
    public int CharacterId { get; set; }
    public int SpellId { get; set; }
}

public class SpecSpells
{
    [PrimaryKey, AutoIncrement]
    public int SpecId { get; set; }
    public int SpellId { get; set; }
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

public class CharacterLocation
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int mapId { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
    public float orientation { get; set; }
}
