using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public DatabaseManager DatabaseManager;
    public List<Character> Characters { get; private set; } = new List<Character>();

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void LoadCharactersForAccount(int accountId)
    {
        Characters = DatabaseManager.GetCharactersByAccountId(accountId);
        Debug.Log($"Loaded {Characters.Count} characters for account ID {accountId}.");
    }
}
