using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.SceneManagement;

public class Character_Load : MonoBehaviour
{
    // Start is called before the first frame update
    public bool loaded = false;
    public static Character_Load Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Ensure there's only one instance of this script
        }
    }

    private void Update()
    {
        if (ClientAccountManager.Instance.GetCurrentAccount() != null && !loaded)
        {
            // Check if the CharacterSelectionScene is loaded
            if (SceneManager.GetSceneByName("CharacterSelectionScene").isLoaded)
            {
                // Ensure the CharacterSelectionManager has received characters
                if (CharacterSelectionManager.Instance.HasReceivedCharacterList())
                {
                    CharacterSelectionManager.Instance.LoadCharacters();
                    loaded = true;
                }
            }
        }
    }

}
