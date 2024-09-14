using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.SceneManagement;

public class Character_Load : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _loaded = false;
    private void Update()
    {
        if (ClientAccountManager.Instance.GetCurrentAccount() != null && !_loaded)
        {
            // Check if the CharacterSelectionScene is loaded
            if (SceneManager.GetSceneByName("CharacterSelectionScene").isLoaded)
            {
                // Ensure the CharacterSelectionManager has received characters
                if (CharacterSelectionManager.Instance.HasReceivedCharacterList())
                {
                    CharacterSelectionManager.Instance.LoadCharacters();
                    _loaded = true;
                    Destroy(this);
                }
            }
        }
    }

}
